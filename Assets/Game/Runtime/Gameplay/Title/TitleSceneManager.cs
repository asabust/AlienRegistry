using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    public Button startGameButton;
    public Button settingButton;
    public Button membersButton;
    public Button exitGameButton;
    public GameObject members;

    private void Start()
    {
        if (settingButton)
            settingButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySfx("click");
                UIManager.Instance.Open<SettingsPanel>(false);
            });
        if (startGameButton)
            startGameButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySfx("click");
                GameManager.Instance.StartNewGame();
                // GameManager.Instance.EnterGameScene();
            });
        if (exitGameButton)
            exitGameButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySfx("click");
                GameManager.Instance.QuitGame();
            });
        if (membersButton)
            membersButton.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlaySfx("click");
                members.SetActive(true);
            });
    }
}