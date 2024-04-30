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
            // ����Ŀ��λ��
            Vector3 targetPosition = target.position + mainCamera.transform.forward * offset;

            // ȷ��Ŀ��λ�������ǰ��
            if (Vector3.Dot((targetPosition - mainCamera.transform.position).normalized, mainCamera.transform.forward) > 0)
            {
                // ����������ת��Ϊ��Ļ����
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(mainCamera, targetPosition);

                // ����Ļ����ת��ΪCanvas��������
                Vector2 localPoint;
                RectTransform canvasRectTransform = rectTransform.parent as RectTransform; // ����rectTransform�ĸ�����Canvas
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, mainCamera, out localPoint))
                {
                    // ����UIԪ�صı�������
                    rectTransform.anchoredPosition = localPoint;
                }
            }
        }
    }
}