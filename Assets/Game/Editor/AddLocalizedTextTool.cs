using UnityEditor;
using UnityEngine;
using TMPro;

namespace Game.Editor
{
    public class AddLocalizedTextTool
    {
        [MenuItem("Tools/Localization/Add LocalizedText To All TMP")]
        static void Add()
        {
            var texts = Object.FindObjectsOfType<TMP_Text>(true);

            int count = 0;
            foreach (var t in texts)
            {
                if (t.GetComponent<LocalizedText>() == null)
                {
                    t.gameObject.AddComponent<LocalizedText>();
                    count++;
                }
            }

            Debug.Log($"Added LocalizedText to {count} objects.");
        }
    }
}