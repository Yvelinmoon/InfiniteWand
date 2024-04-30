using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press : MonoBehaviour
{
    public float floatForce = 5f; // Magnitude of floating force
    public float maxHeight = 4f; // Maximum height the object floats to
    public float driftMagnitude = 0.5f; // Magnitude of drifting effect
    public float rotationMagnitude = 0.01f; // Magnitude of rotation effect
    public float maxRotation = 30f; // Maximum rotation angle

    private bool isFloating = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component of the object
    }

    void Update()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Raycast to determine if there is an object at the mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hits this object
                if (hit.collider.gameObject == gameObject)
                {
                    isFloating = true;
                    Debug.Log("Hit");
                }
            }
        }

        // If the object is floating and has not reached the maximum height, add upward force
        if (isFloating && transform.position.y < maxHeight)
        {
            rb.AddForce(Vector3.up * floatForce);
        }
        // When the object reaches or exceeds the maximum height, apply random drifting effect
        else if (isFloating && transform.position.y >= maxHeight)
        {
            // Apply random force near the maximum height to simulate drifting effect
            Vector3 drift = new Vector3(
                Random.Range(-driftMagnitude, driftMagnitude), // Random force in the X direction
                Random.Range(-driftMagnitude, driftMagnitude), // Random force in the Y direction
                Random.Range(-driftMagnitude, driftMagnitude)  // Random force in the Z direction
            );
            rb.AddForce(drift, ForceMode.Acceleration);

            // Apply random rotation
            Vector3 randomTorque = new Vector3(
                Random.Range(-rotationMagnitude, 0), // Random torque on the X axis
                Random.Range(-rotationMagnitude, rotationMagnitude), // Random torque on the Y axis
                Random.Range(-rotationMagnitude, rotationMagnitude)  // Random torque on the Z axis
            );

            // Calculate the future rotation if torque is applied
            Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

            // Check if the rotation angles exceed the maximum value
            if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
                Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)
            {
                rb.AddTorque(randomTorque, ForceMode.Acceleration);
            }
        }
    }

    // Stop floating when the mouse is released
    void OnMouseUp()
    {
        isFloating = false;
    }
}