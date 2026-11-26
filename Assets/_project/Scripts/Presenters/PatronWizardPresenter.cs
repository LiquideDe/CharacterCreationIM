using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public sealed class PatronWizardPresenter : IPresenter
    {
        [Inject] private PatronCreator _creator;
        [Inject] private PatronStorage _storage;
        [Inject] private PatronEffects _effects;
        private readonly PatronWizardView _view;
        private readonly Subject<Unit> _finished = new();
        private readonly AudioManager _audio;
        private PatronTemplate _template;
        private string _faction;
        private string _possibility;
        private readonly List<string> _possibilityVariants = new();
        private string _weakness;
        private string _aspiration;
        private string _personality;
        private string _name;
        private int _step;
        private string _message;

        public Observable<Unit> Finished => _finished;

        public PatronWizardPresenter(AudioManager audioManager, PatronWizardView view)
        {
            _audio = audioManager;
            _view = view;
        }

        public void Initialize()
        {
            _view.NextClicked = Next;
            _view.BackClicked = Back;
            _view.CancelClicked = Cancel;
            Render();
        }

        public void Dispose()
        {
            _view.NextClicked = null;
            _view.BackClicked = null;
            _view.CancelClicked = null;
            _finished.Dispose();
        }

        private void Next()
        {
            if (_step == 6)
            {
                _name = _view.NameText;
                Save();
                return;
            }
            if (!CompleteStep())
            {
                _message = "Сначала выберите значение на этом шаге.";
                _audio.PlayError();
                Render();
                return;
            }
            _step = _step == 2 && !HasVariants() ? 4 : _step + 1;
            _message = null;
            _audio.PlayClick();
            Render();
        }

        private void Back()
        {
            if (_step == 0) return;
            if (_step == 6) _name = _view.NameText;
            _step = _step == 4 && !HasVariants() ? 2 : _step - 1;
            _message = null;
            Render();
        }

        private void Cancel()
        {
            Debug.Log("[PatronWizard] Создание отменено; файл не создан.");
            _view.HideAndDestroyToLeft();
            _finished.OnNext(Unit.Default);
        }

        private bool CompleteStep() => _step switch
        {
            0 => !string.IsNullOrEmpty(_faction),
            1 => _template != null,
            2 => !string.IsNullOrEmpty(_possibility) && !string.IsNullOrEmpty(_weakness),
            3 => ValidVariants(),
            4 => !string.IsNullOrEmpty(_aspiration),
            5 => !string.IsNullOrEmpty(_personality),
            _ => true
        };

        private bool HasVariants() => _creator.Possibility(_possibility)?.variants?.Count > 0;

        private bool ValidVariants()
        {
            var option = _creator.Possibility(_possibility);
            if (option?.variants == null || option.variants.Count == 0) return true;
            return _possibilityVariants.Count > 0 &&
                   (option.repeatable || _possibilityVariants.Count == 1) &&
                   _possibilityVariants.All(name => option.variants.Any(variant => variant.name == name));
        }

        private void Save()
        {
            if (_template == null || string.IsNullOrEmpty(_possibility) || string.IsNullOrEmpty(_weakness) ||
                string.IsNullOrEmpty(_aspiration) || string.IsNullOrEmpty(_personality) || !ValidVariants())
            {
                _message = "Данные покровителя неполные.";
                Render();
                return;
            }
            var patron = new Patron
            {
                Name = _name,
                Faction = _template.faction,
                Responsibility = _template.responsibilities,
                Opportunity = _template.opportunity,
                Possibility = _possibility,
                PossibilityVariants = new List<string>(_possibilityVariants),
                Weakness = _weakness,
                Aspiration = _aspiration,
                Personality = _personality,
                Influence = _effects.CalculateInfluence(_template, _weakness, _possibility)
            };
            if (!_storage.TrySave(patron, out var error))
            {
                _message = error;
                _audio.PlayError();
                Render();
                return;
            }
            _audio.PlayConfirm();
            _view.HideAndDestroyToLeft();
            _finished.OnNext(Unit.Default);
        }

        private void Render()
        {
            var choices = new List<PatronWizardView.Choice>();
            var title = string.Empty;
            if (_step == 0)
            {
                title = "Служба покровителя";
                foreach (var faction in _creator.Templates.Select(t => t.faction).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                {
                    var selected = faction;
                    choices.Add(new PatronWizardView.Choice { Title = selected, Selected = _faction == selected,
                        Select = () => { if (_faction != selected) { _faction = selected; _template = null; _possibility = null; _possibilityVariants.Clear(); _weakness = null; } Render(); } });
                }
            }
            else if (_step == 1)
            {
                title = "Должность покровителя";
                foreach (var template in _creator.Templates.Where(t => t.faction == _faction))
                {
                    var selected = template;
                    choices.Add(new PatronWizardView.Choice { Title = selected.responsibilities, Description = selected.description,
                        Selected = _template == selected, Select = () => { if (_template != selected) { _template = selected; _possibility = null; _possibilityVariants.Clear(); _weakness = null; } Render(); } });
                }
            }
            else if (_step == 2)
            {
                title = "Возможность и слабость";
                choices.Add(new PatronWizardView.Choice { Title = "Возможность должности — выберите одну", Description = $"Обязательная: {_template.opportunity}. {_template.description_oppotrunity}" });
                var recommended = new HashSet<string>(StringComparer.Ordinal);
                foreach (var name in _template.possibilities ?? new List<string>())
                {
                    var selected = name.Trim();
                    if (!recommended.Add(selected) || selected == _template.opportunity) continue;
                    var option = _creator.Possibility(selected);
                    choices.Add(new PatronWizardView.Choice { Title = selected, Description = option?.description,
                        Available = option != null, Selected = _possibility == selected,
                        Select = () => { if (_possibility != selected) { _possibility = selected; _possibilityVariants.Clear(); } Render(); } });
                }
                choices.Add(new PatronWizardView.Choice { Title = "Другие общие возможности", Description = "Их можно выбрать для любого покровителя." });
                foreach (var option in _creator.Possibilities.Where(item => !recommended.Contains(item.name) && item.name != _template.opportunity)
                             .OrderBy(item => item.name, StringComparer.CurrentCultureIgnoreCase))
                {
                    var selected = option.name;
                    choices.Add(new PatronWizardView.Choice { Title = selected, Description = option.description,
                        Selected = _possibility == selected,
                        Select = () => { if (_possibility != selected) { _possibility = selected; _possibilityVariants.Clear(); } Render(); } });
                }
                choices.Add(new PatronWizardView.Choice { Title = "Слабость — выберите одну" });
                foreach (var name in _template.weakness ?? new List<string>())
                {
                    var selected = name.Trim();
                    var option = _creator.Weakness(selected);
                    choices.Add(new PatronWizardView.Choice { Title = selected, Description = option?.description,
                        Available = option != null, Selected = _weakness == selected,
                        Select = () => { _weakness = selected; Render(); } });
                }
            }
            else if (_step == 3)
            {
                var option = _creator.Possibility(_possibility);
                title = $"Тип: {_possibility}";
                choices.Add(new PatronWizardView.Choice
                {
                    Title = option?.repeatable == true ? "Выберите один или несколько разных типов" : "Выберите один тип",
                    Description = option?.description
                });
                foreach (var variant in option?.variants ?? new List<PatronVariant>())
                {
                    var selected = variant.name;
                    choices.Add(new PatronWizardView.Choice
                    {
                        Title = selected,
                        Description = variant.description,
                        Selected = _possibilityVariants.Contains(selected),
                        Select = () =>
                        {
                            if (option.repeatable)
                            {
                                if (!_possibilityVariants.Remove(selected)) _possibilityVariants.Add(selected);
                            }
                            else
                            {
                                _possibilityVariants.Clear();
                                _possibilityVariants.Add(selected);
                            }
                            Render();
                        }
                    });
                }
            }
            else if (_step == 4)
            {
                title = "Стремление";
                foreach (var option in _creator.Aspirations)
                {
                    var selected = option.name;
                    choices.Add(new PatronWizardView.Choice { Title = selected, Description = option.description, Selected = _aspiration == selected,
                        Select = () => { _aspiration = selected; Render(); } });
                }
            }
            else if (_step == 5)
            {
                title = "Характер";
                foreach (var option in _creator.Personalities)
                {
                    var selected = option.name;
                    choices.Add(new PatronWizardView.Choice { Title = selected, Description = option.description, Selected = _personality == selected,
                        Select = () => { _personality = selected; Render(); } });
                }
            }
            else title = "Имя и сохранение";

            var baseInfluence = _template == null ? string.Empty : string.Join(", ",
                (_template.influence ?? new Dictionary<string, int>()).Select(pair => $"{pair.Key}: {pair.Value}"));
            var influence = _template == null ? string.Empty : string.Join(", ",
                _effects.CalculateInfluence(_template, _weakness, _possibility).Select(pair => $"{pair.Key}: {pair.Value}"));
            var selectedVariants = _possibilityVariants.Count > 0 ? $" ({string.Join(", ", _possibilityVariants)})" : string.Empty;
            var summary = $"Служба: {_faction ?? "—"}  Должность: {_template?.responsibilities ?? "—"}\n" +
                          $"Возможность: {_possibility ?? "—"}{selectedVariants}  Слабость: {_weakness ?? "—"}\n" +
                          $"Стремление: {_aspiration ?? "—"}  Характер: {_personality ?? "—"}\n" +
                          $"Влияние: {baseInfluence} → {influence}";
            _view.Render(title, summary, choices, _step == 6, _name, _message, _step > 0, CompleteStep(), _step == 2);
        }
    }
}
