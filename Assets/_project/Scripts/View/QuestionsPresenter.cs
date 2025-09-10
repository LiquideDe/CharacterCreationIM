using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreation
{
    public class QuestionsPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private QuestionsView _view;
        private readonly List<IDisposable> _subscriptions = new();
        private int _currentId = 0;
        private Character _character;

        public QuestionsPresenter(AudioManager audioManager, QuestionsView view)
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

            _subscriptions.Add(_view.OnSkipButtonClick.Subscribe(_ => { SkipQuestions(); }));
        }

        private void SkipQuestions()
        {
            _audioManager.PlayClick();
            _view.HideAndDestroyToLeft();            
            _nextClicked?.OnNext(_character);
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        private void NextShow() 
        {
            _audioManager.PlayClick();
            _currentId++;
            switch (_currentId)
            {
                case 1:
                    _view.SetText("КАКОВА ВАША СЕМЬЯ?", "Семья – это роскошь, которой не могут похвастаться многие жители Империума. Его ведомства не считают семью ценностью – скорее они видят в ней источник отвлечения или даже двойной верности. В зависимости от вашего происхождения и службы, вы могли иметь нечто вроде семьи, но возможно, всё, что вы получили от неё, ограничивается подготовкой и трудом. Немногочисленные счастливцы обретают семью в своих товарищах по оружию. Или вы – одиночка, для которого существует лишь долг? "); break;

                case 2:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"КАКОВА ВАША СЕМЬЯ? \n {_view.InputField.text}\n";
                    _view.SetText("КАКОВА ВАША ВЕРА?", "Конечно, вы веруете в Императора и Империум, но как это проявляется? Неважно, следуете ли вы традиционным учениям Имперского культа или приемлемым отличиям культа Машины – что вера значит для вас? Вы испытываете праведную ненависть и желаете истребить всех, кто противостоит Его воле? Или ваша набожность – это сияющий щит и путеводный свет? Быть может, вера толкает вас к самопожертвованию и зовёт сражаться без страха, дабы стать мучеником? Или вы чувствуете тягу проповедовать другим или записывать величайшие Его чудеса? "); break;

                case 3:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"КАКОВА ВАША ВЕРА? \n {_view.InputField.text}\n";
                    _view.SetText("КТО ВАШ БЛИЖАЙШИЙ СОРАТНИК?", "Подобно семье, друзья в сорок первом тысячелетии – роскошь, но в опасной галактике выживание зависит от того, есть у вас верные товарищи или нет. Быть может, кто-то из коллег – ваш самый доверенный друг? Быть может, вы завели приятелей в иной службе? Можете ли вы обратиться к таким соратникам в час нужды и имеет ли их дружба границы? "); break;

                case 4:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"КТО ВАШ БЛИЖАЙШИЙ СОРАТНИК? \n {_view.InputField.text}\n";
                    _view.SetText("ЧЕГО ВЫ БОИТЕСЬ?", "В сорок первом тысячелетии есть, чего страшиться. Имперских подданных учат боятся и ненавидеть всё, чего они не понимают, но лишь немногие умудрённые знают, что такое истинный страх. Чего ваш герой боится больше всего? Этот страх толкает его уничтожить его источник или держаться от него подальше любой ценой? "); break;

                case 5:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ЧЕГО ВЫ БОИТЕСЬ? \n {_view.InputField.text}\n";
                    _view.SetText("ЧЕГО ВЫ ЖЕЛАЕТЕ?", "В Империуме подданных учат не желать ничего, кроме жизни в служении Императору. Для простых людей истинное честолюбие всё равно бесполезно. Но вы и ваша жизнь – не простые. Вы стремитесь в светской власти или духовному спасению? У вас есть чёткая и достижимая цель или вы сражаетесь за высокие идеалы? Чем вы готовы пожертвовать ради своих амбиций? "); break;

                case 6:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ЧЕГО ВЫ ЖЕЛАЕТЕ?? \n {_view.InputField.text}\n";
                    _view.SetText("КОМУ ВЫ ВЕРНЫ?", "Верность в Империуме ценят высоко – верность Императору, столпам его царства и своим господам. Вы преданны своему покровителю или видите в нём лишь средство достижения цели? Вы верны своим товарищам или они для вас – просто расходный материал? Вы исполняете свой долг из преданности или чистого прагматизма? "); break;

                case 7:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"КОМУ ВЫ ВЕРНЫ? \n {_view.InputField.text}\n";
                    _view.SetText("ПОЧЕМУ ПОКРОВИТЕЛЬ ВЫБРАЛ ВАС?", "Господин вырвал вас из уготованной вам жизни и даровал новую, и он сделал это не без причин. Почему среди миллиардов махарийцев он выбрал вас? Вы наделены особыми умениями, талантами или менее зримыми качествами? Чего он ожидает от вас? Был ли у вас выбор, или покровитель завербовал вас против вашей воли? "); break;

                case 8:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ПОЧЕМУ ПОКРОВИТЕЛЬ ВЫБРАЛ ВАС? \n {_view.InputField.text}\n";
                    _view.SetText("ЧТО ВЫ ДУМАЕТЕ О СВОЁМ ПОКРОВИТЕЛЕ?", "Прошлый опыт и первая встреча с господином явно сложили у вас мнение о нём. Это таинственный благодетель или почти что друг? Вы боитесь его гнева или уважаете его целеустремлённость? Что вы действительно знаете об этом человеке? "); break;

                case 9:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ЧТО ВЫ ДУМАЕТЕ О СВОЁМ ПОКРОВИТЕЛЕ? \n {_view.InputField.text}\n";
                    _view.SetText("ЧТО ВЫ НЕНАВИДИТЕ?", "Ненависть толкает вперёд слуг Империума и его врагов. Экклезиархия учит людей ненавидеть еретиков, мутантов и чужаков. Верите ли вы в правильность таких учений или считаете слепую ненависть неприятной? Есть ли у вас личный враг, которого вы ненавидите? Быть может, вы затаили злобу на какую-то имперскую службу, что дурно обошлась с вами? "); break;

                case 10:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ЧТО ВЫ НЕНАВИДИТЕ? \n {_view.InputField.text}\n";
                    _view.SetText("ЧЕМ ВЫ ГОТОВЫ ПОЖЕРТВОВАТЬ?", "Ваше служение господину чревато опасностями. Они не всегда очевидны, а порой угрожают больше, чем вашей жизни. Как далеко вы готовы зайти, исполняя свой долг, и какую цену готовы заплатить? Империум учит своих слуг жертвовать собой при исполнении долга, но действительно ли вы верите в это? А как насчёт жизней ваших земляков? Готовы ли вы замарать свою репутацию и забыть о чести, чтобы достичь цели? А как насчёт вашей души? "); break;

                case 11:
                    if (_view.InputField.text.Length > 0) _character.TenQuestions.Value = $"ЧЕМ ВЫ ГОТОВЫ ПОЖЕРТВОВАТЬ? \n {_view.InputField.text}\n";
                    _view.HideAndDestroyToLeft();
                    _nextClicked?.OnNext(_character); break;
            }
        }
    }
}

