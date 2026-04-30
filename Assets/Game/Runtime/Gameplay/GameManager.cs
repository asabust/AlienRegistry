using Game.Runtime.Core;
using Game.Runtime.Core.Attributes;
using Game.Runtime.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using EventHandler = Game.Runtime.Core.EventHandler;


public class GameManager : Singleton<GameManager>
{
    [SceneName] public string titleScene;
    [SceneName] public string openingScene;
    [SceneName] public string firstGameScene;
    [SceneName] public string endingScene;

    public TMP_FontAsset englishFont;
    public TMP_FontAsset chineseFont;
    public TMP_FontAsset japaneseFont;

    public bool IsGameplay => CurrentPhase == GamePhase.Gameplay;
    public GamePhase CurrentPhase { get; private set; }

    public int finalScore { get; private set; }

    public void SetGamePhase(GamePhase newPhase)
    {
        if (CurrentPhase == newPhase) return;
        CurrentPhase = newPhase;
        EventHandler.CallGamePhaseChangedEvent(newPhase);
    }

    protected override void Awake()
    {
        base.Awake();
        EventHandler.AfterSceneLoadEvent += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        EventHandler.AfterSceneLoadEvent -= OnSceneLoaded;
    }

    private void Start()
    {
        GameTitle(); //从标题界面开始
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewGame()
    {
        SetGamePhase(GamePhase.GameOpening);
        TransitionManager.Instance.TransitionTo(openingScene);
    }

    public void EnterGameScene()
    {
        SetGamePhase(GamePhase.Gameplay);
        TransitionManager.Instance.TransitionTo(firstGameScene);
    }

    /// <summary>
    /// 游戏开始界面
    /// </summary>
    public void GameTitle()
    {
        SetGamePhase(GamePhase.GameTitle);
        TransitionManager.Instance.TransitionTo(titleScene);
    }


    public void GameEnding(int correctCount)
    {
        finalScore = correctCount;
        SetGamePhase(GamePhase.GameOver);
        TransitionManager.Instance.TransitionTo(endingScene);
    }

    /// <summary>
    ///     退出游戏
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    #region 本地化

    private void OnSceneLoaded(string obj)
    {
        ApplyFont(LocalizationManager.CurrentLanguage);
    }

    public static void ApplyFont(Language lang)
    {
        var font = GetFont(lang);

        var texts = Object.FindObjectsOfType<LocalizedText>(true);

        foreach (var t in texts)
        {
            if (t.text != null)
            {
                t.text.font = font;
                t.text.ForceMeshUpdate();
            }
        }
    }

    public static TMP_FontAsset GetFont(Language lang)
    {
        return lang switch
        {
            Language.Chinese => Instance.chineseFont,
            Language.Japanese => Instance.japaneseFont,
            _ => Instance.englishFont
        };
    }

    #endregion
}