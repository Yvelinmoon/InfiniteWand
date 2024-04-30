using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LimitedRotation : MonoBehaviour
{

    public float maxRotation = 30f; // Maximum rotation angle

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Generate random torque
        Vector3 randomTorque = new Vector3(
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation),
            Random.Range(-maxRotation, maxRotation)
        );

        // Calculate the future rotation if torque is applied
        Quaternion futureRotation = Quaternion.Euler(rb.rotation.eulerAngles + randomTorque * Time.fixedDeltaTime);

        // Check if the rotation angles exceed the maximum value
        if (Mathf.Abs(futureRotation.eulerAngles.x) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.y) <= maxRotation &&
            Mathf.Abs(futureRotation.eulerAngles.z) <= maxRotation)
        {
            // Apply torque
            rb.AddTorque(randomTorque);
        }
    }