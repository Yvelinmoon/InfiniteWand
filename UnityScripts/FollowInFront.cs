using UnityEngine;

public class FollowInFront : MonoBehaviour
{
    public Transform target;
    public Camera mainCamera;
    public float offset = 2.0f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        if (target != null && mainCamera != null)
        {
            // 计算目标位置
            Vector3 targetPosition = target.position + mainCamera.transform.forward * offset;

            // 确保目标位于摄像机前方
            if (Vector3.Dot((targetPosition - mainCamera.transform.position).normalized, mainCamera.transform.forward) > 0)
            {
                // 将世界坐标转换为屏幕坐标
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, targetPosition);

                // 将屏幕坐标转换为Canvas本地坐标
                Vector2 localPoint;
                RectTransform canvasRectTransform = rectTransform.parent as RectTransform; // 假设rectTransform的父级是Canvas
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, mainCamera, out localPoint))
                {
                    // 设置UI元素的本地坐标
                    rectTransform.anchoredPosition = localPoint;
                }
            }
        }
    }
}