using R3;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class AddExperiencePresenter : ICharacterPresenter, IInitializable
    {
        private AddExperienceView _view;
        private readonly AudioManager _audioManager;
        private Character _character;

        private CompositeDisposable _cd = new CompositeDisposable();

        private Subject<Character> _characterSubject = new Subject<Character>();
        public Observable<Character> NextClicked => _characterSubject;

        public AddExperiencePresenter(AddExperienceView view, AudioManager audioManager)
        {
            _view = view;
            _audioManager = audioManager;
        }

        public void Dispose()
        {
            _cd?.Dispose();
            _characterSubject?.Dispose();
        }

        public void Initialize()
        {
            _view.GetExperience.Subscribe(CharacterGetExperience).AddTo(_cd);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        private void CharacterGetExperience(int experience)
        {
            _audioManager.PlayConfirm();
            _character.Experience.Value.experiencePoints += experience;
            _characterSubject.OnNext(_character);
            _view.HideAndDestroyToLeft();
        }
    }
}

