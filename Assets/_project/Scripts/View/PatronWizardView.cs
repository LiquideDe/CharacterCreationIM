using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    // The prefab contains this component; its controls are built when the screen opens.
    public class PatronWizardView : ViewBase
    {
        public sealed class Choice
        {
            public string Title;
            public string Description;
            public bool Available = true;
            public bool Selected;
            public Action Select;
        }

        private RectTransform _content;
        private TMP_Text _title;
        private TMP_Text _summary;
        private TMP_Text _message;
        private TMP_InputField _nameField;
        private TMP_InputField _searchField;
        private ScrollRect _scroll;
        private readonly List<(GameObject item, string searchableText, bool heading)> _choiceObjects = new();
        private Button _next;
        private Button _back;
        private Button _cancel;
        public Action NextClicked;
        public Action BackClicked;
        public Action CancelClicked;
        public string NameText => _nameField != null ? _nameField.text : string.Empty;

        protected override void Awake()
        {
            Build();
        }

        public override void Show() { }
        public override void HideAndDestroyToLeft() => Destroy(gameObject);

        public void Render(string title, string summary, IEnumerable<Choice> choices, bool showName, string name, string message, bool canBack, bool canNext, bool searchable = false)
        {
            var sameStep = _title.text == title;
            var scrollPosition = _scroll.verticalNormalizedPosition;
            var searchText = sameStep ? _searchField.text : string.Empty;
            _title.text = title;
            _summary.text = summary;
            _message.text = message ?? string.Empty;
            _searchField.gameObject.SetActive(searchable);
            _searchField.SetTextWithoutNotify(searchable ? searchText : string.Empty);
            _back.interactable = canBack;
            _next.interactable = canNext;
            _next.GetComponentInChildren<TMP_Text>().text = showName ? "Сохранить" : "Далее";
            for (var i = _content.childCount - 1; i >= 0; i--)
            {
                _content.GetChild(i).gameObject.SetActive(false);
                Destroy(_content.GetChild(i).gameObject);
            }
            _nameField = null;
            _choiceObjects.Clear();
            if (showName) CreateNameField(name);
            foreach (var choice in choices)
            {
                var text = (choice.Selected ? "✓ " : string.Empty) + choice.Title;
                if (!string.IsNullOrWhiteSpace(choice.Description)) text += "\n" + choice.Description;
                if (!choice.Available) text += "\n[Нет записи в справочнике]";
                var button = MakeButton(_content, text, choice.Select, 100);
                button.interactable = choice.Available && choice.Select != null;
                button.GetComponent<Image>().color = choice.Selected ? new Color(0.20f, 0.40f, 0.30f) : new Color(0.18f, 0.20f, 0.26f);
                _choiceObjects.Add((button.gameObject, text, choice.Select == null));
            }
            ApplyFilter(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
            _scroll.verticalNormalizedPosition = sameStep ? scrollPosition : 1f;
        }

        private void ApplyFilter(bool resetScroll)
        {
            var query = _searchField.text.Trim();
            foreach (var choice in _choiceObjects)
                choice.item.SetActive(query.Length == 0 || choice.heading ||
                                      choice.searchableText.IndexOf(query, StringComparison.CurrentCultureIgnoreCase) >= 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);
            if (resetScroll) _scroll.verticalNormalizedPosition = 1f;
        }

        public void SetMessage(string message) => _message.text = message ?? string.Empty;

        private void Build()
        {
            var canvasGo = new GameObject("Patron Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 900);
            var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            panel.transform.SetParent(canvasGo.transform, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(80, 35);
            rect.offsetMax = new Vector2(-80, -35);
            panel.GetComponent<Image>().color = new Color(0.07f, 0.08f, 0.11f, 0.98f);
            var layout = panel.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(24, 24, 18, 18);
            layout.spacing = 12;
            layout.childControlHeight = false;
            layout.childForceExpandHeight = false;
            _title = MakeText(panel.transform, "Заголовок", 32, 45);
            _summary = MakeText(panel.transform, "Сводка", 21, 175);
            _searchField = MakeInputField(panel.transform, "Поиск возможности или слабости", string.Empty);
            _searchField.gameObject.SetActive(false);
            _searchField.onValueChanged.AddListener(_ => ApplyFilter(true));

            var scrollGo = new GameObject("Choices", typeof(RectTransform), typeof(Image), typeof(Mask), typeof(ScrollRect), typeof(LayoutElement));
            scrollGo.transform.SetParent(panel.transform, false);
            scrollGo.GetComponent<Image>().color = new Color(0.11f, 0.12f, 0.16f);
            scrollGo.GetComponent<Mask>().showMaskGraphic = true;
            scrollGo.GetComponent<LayoutElement>().flexibleHeight = 1;
            var contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentGo.transform.SetParent(scrollGo.transform, false);
            _content = contentGo.GetComponent<RectTransform>();
            _content.anchorMin = new Vector2(0, 1);
            _content.anchorMax = new Vector2(1, 1);
            _content.pivot = new Vector2(0.5f, 1);
            _content.anchoredPosition = Vector2.zero;
            _content.sizeDelta = Vector2.zero;
            var contentLayout = contentGo.GetComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(12, 12, 12, 12);
            contentLayout.spacing = 10;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandHeight = false;
            contentGo.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var scroll = scrollGo.GetComponent<ScrollRect>();
            _scroll = scroll;
            scroll.viewport = scrollGo.GetComponent<RectTransform>();
            scroll.content = _content;
            scroll.horizontal = false;

            _message = MakeText(panel.transform, "Сообщение", 20, 40);
            _message.color = new Color(1f, 0.65f, 0.5f);
            var navGo = new GameObject("Navigation", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            navGo.transform.SetParent(panel.transform, false);
            navGo.GetComponent<LayoutElement>().preferredHeight = 58;
            navGo.GetComponent<HorizontalLayoutGroup>().spacing = 12;
            _back = MakeButton(navGo.transform, "Назад", () => BackClicked?.Invoke(), 56);
            _cancel = MakeButton(navGo.transform, "Отмена", () => CancelClicked?.Invoke(), 56);
            _next = MakeButton(navGo.transform, "Далее", () => NextClicked?.Invoke(), 56);
        }

        private void CreateNameField(string name)
        {
            _nameField = MakeInputField(_content, "Имя покровителя", name);
        }

        private static TMP_InputField MakeInputField(Transform parent, string label, string value)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(TMP_InputField), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<LayoutElement>().preferredHeight = 60;
            go.GetComponent<Image>().color = Color.white;
            var text = MakeText(go.transform, "Текст", 27, 60);
            text.color = Color.black;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(12, 4);
            textRect.offsetMax = new Vector2(-12, -4);
            var placeholder = MakeText(go.transform, label, 27, 60);
            placeholder.color = new Color(0.45f, 0.45f, 0.45f);
            placeholder.rectTransform.anchorMin = Vector2.zero;
            placeholder.rectTransform.anchorMax = Vector2.one;
            placeholder.rectTransform.offsetMin = new Vector2(12, 4);
            placeholder.rectTransform.offsetMax = new Vector2(-12, -4);
            var input = go.GetComponent<TMP_InputField>();
            input.textViewport = go.GetComponent<RectTransform>();
            input.textComponent = (TextMeshProUGUI)text;
            input.placeholder = placeholder;
            input.text = value ?? string.Empty;
            return input;
        }

        private static TMP_Text MakeText(Transform parent, string value, int size, float height)
        {
            var go = new GameObject(value, typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<LayoutElement>().preferredHeight = height;
            var text = go.GetComponent<TextMeshProUGUI>();
            text.fontSize = size;
            text.text = value;
            text.color = Color.white;
            text.textWrappingMode = TextWrappingModes.Normal;
            return text;
        }

        private static Button MakeButton(Transform parent, string label, Action action, float height)
        {
            var go = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<LayoutElement>().preferredHeight = height;
            go.GetComponent<Image>().color = new Color(0.18f, 0.20f, 0.26f);
            var button = go.GetComponent<Button>();
            if (action != null) button.onClick.AddListener(() => action());
            var text = MakeText(go.transform, label, 20, height);
            text.alignment = TextAlignmentOptions.Center;
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            return button;
        }
    }
}
