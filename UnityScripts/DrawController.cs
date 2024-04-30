using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DrawController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<Vector2> points = new List<Vector2>();
    public List<float> times = new List<float>();  // 用于存储每个点的时间戳
    public int maxPoints = 50;
    public RectTransform drawingArea;
    private bool isDrawing = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isDrawing = true;  // 开始或继续绘制
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isDrawing = false;  // 停止绘制
    }

    void Update()
    {
        if (isDrawing && Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
        {
            Vector2 mousePos = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingArea, mousePos, null, out Vector2 localPoint))
            {
                if (points.Count == 0 || (points.Count > 0 && Vector2.Distance(points[points.Count - 1], localPoint) > 1)) // 确保点与点之间有一定距离
                {
                    points.Add(localPoint);
                    times.Add(Time.time);  // 记录当前点的时间
                    if (points.Count == maxPoints)
                    {
                        SendData();  // 当点数达到最大限制时发送数据
                    }
                }
            }
        }
    }

    private void SendData()
    {
        // 传递点和时间数据给ActionRecognition的实例
        ActionRecognition.Instance.ProcessPoints(points, times);
        ClearData();  // 发送数据后清空列表以便重新开始
    }

    private void ClearData()
    {
        points.Clear();
        times.Clear();
        // 不再在这里设置 isDrawing 为 false，允许继续绘制
    }
}