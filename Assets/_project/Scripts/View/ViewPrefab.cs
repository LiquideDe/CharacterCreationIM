using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreation 
{
    [CreateAssetMenu(menuName = "Factories/View Prefab Map")]
    public class ViewPrefab : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string key;
            public GameObject prefab;
        }

        public List<Entry> views;

        private Dictionary<string, GameObject> _viewLookup;

        public void Initialize()
        {
            _viewLookup = views.ToDictionary(e => e.key, e => e.prefab);
        }

        public GameObject GetPrefab(string key)
        {
            if (_viewLookup == null) Initialize();

            if (_viewLookup.TryGetValue(key, out var prefab))
                return prefab;

            Debug.LogError($"Prefab not found for key: {key}");
            return null;
        }
    }
}


