using UnityEngine;

public class ActionTrigger : MonoBehaviour
{
    public GameObject lightObject;  // 控制的灯对象
    public GameObject floatingObject;  // 悬浮的对象
    private Rigidbody floatingObjectRigidbody;  // 羽毛的 Rigidbody 组件

    private void Start()
    {
        // 尝试获取羽毛的 Rigidbody 组件
        if (floatingObject != null)
        {
            floatingObjectRigidbody = floatingObject.GetComponent<Rigidbody>();
        }

        if (floatingObjectRigidbody == null)  // 如果没有 Rigidbody 组件，则报错
            Debug.LogError("Floating object does not have a Rigidbody component.");
    }

    // 处理模型预测的索引
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
                // 什么都不做
                break;
            default:
                Debug.LogWarning("Unsupported index: " + index);
                break;
        }
    }

    private void TurnOnLight()
    {
        if (lightObject != null)
            lightObject.SetActive(true);  // 打开灯
        else
            Debug.LogError("Light object is not assigned.");
    }

    private void StopFloating()
    {
        if (lightObject != null)
            lightObject.SetActive(false);  // 关闭灯

        if (floatingObjectRigidbody != null)
        {
            // 重新启用重力，让羽毛自然下落
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
            floatingObjectRigidbody.useGravity = false; // 关闭重力
            // 持续施加更大的向上浮力以显著提升羽毛
            floatingObjectRigidbody.AddForce(Vector3.up * (floatingObjectRigidbody.mass * Physics.gravity.magnitude * 1.5f), ForceMode.Force);
            // 添加随机水平力以实现晃动效果
            float randomForce = Random.Range(-0.5f, 0.5f);
            floatingObjectRigidbody.AddForce(new Vector3(randomForce, 0, randomForce), ForceMode.Force);
        }
        else
        {
            Debug.LogError("Floating object is not assigned or lacks a Rigidbody.");
        }
    }
}