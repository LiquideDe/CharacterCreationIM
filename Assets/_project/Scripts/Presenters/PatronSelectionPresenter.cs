using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public sealed class PatronSelectionPresenter : ICharacterPresenter
    {
        [Inject] private PatronStorage _storage;
        private readonly PatronSelectionView _view;
        private readonly AudioManager _audio;
        private readonly Subject<Character> _nextClicked = new();
        private Character _character;
        private string _selected;
        public Observable<Character> NextClicked => _nextClicked;

        public PatronSelectionPresenter(AudioManager audioManager, PatronSelectionView view)
        {
            _audio = audioManager;
            _view = view;
        }

        public void Initialize()
        {
            _view.NextClicked = Confirm;
            _view.BackClicked = Confirm;
            _view.CancelClicked = Confirm;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
            _selected = character.Patron.Value;
            Render();
        }

        public void Dispose()
        {
            _view.NextClicked = null;
            _view.BackClicked = null;
            _view.CancelClicked = null;
            _nextClicked.Dispose();
        }

        private void Confirm()
        {
            if (_character == null) return;
            _character.Patron.Value = _selected;
            _audio.PlayConfirm();
            _view.HideAndDestroyToLeft();
            _nextClicked.OnNext(_character);
        }

        private void Render()
        {
            var names = _storage.ListNames();
            var choices = new List<PatronWizardView.Choice>
            {
                new() { Title = "Без покровителя", Selected = string.IsNullOrEmpty(_selected), Select = () => { _selected = null; Render(); } }
            };
            foreach (var name in names)
            {
                var selected = name;
                _storage.TryLoad(name, out var patron);
                choices.Add(new PatronWizardView.Choice
                {
                    Title = selected,
                    Description = patron == null ? "Ошибка чтения файла" :
                        $"{patron.Faction} — {patron.Responsibility}; {patron.Possibility}" +
                        (patron.PossibilityVariants.Count > 0 ? $" ({string.Join(", ", patron.PossibilityVariants)})" : string.Empty) +
                        $"; {patron.Weakness}",
                    Selected = _selected == selected,
                    Available = patron != null,
                    Select = () => { _selected = selected; Render(); }
                });
            }
            var missing = !string.IsNullOrEmpty(_selected) && !names.Contains(_selected);
            if (missing)
            {
                Debug.LogWarning($"[PatronSelection] У персонажа «{_character.Name.Value}» не найден покровитель «{_selected}».");
                choices.Add(new PatronWizardView.Choice { Title = _selected, Description = "Файл покровителя отсутствует; эффекты не действуют. Выберите другого или оставьте прежнее имя.", Selected = true, Available = false });
            }
            _view.Render("Покровитель персонажа", $"Выбран: {_selected ?? "—"}", choices, false, null,
                missing ? "Сохранённая ссылка не найдена. При продолжении она сохранится без эффектов." : null, false, true);
        }
    }
}
