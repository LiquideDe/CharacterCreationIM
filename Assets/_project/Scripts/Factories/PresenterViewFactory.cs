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
            var view = _container.InstantiatePrefabForComponent<TView>(prefab);
            var presenter = (IPresenter)_container.Instantiate(presenterType, new object[] { view, _audioManager });
            presenter.Initialize();

            return presenter;
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

