using Game.Runtime.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public GameObject members;
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
        members.gameObject.SetActive(false);
        members.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.GameTitle());
        DialogueManager.Instance.PlayDialogue(endingDialoguesStar);
    }

    private void onDialogueFinished(int id)
    {
        var score = GameManager.Instance.finalScore;
        if (id == endingDialoguesStar)
        {
            Debug.Log(endingDialogues);

            DialogueManager.Instance.ShowDialogueString(
                $"Your dispatches have disappointed them {5 - score} times and satisfied them {score} times!",
                999);
        }

        if (id == 999)
        {
            DialogueManager.Instance.PlayDialogue(endingDialogues[score]);
        }

        if (id == exitDialogueId)
        {
            //todo : 播放文字
            members.gameObject.SetActive(true);
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