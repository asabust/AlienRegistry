using System.Collections.Generic;
using Game.Runtime.Core;
using Game.Runtime.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackageView : MonoBehaviour
{
    [Header("Item Detail")] [SerializeField]
    private Image itemLargeImage;

    [SerializeField] private LocalizedText itemNameText;
    [SerializeField] private LocalizedText itemDescText;

    [Header("Item Slots")] [SerializeField]
    private ToggleGroup toggleGroup;

    // 在编辑器里直接把那4个固定的 Slot 拖进去
    [SerializeField] private List<Toggle> slots = new List<Toggle>();
    private List<Image> slotImages = new List<Image>();

    private void Awake()
    {
        slotImages.Clear();
        // 为每个 Slot 绑定监听
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i; // 闭包捕获
            slots[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn) OnSlotSelected(index);
            });
            slotImages.Add(slots[i].transform.Find("Icon").GetComponent<Image>());
        }
    }

    // 缓存当前的物品列表，方便点击时查询
    private List<int> currentItemIds;

    /// <summary>
    /// PadManager 调用此方法，直接传入角色数据
    /// </summary>
    public void RefreshView(CharacterData characterData)
    {
        if (characterData == null) return;
        currentItemIds = characterData.itemIds;
        Debug.Log($"刷新背包 Character={characterData.id} 道具数量={currentItemIds.Count}");

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < currentItemIds.Count)
            {
                // 有数据，显示 Slot 
                slots[i].gameObject.SetActive(true);
                FillSlotInfo(slotImages[i], currentItemIds[i]);
            }
            else
            {
                // 没数据，隐藏多余的 Slot
                // slotImages[i].gameObject.SetActive(false);
                slots[i].gameObject.SetActive(false);
            }
        }

        if (currentItemIds.Count > 0)
        {
            slots[0].isOn = true;
        }
        else
        {
            ClearDetailDisplay();
        }
    }

    private void FillSlotInfo(Image iconImage, int itemId)
    {
        iconImage.gameObject.SetActive(true);
        if (DataLoader.Instance.gameData.items.TryGetValue(itemId, out ItemData data))
        {
            Sprite iconSprite = Resources.Load<Sprite>($"item/{data.iconName}");
            if (iconSprite != null) iconImage.sprite = iconSprite;
        }
    }

    public void OnSlotSelected(int index)
    {
        if (currentItemIds == null || index >= currentItemIds.Count) return;

        AudioManager.Instance.PlaySfx("click_package");
        int itemId = currentItemIds[index];
        UpdateDetailDisplay(itemId);
        InspectionManager.Instance.OnItemViewed(itemId);
    }

    private void UpdateDetailDisplay(int itemId)
    {
        if (DataLoader.Instance.gameData.items.TryGetValue(itemId, out ItemData data))
        {
            itemNameText.SetLocalizationKey(data.name);
            itemDescText.SetLocalizationKey(data.description);
            itemLargeImage.sprite = Resources.Load<Sprite>($"item/{data.iconName}");
            itemLargeImage.preserveAspect = true;
        }
        else
        {
            Debug.Log($"Item not found id={itemId}");
        }
    }

    private void ClearDetailDisplay()
    {
        itemNameText.SetLocalizationKey("");
        itemDescText.SetLocalizationKey("");
        itemLargeImage.sprite = null;
    }
}