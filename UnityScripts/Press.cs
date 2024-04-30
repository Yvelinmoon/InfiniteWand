using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour
{
    public float floatForce = 5f; // �������Ĵ�С
    public float maxHeight = 4f; // �����ϸ������߶�
    public float driftMagnitude = 0.5f; // ����Ч���Ĵ�С
    public float rotationMagnitude = 0.01f; // ��תЧ���Ĵ�С
    public float maxRotation = 30f; // �����ת�Ƕ�

    private bool isFloating = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // ��ȡ�����Rigidbody���
    }

    void Update()
    {
        // ����������Ƿ񱻰���
        if (Input.GetMouseButtonDown(0))
        {
            // ���߼�⣬ȷ�������λ���Ƿ�������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ��������Ƿ�����˱�����
                if (hit.collider.gameObject == gameObject)
                {
                    isFloating = true;
                    Debug.Log("Hit");
                }
            }
        }

        // �����ϸ���δ�ﵽ���߶�ʱ��������ϵ���
        if (isFloating && transform.position.y < maxHeight)
        {
            rb.AddForce(Vector3.up * floatForce);
        }
        // �����嵽��򳬹����߶�ʱ����ʼʩ���������Ч��
        else if (isFloating && transform.position.y >= maxHeight)
        {
            // �����߶ȸ���ʩ���������ģ�⸡��Ч��
            Vector3 drift = new Vector3(
                Random.Range(-driftMagnitude, driftMagnitude), // X����������
                Random.Range(-driftMagnitude, driftMagnitude), // Y����������
                Random.Range(-driftMagnitude, driftMagnitude)  // Z����������
            );
            rb.AddForce(drift, ForceMode.Acceleration);

            // �����ת
            Vector3 randomTorque = new Vector3(
                Random.Range(-rotationMagnitude, 0), // X������Ť��
                Random.Range(-rotationMagnitude, rotationMagnitude), // Y������Ť��
                Random.Range(-rotationMagnitude, rotationMagnitude)  // Z������Ť��
            );

            // �������Ӧ��Ť�غ����ת�Ƕ�
            Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

            // �����ת�Ƕ��Ƿ񳬹����ֵ
            if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)



                rb.AddTorque(randomTorque, ForceMode.Acceleration);
        }
    }

    // ������ͷ�ʱֹͣ����
    void OnMouseUp()
    {
        isFloating = false;
    }
}
