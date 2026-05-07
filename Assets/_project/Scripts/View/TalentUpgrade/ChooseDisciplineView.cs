using CharacterCreation.Background;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace CharacterCreation
{
    public class ChooseDisciplineView : ViewBase
    {
        [SerializeField] private Transform _content;
        [SerializeField] private ToggleGroupForTalents _toggleGroupForTalents;
        [SerializeField] private Button _closeButton;

        [Inject] private IFactory<TalentInListView> _factory;
        private List<TalentInListView> _disciplinesList = new List<TalentInListView>();
        private Subject<string> _nameDisciplineChosen = new Subject<string>();

        public Observable<string> NameDisciplineChosen => _nameDisciplineChosen;

        private void OnEnable()
        {
            _closeButton.OnClickAsObservable().Take(1).Subscribe(_ => SetAccessToDiscipline());
            Show();
        }

        public void ShowDisciplines(List<string> strings)
        {
            foreach (var item in strings)
            {
                TalentInListView talentInListView = _factory.Create();
                talentInListView.transform.SetParent(_content, false);

                talentInListView.AddTalent(item);
                _toggleGroupForTalents.AddToggle(talentInListView.Toggle);
                _disciplinesList.Add(talentInListView);
            }
        }

        private void SetAccessToDiscipline()
        {
            if (_toggleGroupForTalents.SelectedCount() == 0)
                return;

            foreach (var item in _disciplinesList)
            {
                if (item.IsSelected)
                {
                    _nameDisciplineChosen.OnNext(item.Talents.FirstOrDefault());
                    Clear();
                    HideToRight();
                    break;
                }
            }
        }

        private void Clear()
        {
            foreach (var item in _disciplinesList)
            {
                Destroy(item.gameObject);
            }
            _disciplinesList.Clear();
        }
       
    }
}

