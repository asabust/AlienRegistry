using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GlitterView : MonoBehaviour
{
    [SerializeField] private Image glitterPortrait;
    [SerializeField] private LocalizedText glitterDesc;
    public Button closeButton;
    public float duration = 0.3f;

    private Tween _currentTween;

    private RectTransform _rect;
    private RectTransform _portraitRect;
    private GlitterData _currentGlitterData;
    private bool _isOpen;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        if (glitterPortrait == null)
        {
            glitterPortrait = transform.Find("Mask/GlitterPortrait").GetComponent<Image>();
        }

        if (glitterDesc == null)
        {
            glitterDesc = transform.Find("GlitterDesc").GetComponent<LocalizedText>();
        }

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnClickClose);
    }

    private void Start()
    {
        _rect.anchoredPosition = new Vector2(0, _rect.anchoredPosition.y);
        _isOpen = false;
    }

    /// <summary>
    ///     点击某个点
    /// </summary>
    public void Show(Sprite sprite, GlitterData data)
    {
        //如果点的是同一个，直接无视
        if (_currentGlitterData == data)
        {
            return;
        }

        _currentGlitterData = data;

        // 如果当前已经打开，先收再开
        if (_isOpen)
        {
            Hide(() =>
            {
                SetContent(sprite, data);
                PlayShow();
            });
        }
        else
        {
            SetContent(sprite, data);
            PlayShow();
        }
    }

    /// <summary>
    ///     点击关闭按钮
    /// </summary>
    public void OnClickClose()
    {
        AudioManager.Instance.PlaySfx("quit");
        _isOpen = false;
        _currentGlitterData = null;
        Hide();
    }

    private void SetContent(Sprite sprite, GlitterData data)
    {
        glitterPortrait.sprite = sprite;
        glitterPortrait.rectTransform.anchoredPosition = data.appearancePosition;
        glitterPortrait.rectTransform.localScale = Vector2.one * data.scale;
        glitterDesc.SetLocalizationKey(data.glitterDescription);
    }

    private void PlayShow()
    {
        _currentTween?.Kill();

        _rect.anchoredPosition = new Vector2(0, _rect.anchoredPosition.y);

        _currentTween = _rect.DOAnchorPosX(-400f, duration)
            .SetEase(Ease.OutCubic);

        closeButton.gameObject.SetActive(true);
        _isOpen = true;
    }


    private void Hide(Action onComplete = null)
    {
        _currentTween?.Kill();

        _currentTween = _rect.DOAnchorPosX(0f, duration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                _isOpen = false;
                onComplete?.Invoke();
            });
    }
}