using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import the namespace for scene management

public class GameResetter : MonoBehaviour
{
    private float lastHitTime; // Time when the mouse last hovered over a collider
    public float resetTime = 6f; // The specified reset time

    void Start()
    {
        // Initialize the time when the mouse last hovered over a collider to the current time
        lastHitTime = Time.time;
    }

    void Update()
    {
        // Raycast to determine if there is an object at the mouse pointer position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Reset the timer if a collider is hit by the raycast
            lastHitTime = Time.time;
        }

        // Check if resetTime seconds have passed since the mouse last hovered over a collider
        if (Time.time - lastHitTime > resetTime)
        {
            // If so, reset the game
            ResetGame();
        }
    }

    void ResetGame()
    {
        // Load the current active scene using the scene manager to reset the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}




}