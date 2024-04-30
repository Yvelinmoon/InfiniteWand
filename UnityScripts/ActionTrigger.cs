using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    public GameObject lightObject;  // ���Ƶĵƶ���
    public GameObject floatingObject;  // �����Ķ���
    private Rigidbody floatingObjectRigidbody;  // ��ë�� Rigidbody ���

    private void Start()
    {
        // ���Ի�ȡ��ë�� Rigidbody ���
        if (floatingObject != null)
        {
            floatingObjectRigidbody = floatingObject.GetComponent<Rigidbody>();
        }

        if (floatingObjectRigidbody == null)  // ���û�� Rigidbody ������򱨴�
            Debug.LogError("Floating object does not have a Rigidbody component.");
    }

    // ����ģ��Ԥ�������
    public void HandlePredictedIndex(int index)
    {
        switch (index)
        {
            case 3:
                StopFloating();
                break;
            case 7:
                TurnOnLight();
                break;
            case 11:
                StartFloating();
                break;
            case 15:
                // ʲô������
                break;
            default:
                Debug.LogWarning("Unsupported index: " + index);
                break;
        }
    }

    private void TurnOnLight()
    {
        if (lightObject != null)
            lightObject.SetActive(true);  // �򿪵�
        else
            Debug.LogError("Light object is not assigned.");
    }

    private void StopFloating()
    {
        if (lightObject != null)
            lightObject.SetActive(false);  // �رյ�

        if (floatingObjectRigidbody != null)
        {
            // ������������������ë��Ȼ����
            floatingObjectRigidbody.useGravity = true;
        }
        else
        {
            Debug.LogError("Floating object is not assigned or lacks a Rigidbody.");
        }
    }

    private void StartFloating()
    {
        if (floatingObjectRigidbody != null)
        {
            floatingObjectRigidbody.useGravity = false; // �ر�����
            // ����ʩ�Ӹ�������ϸ���������������ë
            floatingObjectRigidbody.AddForce(Vector3.up * (floatingObjectRigidbody.mass * Physics.gravity.magnitude * 1.5f), ForceMode.Force);
            // ������ˮƽ����ʵ�ֻζ�Ч��
            float randomForce = Random.Range(-0.5f, 0.5f);
            floatingObjectRigidbody.AddForce(new Vector3(randomForce, 0, randomForce), ForceMode.Force);
        }
        else
        {
            Debug.LogError("Floating object is not assigned or lacks a Rigidbody.");
        }
    }
}