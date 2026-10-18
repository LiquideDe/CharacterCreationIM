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
            var text = _inputField.text;
            int exp = 0;
            if (!string.IsNullOrWhiteSpace(text) && !int.TryParse(text, out exp))
            {
                _audio.PlayError();
                return;
            }

            if (exp < 0)
            {
                _audio.PlayError();
                return;
            }

            _getExperience.OnNext(exp);
        }
    }
}

