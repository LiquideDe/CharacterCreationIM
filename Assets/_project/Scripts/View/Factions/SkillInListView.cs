using R3;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CharacterCreation.Background
{
    public class SkillInListView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private Button _buttonPlus;
        [SerializeField] private Button _buttonMinus;
        [SerializeField] private InfoButtonView _infoButtonView;

        public Observable<Unit> OnPlusButtonClick => _buttonPlus.OnClickAsObservable();
        public Observable<Unit> OnMinusButtonClick => _buttonMinus.OnClickAsObservable();

        private int _level = 0;
        public string NameSkill { get; private set; }

        public int Level => _level;

        private CompositeDisposable _disposables = new CompositeDisposable();

        void Start()
        {
            _disposables = new CompositeDisposable();
            _buttonPlus.OnClickAsObservable().Subscribe(_ =>
            {
                _textName.text = $"{NameSkill} - {_level}";
            }).AddTo(_disposables);

            _buttonMinus.OnClickAsObservable().Subscribe(_ =>
            {
                _textName.text = $"{NameSkill} - {_level}";
            }).AddTo(_disposables);
        }

        public void SetName(string name)
        {
            _infoButtonView.SetSkill(name);
            NameSkill = name;
            _textName.text = $"{name} - {_level}";
        }

        public void PlusLevel() => _level++;
        public void MinusLevel() => _level--;

        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

    }
}

