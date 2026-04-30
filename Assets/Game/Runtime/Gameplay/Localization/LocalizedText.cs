using Game.Runtime.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    public string key;

    [HideInInspector] public TMP_Text text;

    void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        EventHandler.LanguageChangedEvent += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        EventHandler.LanguageChangedEvent -= Refresh;
    }

    public void SetLocalizationKey(string localizationKey)
    {
        key = localizationKey;
        Refresh();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void Refresh()
    {
        if (!string.IsNullOrEmpty(key))
        {
            text.text = LocalizationManager.Get(key);
        }

        text.font = GameManager.GetFont(LocalizationManager.CurrentLanguage);
    }
}