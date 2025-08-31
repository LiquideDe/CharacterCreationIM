using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class PresenterViewFactory
    {
        private readonly ViewPrefab _prefabMap;
        private readonly DiContainer _container;
        private readonly AudioManager _audioManager;

        public PresenterViewFactory(ViewPrefab prefabMap, DiContainer container, AudioManager audioManager)
        {
            _prefabMap = prefabMap;
            _container = container;
            _audioManager = audioManager;
        }

        public IPresenter Create<TView>() where TView : ViewBase
        {
            var viewType = typeof(TView);
            var presenterType = FindPresenterTypeFor(viewType);
            if (presenterType == null)
            {
                Debug.LogError($"Presenter not found for view type: {viewType.Name}");
                return null;
            }
            var prefab = _prefabMap.GetPrefab(viewType.Name);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found for view type: {viewType.Name}");
                return null;
            }
            //var view = _container.InstantiatePrefabForComponent<TView>(prefab);
            var view = SafeSpawnView<TView>(prefab, _container);
            var presenter = (IPresenter)_container.Instantiate(presenterType, new object[] { view, _audioManager });
            presenter.Initialize();
            view.gameObject.SetActive(true);
            return presenter;
        }

        TView SafeSpawnView<TView>(GameObject prefab, DiContainer container)
    where TView : Component
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));

            try
            {
                // Важно: сразу указываем родителя
                var go = container.InstantiatePrefab(prefab);

                // Логи активности
                var rt = go.transform;
                Debug.Log($"[Spawn] {go.name} self:{go.activeSelf} inHierarchy:{go.activeInHierarchy} " +
                          $"parent:{rt.parent?.name} parentActive:{rt.parent?.gameObject.activeInHierarchy}");

                // ЯВНО ищем компонент (и на корне, и в детях, включая неактивных)
                var view = go.GetComponentInChildren<TView>(true);
                if (view == null)
                    throw new InvalidOperationException(
                        $"На инстансе '{go.name}' не найден компонент {typeof(TView).Name} (ни на корне, ни в детях).");

                // На всякий случай нормализуем трансформ
                view.transform.localScale = Vector3.one;

                // Если корень префаба в ассете выключен — инстанс родится выключенным.
                // Включим явно на ГОшке вьюхи (если у тебя вью — не корневой объект).
                view.gameObject.SetActive(true);

                return view;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Spawn ERROR] {typeof(TView).Name} из префаба '{prefab.name}':\n{ex}");
                throw; // чтобы увидеть стек в консоли/дебаггере
            }
        }

        private Type FindPresenterTypeFor(Type viewType)
        {
            // Предполагаем соглашение: MainMenuView → MainMenuPresenter
            var presenterName = viewType.Name.Replace("View", "Presenter");

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t =>
                    typeof(IPresenter).IsAssignableFrom(t) &&
                    t.Name == presenterName);
        }
    }
}

