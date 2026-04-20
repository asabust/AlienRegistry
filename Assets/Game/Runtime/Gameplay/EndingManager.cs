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
        9,
        8,
        8,
        7,
        7,
        6
    };

    void Start()
    {
        DialogueManager.Instance.PlayDialogue(endingDialoguesStar);
    }

    private void onDialogueFinished(int id)
    {
        if (id == endingDialoguesStar)
        {
            Debug.Log(endingDialogues);

            DialogueManager.Instance.ShowDialogueString(
                $"Your dispatches have disappointed them 5 times and satisfied them {GameManager.Instance.finalScore} times!",
                999);
        }

        if (id == 999)
        {
            var score = GameManager.Instance.finalScore;
            DialogueManager.Instance.PlayDialogue(score);
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