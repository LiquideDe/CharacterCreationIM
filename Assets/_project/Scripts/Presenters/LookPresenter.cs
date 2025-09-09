using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CharacterCreation
{
    public class LookPresenter : ICharacterPresenter
    {
        private readonly Subject<Character> _nextClicked = new();
        public Observable<Character> NextClicked => _nextClicked;
        private readonly AudioManager _audioManager;
        private LookView _view;
        private readonly List<IDisposable> _subscriptions = new();
        private int _currentId = 0;
        private Character _character;
        private List<int[]> _ageGroup = new List<int[]>()
        {
            new int[2]{17,1},
            new int[2]{17, 2},
            new int[2]{50, 5},
            new int[2]{100, 5}
        };

        private List<string> _eyes = new List<string>()
        {
            "Скорбные",
            "Жёсткие",
            "Старые",
"Сосредоточенный взгляд",
            "Безумные",
"Яркие",
"Злобные",
"Полные надежд",
"Подозрительные",
"Свирепые",
"Тёплые",
"Мёртвые",
"Горящие",
"Осуждающие",
"Гипнотизирующие",
"Несчастные",
"Любопытные",
"Холодные",
"Загнанный взор",
"Расчётливые"
        };

        private Dictionary<int, string> _ageGroupNames = new Dictionary<int, string>()
        {
            {0, "Новобранец" },
            {1, "Ветеран" },
            {2, "Омоложенный" },
            {3, "Аугментированный" }
        };

        private List<string> _hairsColor = new List<string>()
        {
            "Серые или седые",
"Чёрные",
"Каштановые",
"Рыжие",
"Белые",
"Зелёные",
"Синие",
"Фиолетовые",
"Отсутствуют",
"Многоцветные"

        };

        private List<string> _hairsStyle = new List<string>()
        {
            "Непричёсанные",
"Косы",
"Ирокез",
"Пересаженные от лошади или др. животного",
"Длинные волосы",
"Коротко обрезанные",
"Кудри",
"Пышная сложная причёска",
"Металлические провода и шипы",
"Заклёпки, проволока и металлические пластины"

        };

        public readonly D100Lookup<string> Omens = new D100Lookup<string>(new[]
    {
        (1, 3,  "У вас на лбу набита татуировка в виде аквилы"),
        (4, 10, "От вас попахивает трупной мукой."),
        (11,11, "В вашей груди засел неразорвавшийся болтерный снаряд. Вы знаете, что когда-нибудь он разорвётся."),
        (12,15, "У вас есть выцветшая электуировка, окоторой вы хотели бы забыть."),
        (16,26, "Вашу кожу марают оспины или старые следы другой кожной болезни."),
        (27,27, "Ваши уши были заменены на большие, но рабочие аугметические протезы."),
        (28,28, "Ваши зубы сделаны из металла."),
        (29,30, "У вас татуированы роговицы глаз."),
        (31,40, "У вас страшные шрамы – возможно, это следы боя или несчастного случая."),
        (41,42, "Ваши зубы бритвенной остроты."),
        (43,46, "Часть костей вашего черепа заменена на металлические пластины."),
        (47,47, "У вас есть необъяснимый шрам в виде аквилы."),
        (48,49, "Ваши веки покрыты тату-надписями на высоком готическом с обеих сторон."),
        (50,50, "У вас есть родимое пятно в виде черепа. Определите, где оно."),
        (51,54, "Культ или секта оставила на вашем теле клеймо."),
        (55,57, "У вас совершенно белые зрачки."),
        (58,65, "У вас ужасно бледная кожа."),
        (66,68, "У вас огромные мышцы, выросшие благодаря жизни, полной труда или путём искусственной пересадки."),
        (69,72, "В вашей голове есть множество разъёмов для подключения."),
        (73,78, "Вы покрываете себя кусочками пергамента с письменами и печатями чистоты, без которых вам неуютно."),
        (79,99, "Вы совершенно непримечательны – ещё одна легко заменимая шестерёнка в бесконечной машине Империума."),
        (100,100, "")
    }, name: "Приметы");

        public LookPresenter(AudioManager audioManager, LookView view)
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

            _subscriptions.Add(
                _view.OnRandomButtonClick.Subscribe(_ =>
                {
                    NextShow(false);
                    _audioManager.PlayClick();                    
                })
            );
            //NextShow();
        }

        public void SetCharacter(Character character)
        => _character = character;
        
        private void NextShow(bool fromManual = true)
        {
            _currentId++;
            switch (_currentId)
            {
                case 1:
                    _view.SetNameCategory("Ваш Возраст", "Впишите ваш возраст или выберете случайное");
                    break;

                    //Тип глаз
                case 2:
                    if (fromManual)
                    {
                        if (_view.InputField.Length != 0)
                        {
                            int.TryParse(_view.InputField, out int age);
                            if (age == 0)
                            {
                                _audioManager.PlayError();
                                _currentId--;                                
                                break;
                            }
                            else
                            {
                                _audioManager.PlayConfirm();
                                _character.Age.Value = age;
                            }
                        }
                    }
                    else
                    {
                        _audioManager.PlayConfirm();
                        var rand = new System.Random();
                        int group = RandomId(4);
                        int age = _ageGroup[group][0];
                        for (int i = 0; i < _ageGroup[group][1]; i++)
                            age += rand.Next(1, 11);
                        _character.Age.Value = age;
                    }

                    _view.SetNameCategory("Ваши Глаза", "Впишите ваш тип глаз или выберете случайные, или выберете из списка");
                    _view.SetList(_eyes);
                    break;

                    //Цвет волос
                case 3:
                    if (fromManual)
                    {
                        string eyes;
                        if (GetName(out eyes))                        
                            _character.Eyes.Value = eyes;
                        else
                            break;              
                    }
                    else                    
                        _character.Eyes.Value = _eyes[RandomId(_eyes.Count)];                    

                    _audioManager.PlayConfirm();
                    _view.SetNameCategory("Ваши Волосы", "Впишите ваш цвет волос или выберете случайные, или выберете из списка");
                    _view.SetList(_hairsColor);
                    break;

                //Если выпало 81 и выше надо брать 2 цвета до лысых
                case 4:
                    if (fromManual) 
                    {
                        string hairColor;
                        if(GetName(out hairColor))
                        {
                            if(hairColor != null && string.Compare(hairColor, "Многоцветные") == 0)
                            {
                                _character.HairColor.Value = $"{_hairsColor[RandomId(_hairsColor.Count - 2)]} и {_hairsColor[RandomId(_hairsColor.Count - 2)]}";
                            }
                            else
                                _character.HairColor.Value = hairColor;
                        }
                        else                        
                            break;                        
                    }
                    else
                    {                        
                        var hair = _hairsColor[RandomId(_hairsColor.Count)];
                        if(string.Compare(hair, "Многоцветные") == 0)
                        {
                            int color1 = RandomId(_hairsColor.Count - 2);
                            int color2 = RandomId(_hairsColor.Count - 2);
                            _character.HairColor.Value = $"{_hairsColor[color1]} и {_hairsColor[color2]}";
                        }                            
                        else
                            _character.HairColor.Value= hair;
                    }

                    _audioManager.PlayConfirm();
                    _view.SetNameCategory("Ваши Волосы", "Впишите ваш стиль волос или выберете случайные, или выберете из списка");
                    _view.SetList(_hairsStyle);
                    break;

                case 5: //тип прически
                    if (fromManual)
                    {
                        string hairStyle;
                        if(GetName(out hairStyle))
                            _character.HairStyle.Value = hairStyle;
                        else                        
                            break;                        
                    }
                    else                    
                        _character.HairStyle.Value = _hairsStyle[RandomId(_hairsStyle.Count)];
                    

                    _audioManager.PlayConfirm();
                    _view.SetNameCategory("Ваши Особые Приметы", "Впишите ваши особые приметы или выберете случайные, или выберете из списка");
                    List<string> allOmens = Omens.GetAllUniqueValues();
                    _view.SetList(allOmens);
                    break;

                case 6:
                    if (fromManual) 
                    {
                        string omen;
                        if(GetName(out omen))
                            _character.Omen.Value = omen;
                        else                        
                            break;
                    }
                    else
                        _character.Omen.Value = Omens.Roll();
                    _audioManager.PlayConfirm();
                    _view.HideAndDestroyToLeft();
                    _nextClicked?.OnNext(_character);
                    break;
            }
        }

        private bool GetName(out string name)
        {
            if (_view.InputField.Length != 0)
                name = _view.InputField;
            else if (_view.GetChosenLook().Length != 0)
                name = _view.GetChosenLook();
            else
            {
                _audioManager.PlayError();
                _currentId--;
                name = null;
                return false;
            }

            return true;
        }

        private int RandomId(int endId)
        {
            var rand = new System.Random();
            return rand.Next(0, endId);
        }
    }

    public sealed class D100Lookup<T>
    {
        private readonly T[] _table = new T[101]; // индексы 1..100

        public D100Lookup(IEnumerable<(int min, int max, T value)> ranges, string name = null)
        {
            foreach (var (a, b, v) in ranges ?? Array.Empty<(int, int, T)>())
            {
                if (v == null) continue;
                int min = Math.Max(1, Math.Min(a, b));
                int max = Math.Min(100, Math.Max(a, b));
                if (min > max) continue;

                for (int r = min; r <= max; r++)
                {
                    if (!EqualityComparer<T>.Default.Equals(_table[r], default))
                        Debug.LogError($"[D100] Перекрытие в {name ?? "table"}: roll={r} уже занят.");
                    _table[r] = v;
                }
            }

            // Подсветим первую «дыру» (опционально)
            for (int r = 1; r <= 100; r++)
            {
                if (EqualityComparer<T>.Default.Equals(_table[r], default))
                {
                    Debug.LogWarning($"[D100] Дыра в {name ?? "table"}: roll={r} не покрыт.");
                    break;
                }
            }
        }

        public T Get(int roll) => (roll >= 1 && roll <= 100) ? _table[roll] : default;

        public T Roll(System.Random rng = null)
        {
            rng ??= new System.Random();
            return _table[rng.Next(1, 101)];
        }

        public List<T> GetAllUniqueValues()
        {
            return _table
                .Where(x => !EqualityComparer<T>.Default.Equals(x, default))
                .Distinct()
                .ToList();
        }
    }
}

