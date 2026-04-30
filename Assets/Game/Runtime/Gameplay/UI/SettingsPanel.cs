using Game.Runtime.Core;
using Game.Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventHandler = Game.Runtime.Core.EventHandler;

public class SettingsPanel : UIPanel
{
    [Header("Settings - Audio")] public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Settings - Buttons")] public Button btnBack;
    public Button btnQuit;
    public Button btnClose;
    public TMP_Dropdown languageDropdown;


    private void Awake()
    {
        languageDropdown.value = PlayerPrefs.GetInt("LanguageKey", 0);

        // Slider 监听
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        languageDropdown.onValueChanged.AddListener(OnSelectLanguage);

        // 按钮绑定
        btnBack.onClick.AddListener(OnBack);
        btnQuit.onClick.AddListener(OnQuit);
        btnClose.onClick.AddListener(Close);
    }

    public override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        if (data is bool inGame)
        {
            btnBack.gameObject.SetActive(inGame);
            btnQuit.gameObject.SetActive(inGame);
        }
    }


    #region 音量

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSfxChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    #endregion

    #region 语言

    private void OnSelectLanguage(int index)
    {
        Debug.Log($"OnSelectLanguage {index}");
        AudioManager.Instance.PlaySfx("click");
        PlayerPrefs.SetInt("LanguageKey", index);
        LocalizationManager.SetLanguage((Language)index);
    }

    #endregion

    #region 设置按钮

    void OnBack()
    {
        AudioManager.Instance.PlaySfx("quit");
        GameManager.Instance.GameTitle();
        Close();
    }

    void OnQuit()
    {
        GameManager.Instance.QuitGame();
    }

    void Close()
    {
        AudioManager.Instance.PlaySfx("quit");
        UIManager.Instance.Close<SettingsPanel>();
    }

    #endregion
}