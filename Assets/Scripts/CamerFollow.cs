using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerFollow : MonoBehaviour
{

    public Transform target;
    public float smoothing = 5.0f;

    Vector3 offset;

    void Start()
    {
        // "offset" is the distance from the camera to the player (the "target").
        offset = (transform.position - target.position);
    }

    void FixedUpdate()
    {
        // The camera follows the player while still keeping the right distance from them.
        Vector3 targetCameraPosition = (target.position + offset);

        // "Vector3.Lerp" is used to smoothly move between two positions.
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, (smoothing * Time.deltaTime));
    }
}
