using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class UIPadParallax : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("移动设置")] [Tooltip("X轴和Y轴的最大移动像素范围。设定为20即可。")]
    public Vector2 maxMovePixels = new(20f, 10f);

    [Tooltip("移动的平滑度，值越大跟随越紧密")] public float smoothSpeed = 10f;

    private bool isHovering;
    private Vector2 originalPosition;

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // 记录初始的锚点位置
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        var targetPosition = originalPosition;

        // 确保正在悬停，并且当前有鼠标设备连接
        if (isHovering && Mouse.current != null)
        {
            var mousePos = Mouse.current.position.ReadValue();

            // 获取鼠标在屏幕上的归一化坐标 (范围转换到 -1 到 1 之间)
            var mouseX = mousePos.x / Screen.width * 2f - 1f;
            var mouseY = mousePos.y / Screen.height * 2f - 1f;

            // 计算目标位置：反向移动
            var targetX = originalPosition.x - mouseX * maxMovePixels.x;
            var targetY = originalPosition.y - mouseY * maxMovePixels.y;

            targetPosition = new Vector2(targetX, targetY);
        }

        // 使用Lerp进行平滑插值过渡
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            Time.deltaTime * smoothSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标进入Pad区域
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开Pad区域
        isHovering = false;
    }

    public void SetBasePosition(Vector2 newPosition)
    {
        originalPosition = newPosition;
    }
}