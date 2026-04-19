using UnityEngine;

public class UITestOpen : MonoBehaviour
{
    [SerializeField] private int characterId = 2;

    public void OpenNow()
    {
        if (characterId <= 0)
        {
            Debug.LogWarning("characterId 必须大于 0");
            return;
        }

        Debug.Log($"OpenNow clicked, characterId={characterId}");

        UIManager.Instance.Open<PlanetsPanel>(
            new PlanetsPanel.OpenData
            {
                characterId = characterId
            }
        );
    }
}
