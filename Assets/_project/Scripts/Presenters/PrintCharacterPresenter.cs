using R3;
using System;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class PrintCharacterPresenter
    {
        [Inject] private IFactory<FirstPage> _firstFactory;
        [Inject] private IFactory<SecondPage> _secondFactory;
        private Subject<Unit> _workIsFinished = new Subject<Unit>();
        public Observable<Unit> WorkIsFinished => _workIsFinished;
        private Character _character;
        private CompositeDisposable _cd = new CompositeDisposable();

        public void PrintCharacter(Character character)
        {
            _character = character;
            try { TakeScreenShotFirst(); }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
            
        }

        private void TakeScreenShotFirst()
        {
            var view = _firstFactory.Create();
            view.WorkIsFinished.Subscribe(_ => TakesScreenShotSecond()).AddTo(_cd);
            view.SetCharacter(_character);
        }

        private void TakesScreenShotSecond()
        {
            var view = _secondFactory.Create();
            view.WorkIsFinished.Subscribe(_ => EndSavePicture()).AddTo(_cd);
            try { view.SetCharacter(_character); }
            catch(Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private void EndSavePicture()
        {
            //_character.Release();
            _workIsFinished.OnNext(Unit.Default);
        }
    }
}

