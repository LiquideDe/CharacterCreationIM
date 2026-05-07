using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CharacterCreation
{
    public class AddExperienceView : ViewBase
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private TMP_InputField _inputField;

        private CompositeDisposable _cd = new CompositeDisposable();
        private Subject<int> _getExperience = new Subject<int>();
        public Observable<int> GetExperience => _getExperience;

        private void Start()
        {
            Show();
        }

        protected override void Awake()
        {
            base.Awake();
            _nextButton.OnClickAsObservable().Subscribe(_ => SetExperience()).AddTo(_cd);
        }

        private void OnDestroy()
        {
            _cd.Clear();
        }

        private void SetExperience()
        {
            int.TryParse(_inputField.text, out int _exp);
            if (_exp > 0)
            {
                _getExperience.OnNext(_exp);
            }

            else
                _audio.PlayError();
        }
    }
}

