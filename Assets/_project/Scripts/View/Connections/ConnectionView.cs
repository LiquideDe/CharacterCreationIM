using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CharacterCreation
{
    public class ConnectionView : ViewBase
    {
        [Header("Panels")]
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private GameObject _newConnectionPanel;

        [Header("Buttons")]
        [SerializeField] private Button _buttonCloseStartPanel;
        [SerializeField] private Button _buttonCreateNewConnection;
        [SerializeField] private Button _buttonAddNewConnection;
        [SerializeField] private Button _buttonRandom;
        [SerializeField] private Button _buttonNext;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _textWithConnections;
        [SerializeField] private TMP_InputField _inputFieldName;
        [SerializeField] private TMP_InputField _inputFieldConnection;

        [Header("List")]
        [SerializeField] private Transform _content;
        [SerializeField] private PanelWithText _panelPrefab;

        [Inject] private AudioManager _audioManager;

        public string Text { get; set; }
        public Observable<Unit> OnNextButtonClick => _buttonNext.OnClickAsObservable();
        private List<string> _connections = new List<string>()
        {
            "{0} должен мне немало соляров.",
"Я должен {0} немало соляров.",
"Я слышал, как {0} говорил нечто богохульное – едва ли не еретическое.",
"{0} знает мою самую тёмную тайну.",
"{0} знает, что я спрятал дорогого мне человека от Чёрных кораблей.",
"{0} и я давным-давно сражались на разных сторонах.",
"Я начертал имя {0} на моём любимом оружии.",
"{0} и я выросли вместе.",
"{0} попросил меня предать кого-то близкого нам.",
"Мы с {0} пережили нечто, о чём не говорим.",
"Я уверен, что видел, как {0} погиб.",
"У меня были видения о {0}.",
"Я спас {0} жизнь.",
"{0} спас мою жизнь.",
"Однажды я видел, как {0} читает странную книгу. Это выглядело жутковато.",
"Я слышал, как {0} говорил с кем-то один на один и говорил пугающие вещи.",
"У меня есть кое-что, принадлежащее {0}, а у него – кое-что моё.",
"{0} поделился ужасной тайной.",
"У меня есть одна серебряная пуля. На ней выгравировано несколько слов на высоком готическом и имя {0}.",
"Я слышал, как {0} подвергал сомнению свои убеждения.",
"{0} владеет чем-то, что я хочу заполучить.",
"Я знаю, кто {0} на самом деле.",
"Я уже работал с {0} раньше.",
"{0} и я постоянно соперничаем.",
"Я подозреваю, что {0} – предатель."
        };

        private List<PanelWithText> _panels = new List<PanelWithText>();
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private void Start()
        {
            Text = string.Empty;
            Show();
            foreach (var item in _connections)
            {                
                var panel = Instantiate(_panelPrefab, _content);
                panel.Text.text = item;
                _panels.Add(panel);
                panel.OnSend.Subscribe(text => SetText(text)).AddTo(_compositeDisposable);
                panel.gameObject.SetActive(true);
            }

            _buttonCloseStartPanel.OnClickAsObservable().Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _startPanel.SetActive(false);
                _mainPanel.SetActive(true);

            }).AddTo(_compositeDisposable);

            _buttonCreateNewConnection.OnClickAsObservable().Subscribe(_ =>
            {
                _audioManager.PlayClick();
                _mainPanel.SetActive(false);
                _newConnectionPanel.SetActive(true);
            }).AddTo(_compositeDisposable);

            _buttonAddNewConnection.OnClickAsObservable().Subscribe(_ => AddNewConnection()).AddTo(_compositeDisposable);
            _buttonRandom.OnClickAsObservable().Subscribe(_ => RandomConnection()).AddTo(_compositeDisposable);
        }

        private void SetText(string text)
        {
            string name = _inputFieldName.text;
            string value = string.Format(text, name);
            _inputFieldConnection.text = value;
        }

        private void AddNewConnection()
        {
            _audioManager.PlayClick();
            _mainPanel.SetActive(true);
            _newConnectionPanel.SetActive(false);
            if (_inputFieldConnection.text.Length != 0)
            {
                if (Text.Length == 0)
                    Text = _inputFieldConnection.text;
                else
                    Text += $"\n _inputFieldConnection.text";
                _inputFieldConnection.text = string.Empty;
                _textWithConnections.text = Text;
            }
        }

        private void RandomConnection()
        {
            _audioManager.PlayClick();
            int value = Random.Range(0, _panels.Count);
            SetText(_panels[value].Text.text);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}

