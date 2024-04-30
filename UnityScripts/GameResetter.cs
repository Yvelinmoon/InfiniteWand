using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 引入场景管理的命名空间

public class GameResetter : MonoBehaviour
{
    private float lastHitTime; // 上次鼠标指向碰撞体的时间
    public float resetTime = 6f; // 设定的重置时间

    void Start()
    {
        // 初始化上次鼠标指向碰撞体的时间为当前时间
        lastHitTime = Time.time;
    }

    void Update()
    {
        // 射线检测，确定鼠标指针位置是否有物体
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 如果射线检测到了碰撞体，重置计时器
            lastHitTime = Time.time;
        }

        // 检查自上次鼠标指向碰撞体以来是否已经过了resetTime秒
        if (Time.time - lastHitTime > resetTime)
        {
            // 如果是，则重置游戏
            ResetGame();
        }
    }

    void ResetGame()
    {
        // 使用场景管理器加载当前活动场景，从而重置游戏
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}