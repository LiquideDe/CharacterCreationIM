using System;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class UpgradeItemsListPresenter
    {
        public event Action UpgradeCancelled;
        private readonly UpgradeItemsListView _view;
        private readonly List<UpgradeEntry> _entries = new();
        private bool _isUpgradeSessionActive;

        public UpgradeItemsListPresenter(UpgradeItemsListView view)
        {
            _view = view;
        }

        public void BeginUpgradeSession()
        {
            if (!_isUpgradeSessionActive)
            {
                Clear();
                _isUpgradeSessionActive = true;
            }

            _view.Show();
        }

        public void EndUpgradeSession()
        {
            Clear();
            _view.Hide();
            _isUpgradeSessionActive = false;
        }

        public void AddUpgrade(string name, int cost, IGameCommand command, Action afterCancel, Func<bool> isStillValid = null)
        {
            UpgradeEntry entry = null;
            var item = _view.CreateUpgradeItem(name, cost, () => CancelUpgrade(entry));
            entry = new UpgradeEntry(item, command, afterCancel, isStillValid);
            _entries.Add(entry);
        }

        private void CancelUpgrade(UpgradeEntry entry)
        {
            Debug.Log($"[FIX:UpgradeExperience] Cancelling upgrade '{entry.Item.name}' and refreshing active upgrade views.");
            RemoveUpgrade(entry);
            RemoveInvalidUpgrades();
            UpgradeCancelled?.Invoke();
        }

        private void RemoveInvalidUpgrades()
        {
            var removedEntry = true;
            while (removedEntry)
            {
                removedEntry = false;
                for (var index = _entries.Count - 1; index >= 0; index--)
                {
                    var entry = _entries[index];
                    if (entry.IsStillValid == null || entry.IsStillValid())
                        continue;

                    RemoveUpgrade(entry);
                    removedEntry = true;
                    break;
                }
            }
        }

        private void RemoveUpgrade(UpgradeEntry entry)
        {
            entry.Command.Undo();
            entry.AfterCancel?.Invoke();
            _entries.Remove(entry);
            _view.RemoveUpgradeItem(entry.Item);
        }

        private void Clear()
        {
            _entries.Clear();
            _view.Clear();
        }

        private sealed class UpgradeEntry
        {
            public UpgradeItem Item { get; }
            public IGameCommand Command { get; }
            public Action AfterCancel { get; }
            public Func<bool> IsStillValid { get; }

            public UpgradeEntry(UpgradeItem item, IGameCommand command, Action afterCancel, Func<bool> isStillValid)
            {
                Item = item;
                Command = command;
                AfterCancel = afterCancel;
                IsStillValid = isStillValid;
            }
        }
    }
}
