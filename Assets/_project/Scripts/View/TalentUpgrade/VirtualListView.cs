using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class VirtualListView : MonoBehaviour
    {

        [Serializable] public class BindCellEvent : UnityEvent<int, TalentItemInList> { }
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField]  private RectTransform _viewport;
        [SerializeField]  private RectTransform _content;

        [Header("Core")]
        public RectTransform itemPrefab;     // префаб, на котором есть TalentItemView (или он добавится)
        [Min(1)] public int poolSize = 15;
        

        [Header("Layout")]
        public float itemHeight = 80f;
        public float spacing = 6f;
        public float paddingTop = 6f;
        public float paddingBottom = 6f;
        public float paddingLeft = 6f;
        public float paddingRight = 6f;

        [Header("Binding (на выбор)")]
        public BindCellEvent OnBind;               // Инспекторный путь: вы сами делаете cell.Bind(names[index])
        public Func<int, string> nameProvider;     // Кодовый путь: listView.nameProvider = i => names[i];

        // R3: централизованный поток кликов из списка (index, name)
        public readonly Subject<(int index, string name)> ItemClicked = new Subject<(int, string)>();        

        private readonly List<RectTransform> _pool = new();
        private readonly List<TalentItemInList> _cells = new();

        private int totalCount = 0;
        private int _firstVisible = -1;
        private bool _initialized;
        private bool _warnedNoData;
        private List<string> _names = new List<string>();

        private float Stride => itemHeight + spacing;

        void Awake()
        {
            _scrollRect.horizontal = false;
            _scrollRect.vertical = true;
            _scrollRect.movementType = ScrollRect.MovementType.Clamped;

            SetupTopStretch(_content, keepPos: false);
            _scrollRect.onValueChanged.AddListener(_ => UpdateVisible());
        }

        void OnEnable()
        {
            EnsureInitialized();
            Refresh(true);
        }

        void OnRectTransformDimensionsChange()
        {
            if (!isActiveAndEnabled) return;
            Refresh(true);
        }

        public void SetNames(IList<string> names, bool keepPosition = true)
        {
            _names = names != null ? new List<string>(names) : null;
            totalCount = _names?.Count ?? 0;
            if (!keepPosition) _content.anchoredPosition = Vector2.zero;
            Refresh(true);
        }

        public void SetTotalCount(int count, bool keepPosition = true)
        {
            totalCount = Mathf.Max(0, count);
            if (!keepPosition) _content.anchoredPosition = Vector2.zero;
            Refresh(true);
        }

        public void ScrollToIndex(int index)
        {
            index = Mathf.Clamp(index, 0, Mathf.Max(0, totalCount - 1));
            var pos = _content.anchoredPosition;
            pos.y = paddingTop + index * Stride;
            _content.anchoredPosition = pos;
            UpdateVisible();
        }

        void EnsureInitialized()
        {
            if (_initialized) return;
            if (itemPrefab == null) { Debug.LogError("[VirtualListView] Не назначен itemPrefab."); enabled = false; return; }

            // очистка
            for (int i = _content.childCount - 1; i >= 0; i--) Destroy(_content.GetChild(i).gameObject);
            _pool.Clear(); _cells.Clear();

            int count = Mathf.Min(poolSize, Math.Max(1, totalCount));
            for (int i = 0; i < count; i++)
            {
                var rt = Instantiate(itemPrefab, _content);
                SetupItemRect(rt);
                rt.gameObject.name = $"Item_{i}";
                _pool.Add(rt);

                // кешируем компонент ячейки один раз
                var cell = rt.GetComponent<TalentItemInList>() ?? rt.gameObject.AddComponent<TalentItemInList>();
                _cells.Add(cell);

                // одна подписка на клик на весь срок жизни объекта
                cell.Clicked
                    .Subscribe(name => ItemClicked.OnNext((cell.Index, name)))
                    .AddTo(rt.gameObject);
            }
            _initialized = true;
        }

        public void Refresh(bool forceRebuild = false)
        {
            if (!_initialized) EnsureInitialized();

            float contentHeight = paddingTop + paddingBottom + Mathf.Max(0, totalCount) * Stride - spacing;
            if (contentHeight < 0f) contentHeight = 0f;

            var size = _content.sizeDelta; size.y = contentHeight; _content.sizeDelta = size;

            foreach (var rt in _pool) SetupItemRect(rt);
            if (forceRebuild) _firstVisible = -1;

            ClampContentPosition();
            UpdateVisible();
        }

        void UpdateVisible()
        {
            if (totalCount == 0 || _pool.Count == 0) { HideAll(); return; }

            int newFirst = Mathf.FloorToInt((_content.anchoredPosition.y - paddingTop) / Stride);
            if (float.IsNaN(newFirst)) newFirst = 0;
            newFirst = Mathf.Clamp(newFirst, 0, Mathf.Max(0, totalCount - 1));
            if (newFirst == _firstVisible) return;
            _firstVisible = newFirst;

            for (int i = 0; i < _pool.Count; i++)
            {
                int dataIndex = _firstVisible + i;
                var rt = _pool[i];
                var cell = _cells[i];

                if (dataIndex >= totalCount)
                {
                    rt.gameObject.SetActive(false);
                    continue;
                }

                rt.gameObject.SetActive(true);
                PositionItem(rt, dataIndex);

                // ВАЖНО: СНАЧАЛА индекс (для корректного ItemClicked), затем Bind(...)
                cell.Index = dataIndex;

                // 3 варианта получения имени: OnBind (ручной), nameProvider (код), SetNames (список)
                bool bound = false;

                if (OnBind != null && OnBind.GetPersistentEventCount() > 0)
                {
                    OnBind.Invoke(dataIndex, cell);
                    bound = true;
                }
                else if (nameProvider != null)
                {
                    cell.Bind(nameProvider(dataIndex));
                    bound = true;
                }
                else if (_names != null && dataIndex < _names.Count)
                {
                    cell.Bind(_names[dataIndex]);
                    bound = true;
                }

                if (!bound && !_warnedNoData)
                {
                    Debug.LogWarning("[VirtualListView] Нет данных для биндинга: используйте SetNames(...), nameProvider или подпишитесь на OnBind.");
                    _warnedNoData = true;
                }
            }
        }

        void HideAll()
        {
            foreach (var rt in _pool) rt.gameObject.SetActive(false);
        }

        void SetupItemRect(RectTransform rt)
        {
            SetupTopStretch(rt, keepPos: true);
            var offMin = rt.offsetMin; offMin.x = paddingLeft; rt.offsetMin = offMin;
            var offMax = rt.offsetMax; offMax.x = -paddingRight; rt.offsetMax = offMax;
            var size = rt.sizeDelta; size.y = itemHeight; rt.sizeDelta = size;
        }

        void PositionItem(RectTransform rt, int dataIndex)
        {
            float y = -(paddingTop + dataIndex * Stride);
            var ap = rt.anchoredPosition; ap.y = y; rt.anchoredPosition = ap;
        }

        void ClampContentPosition()
        {
            float viewportH = _viewport.rect.height;
            float contentH = _content.rect.height;
            var pos = _content.anchoredPosition;
            float maxY = Mathf.Max(0f, contentH - viewportH);
            pos.y = Mathf.Clamp(pos.y, 0f, maxY);
            _content.anchoredPosition = pos;
        }

        static void SetupTopStretch(RectTransform rt, bool keepPos)
        {
            Vector2 oldPos = rt.anchoredPosition;
            Vector2 oldSize = rt.sizeDelta;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);

            if (!keepPos)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = oldSize;
            }
            else
            {
                rt.anchoredPosition = oldPos;
                rt.sizeDelta = oldSize;
            }
        }
    }

}

