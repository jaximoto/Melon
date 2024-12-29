using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Vector3 offset;    // Offset for the camera position
    public float smoothSpeed = 0.125f;  // Smoothing factor

    void FixedUpdate()
    {
        if (player != null)
        {
            // Desired camera position
            Vector3 targetPosition = player.position + offset;

            // Smoothly interpolate the camera position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            transform.position = smoothedPosition;
        }
    }
}
