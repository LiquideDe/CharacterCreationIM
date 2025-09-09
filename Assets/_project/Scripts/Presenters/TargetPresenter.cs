using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterCreation
{
    public class TargetPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private TargetView _view;
        private readonly List<IDisposable> _subscriptions = new();
        private int _currentId = 0;
        private Character _character;

        public TargetPresenter(AudioManager audioManager, TargetView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void Dispose()
        {
            _nextClicked.Dispose();
            foreach (var sub in _subscriptions)
                sub.Dispose();
            _subscriptions.Clear();
        }

        public void Initialize()
        {
            _subscriptions.Add(
                _view.OnNextButtonClick.Subscribe(_ =>
                {
                    NextShow();
                })
            );
        }

        private void NextShow()
        {
            _currentId++;
            string text = string.Empty;
            switch (_currentId)
            {
                case 1:
                    _audioManager.PlayClick();
                    _view.SetText("КРАТКОСРОЧНЫЕ ЦЕЛИ", "Краткосрочные цели отражают непосредственно стоящие перед вами задачи. Вы рассчитываете достичь их через несколько дней или недель. В целом, достижение краткосрочной цели должно занимать две-три игровые встречи. \n\nПример: отомстить местному бандиту, поймать вора, совершившего череду краж, достать цепной меч на замену сломанному.");
                    break;

                case 2:
                    if(CheckInput(out text))
                    {
                        _audioManager.PlayClick();
                        _character.ShortTarget.Value = text;
                        _view.SetText("ДОЛГОСРОЧНЫЕ ЦЕЛИ", "Долгосрочные цели – это ваши честолюбивые планы, над воплощением которым предстоит работать месяцы и годы или не достичь никогда. Возможно, такая цель служат вам не достижимой задачей, а главным стремлением всей жизни. \n\n<b>Пример<\b>: перехватить власть над крупным преступным картелем, погубить династию вольных торговцев и отнять их патент, уничтожить крупный культ Губительных сил. \r\n");
                    }
                    break;

                case 3:
                    if(CheckInput(out text))
                    {
                        _audioManager.PlayClick();
                        _character.LongTarget.Value = text;
                        _view.HideAndDestroyToLeft();
                        _nextClicked?.OnNext(_character);
                    }break;
            }
        }

        private bool CheckInput(out string text)
        {
            if (_view.InputField.text.Length == 0)
            {
                _currentId--;
                _audioManager.PlayError();
                text = string.Empty;
                return false;
            }

            text = _view.InputField.text;
            return true;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }
    }
}

