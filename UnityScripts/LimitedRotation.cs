using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedRotation : MonoBehaviour
{
   
    public float maxRotation = 30f; // 最大旋转角度

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 随机生成扭矩
        Vector3 randomTorque = new Vector3(
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation)
        );

        // 计算如果应用扭矩后的旋转角度
        Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

        // 检查旋转角度是否超过最大值
        if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)
        {
            // 应用扭矩
            rb.AddTorque(randomTorque);
        }
    }

}
