using Game.Runtime.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSceneManager : MonoBehaviour
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

            var result = string.Format(LocalizationManager.Get("ending_result_template"), 5 - score, score);
            DialogueManager.Instance.ShowDialogueString(result, 999);
        }

        if (id == 999)
        {
            DialogueManager.Instance.PlayDialogue(endingDialogues[score]);
        }

        if (id == exitDialogueId)
        {
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