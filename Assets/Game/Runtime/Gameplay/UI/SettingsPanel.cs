using Game.Runtime.Data;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : UIPanel
{

    [Header("Settings - Audio")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Settings - Buttons")]
    public Button btnBack;
    public Button btnQuit;
    public Button btnClose;

    void Start()
    {
        // Slider 监听
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);

        // 按钮绑定
        btnBack.onClick.AddListener(OnBack);
        btnQuit.onClick.AddListener(OnQuit);
        btnClose.onClick.AddListener(()=>UIManager.Instance.Close<SettingsPanel>());
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

    void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    void OnSfxChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    #endregion

    #region 设置按钮
    
    void OnBack()
    {
        GameManager.Instance.GameTitle();
    }

    void OnQuit()
    {
        GameManager.Instance.QuitGame();
    }

    #endregion

}