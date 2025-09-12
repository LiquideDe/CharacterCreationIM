using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class NewSpecializationPanel : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _textSkill;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _buttonClose;
        [SerializeField] private Button _buttonCreate;
        [Inject] private AudioManager _audioManager;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private string _nameSkill;

        private readonly Subject<SpecializationData> _nextClicked = new();
        public Observable<SpecializationData> NextClicked => _nextClicked;

        private void OnEnable()
        {
            _buttonCreate.OnClickAsObservable().Subscribe(_ => { CreateSpecialization(); }).AddTo(_compositeDisposable);
            _buttonClose.OnClickAsObservable().Subscribe(_ => { _audioManager.PlayCancel(); HideAndDestroyToRight(); }).AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable.Dispose();
        }

        private void CreateSpecialization()
        {
            if (_inputField.text.Length > 0)
            {
                _audioManager.PlayConfirm();
                var spec = new SpecializationData();
                spec.name = _inputField.text;
                spec.skill = _nameSkill;
                _nextClicked.OnNext(spec);
                HideAndDestroyToRight();
            }
            else
                _audioManager.PlayError();
                
        }

        private void Start()
        {
            Show();
        }

        public void SetSkill(string nameSkill)
        {
            _nameSkill = nameSkill;
            _textSkill.text = $"Новая специализация будет зависеть от {nameSkill}";
        }
    }
}

