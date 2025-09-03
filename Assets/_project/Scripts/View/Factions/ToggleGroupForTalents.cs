using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation.Background
{
    public class ToggleGroupForTalents : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Text = null;
        private List<Toggle> _toggles = new();

        public int MinSelected = 1;  // минимум, который нельзя «разщёлкать» ниже
        public int MaxSelected;

        private bool _changing;
        private readonly Dictionary<Toggle, int> _order = new();
        private int _seq;

        public void AddToggle(Toggle t)
        {
            if (_toggles.Contains(t)) return;
            _toggles.Add(t);
            t.onValueChanged.AddListener((isOn) => OnToggleChanged(t, isOn));
            EnforceLimitsInitial();
        }

        public int SelectedCount() => _toggles.Count(x => x && x.isOn);
        

        private void OnToggleChanged(Toggle t, bool isOn)
        {
            if (_changing) return;
            _changing = true;

            if (MaxSelected < 1) MaxSelected = 1;
            if (MinSelected < 0) MinSelected = 0;
            if (MinSelected > MaxSelected) MinSelected = MaxSelected;

            var on = _toggles.Where(x => x && x.isOn).ToList();

            if (isOn)
            {
                if (!_order.ContainsKey(t)) _order[t] = ++_seq;

                if (on.Count > MaxSelected)
                {                                       
                        var toOff = on
                            .Where(x => x != t)
                            .OrderBy(x => _order.TryGetValue(x, out var o) ? o : int.MaxValue)
                            .FirstOrDefault();

                        if (toOff != null)
                        {
                            toOff.isOn = false;
                            on.Remove(toOff);
                            _order.Remove(toOff);
                        }
                        else
                        {
                            t.isOn = false;
                            _order.Remove(t);
                        }
                    
                }
            }
            else
            {
                if (on.Count < MinSelected)
                {
                    t.isOn = true;
                    if (!_order.ContainsKey(t)) _order[t] = ++_seq;
                }
                else
                {
                    _order.Remove(t);
                }
            }

            _changing = false;

        }

        private void EnforceLimitsInitial()
        {
            var on = _toggles.Where(x => x && x.isOn).ToList();
            if (on.Count > MaxSelected)
            {
                var keep = on.OrderBy(x => _order.TryGetValue(x, out var o) ? o : int.MaxValue)
                             .Take(MaxSelected)
                             .ToHashSet();
                foreach (var t in on)
                    if (!keep.Contains(t)) { t.isOn = false; _order.Remove(t); }
            }

            
            for (int i = 0; _toggles != null && on.Count < MinSelected && i < _toggles.Count; i++)
            {
                var t = _toggles[i];
                if (t && !t.isOn) { t.isOn = true; on.Add(t); _order[t] = ++_seq; }
            }
        }
    }
}

