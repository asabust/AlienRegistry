using System;
using UnityEngine;
using Game.Runtime.Core;
using UnityEngine.UI;
using EventHandler = Game.Runtime.Core.EventHandler;

public class OpeningSceneManager : MonoBehaviour
{
    public Button settingsButton;
    public Button skipButton;

    private void Start()
    {
        settingsButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();

        settingsButton.onClick.AddListener(() => UIManager.Instance.Open<SettingsPanel>());
        skipButton.onClick.AddListener(() =>
        {
            UIManager.Instance.Close<DialoguePanel>();
            GameManager.Instance.EnterGameScene();
        });
        DialogueManager.Instance.PlayDialogue(1);
    }

    private void OnDialogueEnd(int dialogueId)
    {
        if (dialogueId == 4)
        {
            GameManager.Instance.EnterGameScene();
        }
    }

    private void OnEnable()
    {
        EventHandler.DialogueFinishedEvent += OnDialogueEnd;
    }

    private void OnDisable()
    {
        EventHandler.DialogueFinishedEvent -= OnDialogueEnd;
    }
}