using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour
{
    public float floatForce = 5f; // 浮动力的大小
    public float maxHeight = 4f; // 物体上浮的最大高度
    public float driftMagnitude = 0.5f; // 浮动效果的大小
    public float rotationMagnitude = 0.01f; // 旋转效果的大小
    public float maxRotation = 30f; // 最大旋转角度

    private bool isFloating = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // 获取物体的Rigidbody组件
    }

    void Update()
    {
        // 检测鼠标左键是否被按下
        if (Input.GetMouseButtonDown(0))
        {
            // 射线检测，确定鼠标点击位置是否有物体
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 检测射线是否击中了本物体
                if (hit.collider.gameObject == gameObject)
                {
                    isFloating = true;
                    Debug.Log("Hit");
                }
            }
        }

        // 物体上浮且未达到最大高度时，添加向上的力
        if (isFloating && transform.position.y < maxHeight)
        {
            rb.AddForce(Vector3.up * floatForce);
        }
        // 当物体到达或超过最大高度时，开始施加随机浮动效果
        else if (isFloating && transform.position.y >= maxHeight)
        {
            // 在最大高度附近施加随机力以模拟浮动效果
            Vector3 drift = new Vector3(
                Random.Range(-driftMagnitude, driftMagnitude), // X方向的随机力
                Random.Range(-driftMagnitude, driftMagnitude), // Y方向的随机力
                Random.Range(-driftMagnitude, driftMagnitude)  // Z方向的随机力
            );
            rb.AddForce(drift, ForceMode.Acceleration);

            // 随机旋转
            Vector3 randomTorque = new Vector3(
                Random.Range(-rotationMagnitude, 0), // X轴的随机扭矩
                Random.Range(-rotationMagnitude, rotationMagnitude), // Y轴的随机扭矩
                Random.Range(-rotationMagnitude, rotationMagnitude)  // Z轴的随机扭矩
            );

            // 计算如果应用扭矩后的旋转角度
            Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

            // 检查旋转角度是否超过最大值
            if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)



                rb.AddTorque(randomTorque, ForceMode.Acceleration);
        }
    }

    // 当鼠标释放时停止浮动
    void OnMouseUp()
    {
        isFloating = false;
    }
}
