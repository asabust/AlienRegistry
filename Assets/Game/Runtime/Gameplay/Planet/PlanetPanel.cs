using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Game.Runtime.Core;
using Game.Runtime.Data;
using System;

public class PlanetPanel : UIPanel
{
    public Button closeButton;

    [Header("Icon")] public Image icon1;
    public Image icon2;
    public Image icon3;

    [Header("Card")] public Transform cardBorder;
    public LocalizedText nameText;
    public LocalizedText descText;
    public LocalizedText requireText;
    public Button dispatchButton;

    private List<PlanetData> _planets = new();
    private List<Image> _icons = new();
    private int _selectedIndex = -1;

    private RectTransform _rect;
    private float _screenH;

    public override void OnInit()
    {
        _rect = GetComponent<RectTransform>();
        _screenH = GetScreenHeight();

        _icons.Add(icon1);
        _icons.Add(icon2);
        _icons.Add(icon3);

        // 绑定点击事件
        for (int i = 0; i < _icons.Count; i++)
        {
            int index = i;
            var btn = _icons[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnClickPlanet(index));
            }
        }

        // Dispatch
        dispatchButton.onClick.AddListener(OnClickDispatch);
        closeButton.onClick.AddListener(OnClickClose);

        // 初始隐藏卡片
        cardBorder.localScale = Vector3.zero;
    }

    private void OnClickClose()
    {
        CloseWithAnim(() =>
        {
            UIManager.Instance.Close<PlanetPanel>();
        });
    }

    private void OnClickDispatch()
    {
        if (_selectedIndex < 0) return;

        int planetId = _planets[_selectedIndex].id;
        InspectionManager.Instance.OnDispatchCallback(planetId);
        OnClickClose();
    }

    public override void OnOpen(object data = null)
    {
        if (data == null)
        {
            Debug.LogError("未传入角色ID");
            return;
        }

        int characterId = (int)data;

        if (!DataLoader.Instance.gameData.characters.TryGetValue(characterId, out CharacterData character))
        {
            Debug.LogError("没找到角色数据 CharacterID: " + characterId);
        }

        PlayOpenAnim();
        _planets.Clear();

        for (int i = 0; i < character?.planetOption.Count; i++)
        {
            int planetId = character.planetOption[i];
            if (DataLoader.Instance.gameData.planets.TryGetValue(planetId, out PlanetData planet))
            {
                _planets.Add(planet);
            }
        }

        RefreshIcons();
        ResetState();
    }

    private void RefreshIcons()
    {
        for (int i = 0; i < _icons.Count; i++)
        {
            if (i < _planets.Count)
            {
                var sprite = Resources.Load<Sprite>("PlanetIcons/" + _planets[i].iconName);
                _icons[i].sprite = sprite;
                _icons[i].color = Color.white;
                _icons[i].gameObject.SetActive(true);
                _icons[i].SetNativeSize();
            }
            else
            {
                _icons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ResetState()
    {
        _selectedIndex = -1;

        foreach (var icon in _icons)
        {
            icon.color = Color.white;
            icon.transform.localScale = Vector3.one;
        }

        cardBorder.localScale = Vector3.zero;

        nameText.SetText(string.Empty);
        descText.SetText(string.Empty);
        requireText.SetText(string.Empty);
    }

    private void OnClickPlanet(int index)
    {
        if (index >= _planets.Count) return;

        _selectedIndex = index;

        UpdateIconState();
        ShowPlanetInfo(index);
        PlayCardAnim();
    }

    private void UpdateIconState()
    {
        for (int i = 0; i < _icons.Count; i++)
        {
            if (i == _selectedIndex)
            {
                _icons[i].color = Color.white;
                _icons[i].transform.localScale = Vector3.one;
            }
            else
            {
                var c = Color.white;
                c.a = 0.4f; // 半透明
                _icons[i].color = c;
                _icons[i].transform.localScale = Vector3.one * 0.6f;
            }
        }
    }

    private void ShowPlanetInfo(int index)
    {
        var data = _planets[index];

        nameText.SetLocalizationKey(data.name);
        descText.SetLocalizationKey(data.description);
        requireText.SetLocalizationKey(data.planetRequire);
    }

    private void PlayCardAnim()
    {
        cardBorder.DOKill();

        cardBorder.localScale = Vector3.zero;
        cardBorder.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack);
    }

    private void PlayOpenAnim()
    {
        _rect.DOKill();

        _rect.anchoredPosition = new Vector2(0, -_screenH);
        _rect.localScale = new Vector3(0.96f, 0.96f, 1f);

        DOTween.Sequence()
            .Append(_rect.DOAnchorPosY(0, 0.4f).SetEase(Ease.OutCubic))
            .Join(_rect.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }

    private void CloseWithAnim(Action onClosed = null)
    {
        _rect.DOKill();

        _rect.localScale = Vector3.one;

        DOTween.Sequence()
            .Append(_rect.DOAnchorPosY(-_screenH, 0.3f).SetEase(Ease.InCubic))
            .Join(_rect.DOScale(0.96f, 0.3f))
            .OnComplete(() =>
            {
                onClosed?.Invoke();
            });
    }

    private float GetScreenHeight()
    {
        var canvas = GetComponentInParent<Canvas>();
        var rect = canvas.GetComponent<RectTransform>();
        return rect.rect.height;
    }
}