using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ���볡������������ռ�

public class GameResetter : MonoBehaviour
{
    private float lastHitTime; // �ϴ����ָ����ײ���ʱ��
    public float resetTime = 6f; // �趨������ʱ��

    void Start()
    {
        // ��ʼ���ϴ����ָ����ײ���ʱ��Ϊ��ǰʱ��
        lastHitTime = Time.time;
    }

    void Update()
    {
        // ���߼�⣬ȷ�����ָ��λ���Ƿ�������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ������߼�⵽����ײ�壬���ü�ʱ��
            lastHitTime = Time.time;
        }

        // ������ϴ����ָ����ײ�������Ƿ��Ѿ�����resetTime��
        if (Time.time - lastHitTime > resetTime)
        {
            // ����ǣ���������Ϸ
            ResetGame();
        }
    }

    void ResetGame()
    {
        // ʹ�ó������������ص�ǰ��������Ӷ�������Ϸ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}