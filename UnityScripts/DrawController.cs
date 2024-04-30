using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DrawController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<Vector2> points = new List<Vector2>();
    public List<float> times = new List<float>();  // ���ڴ洢ÿ�����ʱ���
    public int maxPoints = 50;
    public RectTransform drawingArea;
    private bool isDrawing = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isDrawing = true;  // ��ʼ���������
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isDrawing = false;  // ֹͣ����
    }

    void Update()
    {
        if (isDrawing && Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, mousePos, null, out Vector2 localPoint))
            {
                if (points.Count == 0 || (points.Count > 0 && Vector2.Distance(points[points.Count - 1], localPoint) > 1)) // ȷ�������֮����һ������
                {
                    points.Add(localPoint);
                    times.Add(Time.time);  // ��¼��ǰ���ʱ��
                    if (points.Count == maxPoints)
                    {
                        SendData();  // �������ﵽ�������ʱ��������
                    }
                }
            }
        }
    }

    private void SendData()
    {
        // ���ݵ��ʱ�����ݸ�ActionRecognition��ʵ��
        ActionRecognition.Instance.ProcessPoints(points, times);
        ClearData();  // �������ݺ�����б��Ա����¿�ʼ
    }

    private void ClearData()
    {
        points.Clear();
        times.Clear();
        // �������������� isDrawing Ϊ false�������������
    }
}