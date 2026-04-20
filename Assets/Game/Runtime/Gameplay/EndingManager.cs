using Game.Runtime.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    private int endingDialoguesStar = 5;
    private int exitDialogueId = 15;

    private List<int> endingDialogues = new List<int>()
    {
        6,
        7,
        7,
        8,
        8,
        9
    };

    void Start()
    {
        DialogueManager.Instance.PlayDialogue(endingDialoguesStar);
    }

    private void onDialogueFinished(int id)
    {
        if (id == endingDialoguesStar)
        {
            var score = GameManager.Instance.finalScore;
            DialogueManager.Instance.PlayDialogue(endingDialogues[score]);
        }

        if (id == exitDialogueId)
        {
            //todo : 播放文字
            GameManager.Instance.GameTitle();
        }
    }

    private void OnEnable()
    {
        EventHandler.DialogueFinishedEvent += onDialogueFinished;
    }

    private void OnDisable()
    {
        EventHandler.DialogueFinishedEvent -= onDialogueFinished;
    }
}