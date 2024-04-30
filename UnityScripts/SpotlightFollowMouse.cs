using UnityEngine;

public class SpotlightFollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;  // ��ȡ�������
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // ������������λ�õ�����
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ʹ�۹���������߻��еĵ�
            transform.LookAt(hit.point);
        }
    }
}
