using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedRotation : MonoBehaviour
{
   
    public float maxRotation = 30f; // �����ת�Ƕ�

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // �������Ť��
        Vector3 randomTorque = new Vector3(
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation)
        );

        // �������Ӧ��Ť�غ����ת�Ƕ�
        Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

        // �����ת�Ƕ��Ƿ񳬹����ֵ
        if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)
        {
            // Ӧ��Ť��
            rb.AddTorque(randomTorque);
        }
    }

}
