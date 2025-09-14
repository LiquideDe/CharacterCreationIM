using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace CharacterCreation
{
    public class SetNamePresenter : ICharacterPresenter
    {
        
        private readonly Subject<Character> _nextClicked = new();
        private readonly Subject<Character> _prevClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        public Observable<Character> PrevClicked => _prevClicked;
        private readonly AudioManager _audioManager;
        private SetNameView _view;
        private readonly CompositeDisposable _cd = new CompositeDisposable();
        private Character _character;

        private List<string> _prophecies = new List<string>()
        {
            "Лучше умереть за Императора, чем жить для себя.",
"Доверяй своему рвению – нет оружия лучше его.",
"Неглубокий разум – это чистый разум.",
"Приветствовать ксеноса – значит приветствовать своё проклятие.",
"Долг превыше всего.",
"Мудрый учится на смертях других",
"Убей ксеноса прежде, чем он изречёт свою ложь.",
"Логика – враг веры.",
"Размышления порождают ересь.",
"Ересь порождает возмездие.",
"Разум без цели обречён скитаться во тьме.",
"О нас будут судить по нашим смертям.",
"Успех измеряется кровью – твоей или твоих врагов.",
"Император благословил нас даром нетерпимости.",
"Истинная вера слепа и оправдана.",
"Нет замены рвению.",
"Даже тот, у кого нет ничего, может отдать свою жизнь.",
"Кровь мучеников питает Империум.",
"Не считай счастливым никого из живых.",
"Открытый разум подобен крепости, чьи врата распахнуты и лишены стражи.",
"Кровью мучеников закаляется клинок Империума.",
"Один миг ереси портит жизнь служения.",
"Беспощадность – это доброта мудрого.",
"Идти на уступки – удел трусов.",
"Лишь со смертью кончается долг.",
"Смерть – это слуга праведников.",
"Победа будет жить вечно; поражение едва вспомнят.",
"Невиновность ничего не доказывает.",
"Проявить милосердие – значит выказать слабость.",
"Сожги Нечестивца в очищающем пламени.",
"Благословен разум, слишком слабый, чтобы сомневаться.",
"Разумом прикрываются предатели.",
"Слабость – единственное, чего стоит боятся.",
"Усомниться – значит выказать слабость.",
"Презрение – лучшая броня.",
"Отвернись от Императора на свой страх и риск.",
"Справедливость твоих деяний изменяется силой твоих убеждений.",
"Ненависть – величайший дар Императора человечеству.",
"Сомнениями вымощен путь к проклятию.",
"Насилие решает всё.",
"Страдание – это неумолимый наставник.",
"Император будет судить тебя не по твоим медалям, а по твоим шрамам.",
"Надежда – первый шаг на пути к разочарованию.",
"Простить – значит выказать слабость.",
"Не спрашивай, почему ты должен служить. Спрашивай, как.",
"Все твои усилия обернутся прахом, если они не служат Императору",
"На войне нет свидетелей – лишь солдаты и предатели.",
"Неси волю Императора как факел и разгоняй им тени.",
"Ересь вырастает из безделья.",
"Сожги еретика! Убей мутанта! Очисти нечестивца!"
        };

        public SetNamePresenter(AudioManager audioManager, SetNameView view)
        {
            _audioManager = audioManager;
            _view = view;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }

        public void Initialize()
        {
            _view.OnButtonNextClick.Subscribe(_ => { SetName(); }).AddTo(_cd);
        }

        public void Dispose()
        {
            _cd?.Dispose();
        }

        public void SetName()
        {
            if (_view.InputfieldName.text.Length > 0 && _view.InputfieldHeight.text.Length > 0 && _view.InputfieldWeight.text.Length > 0)
            {
                _character.Name.Value = _view.InputfieldName.text;
                int.TryParse(_view.InputfieldWeight.text, out int weight);
                int.TryParse(_view.InputfieldHeight.text, out int height);
                _character.Weight.Value = weight;
                _character.Height.Value = height;
                _audioManager.PlayConfirm();
                _view.HideAndDestroyToLeft();
                SetProphecy();
            }
            else
                _audioManager.PlayError();

        }

        private void SetProphecy()
        {
            var rand = new System.Random(); ;
            _character.Prophecy.Value = _prophecies[rand.Next(0, _prophecies.Count)];
            _nextClicked.OnNext(_character);
        }
    }
}

