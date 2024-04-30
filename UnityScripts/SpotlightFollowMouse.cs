using UnityEngine;

public class SpotlightFollowMouse : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;  // 获取主摄像机
    }

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // 从摄像机到鼠标位置的射线
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 使聚光灯面向射线击中的点
            transform.LookAt(hit.point);
        }
    }
}
