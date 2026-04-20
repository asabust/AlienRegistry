using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SequenceAnimation : MonoBehaviour
{
    [Header("设置")] public List<Sprite> frames; // 拖入那 14 张图
    public float fps = 12f; // 每秒帧数
    public bool loop = true;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;
    private float timer = 0f;

    void Awake()
    {
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames == null || frames.Count == 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / fps)
        {
            timer = 0f;
            currentIndex++;

            if (currentIndex >= frames.Count)
            {
                if (loop) currentIndex = 0;
                else return;
            }

            // 同时兼容 UI 和 场景物体
            if (uiImage != null) uiImage.sprite = frames[currentIndex];
            if (spriteRenderer != null) spriteRenderer.sprite = frames[currentIndex];
        }
    }
}