using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Zenject;

namespace CharacterCreation
{
    public class ViewBase : MonoBehaviour
    {
        [Header("Анимация")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float slideOffset = 2000f;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;

        [Inject] protected AudioManager _audio = null;
        

        protected virtual void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        public virtual void Show()
        {
            Sequence seq = DOTween.Sequence();

            if (canvasGroup != null || rectTransform != null)
            {
                Vector2 targetBodyPosition = rectTransform.anchoredPosition;
                Vector2 startShift = new Vector2(Screen.width / 2, targetBodyPosition.y);
                _audio.PlayFadeIn();
                seq = DOTween.Sequence();

                seq.Append(canvasGroup.DOFade(1, 1f).From(0)).Join(rectTransform.DOAnchorPos(targetBodyPosition, 1f).From(startShift));
            }
        }

        public virtual void ShowFromLeft()
        {
            Sequence seq = DOTween.Sequence();
            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 startShift = new Vector2(-Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeIn();
            seq = DOTween.Sequence();

            seq.Append(canvasGroup.DOFade(1, 1f).From(0)).Join(rectTransform.DOAnchorPos(targetBodyPosition, 1f).From(startShift));
        }

        public virtual void HideAndDestroyToLeft()
        {
            Sequence seq = DOTween.Sequence();

            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(-Screen.width / 2, targetBodyPosition.y);
            Vector2 startShift = new Vector2(Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeOut();

            seq.Append(canvasGroup.DOFade(0, 1f).From(1)).Join(rectTransform.DOAnchorPos(finishShift, 1f).From(targetBodyPosition)).
                OnComplete(() => Destroy(gameObject));
        }

        public virtual void HideAndDestroyToLeft(Action onComplete)
        {
            Sequence seq = DOTween.Sequence();

            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(-Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeOut();

            seq.Append(canvasGroup.DOFade(0, 1f).From(1)).Join(rectTransform.DOAnchorPos(finishShift, 1f).From(targetBodyPosition)).
                OnComplete(() =>
                {
                    onComplete?.Invoke();
                    Destroy(gameObject);
                });
        }

        public virtual void HideToRight()
        {
            Sequence seq = DOTween.Sequence();

            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(-Screen.width / 2, targetBodyPosition.y);
            Vector2 startShift = new Vector2(Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeOut();

            seq.Append(canvasGroup.DOFade(0, 1f).From(1)).Join(rectTransform.DOAnchorPos(finishShift, 1f).From(targetBodyPosition)).
                OnComplete(() => gameObject.SetActive(false));
        }

        public void HideAndShow()
        {
            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(-Screen.width / 2f, targetBodyPosition.y); // уходит влево
            Vector2 startShift = new Vector2(Screen.width / 2f, targetBodyPosition.y);  // входит справа

            Sequence seq = DOTween.Sequence();

            // шаг 1 — уходим влево
            seq.AppendCallback(() => _audio.PlayFadeOut());
            seq.Append(canvasGroup.DOFade(0, 0.6f).From(1f))
               .Join(rectTransform.DOAnchorPos(finishShift, 0.6f).From(targetBodyPosition));

            // шаг 2 — по завершении скрыть, сбросить на правый край
            seq.AppendCallback(() =>
            {
                rectTransform.anchoredPosition = startShift;
                canvasGroup.alpha = 0;
            });

            // шаг 3 — вход справа
            seq.AppendCallback(() => _audio.PlayFadeIn());
            seq.Append(canvasGroup.DOFade(1, 0.6f).From(0f))
               .Join(rectTransform.DOAnchorPos(targetBodyPosition, 0.6f).From(startShift));
        }

        public virtual void HideAndDestroyToRight()
        {
            Sequence seq = DOTween.Sequence();
            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeOut();

            seq.Append(canvasGroup.DOFade(0, 1f).From(1)).Join(rectTransform.DOAnchorPos(finishShift, 1f).From(targetBodyPosition)).
                OnComplete(() => Destroy(gameObject));
        }

        public virtual void HideAndDestroyToRight(Action onComplete)
        {
            Sequence seq = DOTween.Sequence();
            Vector2 targetBodyPosition = rectTransform.anchoredPosition;
            Vector2 finishShift = new Vector2(Screen.width / 2, targetBodyPosition.y);
            _audio.PlayFadeOut();

            seq.Append(canvasGroup.DOFade(0, 1f).From(1)).Join(rectTransform.DOAnchorPos(finishShift, 1f).From(targetBodyPosition)).
                OnComplete(() =>
                {
                    onComplete?.Invoke();
                    Destroy(gameObject);
                });
        }
    }
}

