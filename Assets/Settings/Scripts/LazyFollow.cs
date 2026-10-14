using UnityEngine;

/// <summary>
/// Makes the UI panel smoothly follow the user's gaze.
/// Attach to MainCanvas.
/// </summary>
public class LazyFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("Transform to follow (assign CenterEyeAnchor)")]
    public Transform cameraTransform;

    [Tooltip("Distance from the camera in meters")]
    public float distance = 1.5f;

    [Tooltip("How smoothly it follows (higher = slower)")]
    [Range(0.01f, 1f)]
    public float smoothSpeed = 0.1f;

    [Tooltip("Vertical offset from center of view")]
    public float verticalOffset = -0.1f;

    [Tooltip("Angle threshold before it starts following (degrees)")]
    public float angleTreshold = 30f;

    private Vector3 targetPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            // Try to find CenterEyeAnchor automatically
            GameObject cam = GameObject.Find("CenterEyeAnchor");
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogWarning("[LazyFollow] CenterEyeAnchor not found. Assign manually.");
        }

        // Set initial position
        if (cameraTransform != null)
            SnapToTarget();
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // Calculate target position in front of camera
        Vector3 forward = cameraTransform.forward;
        Vector3 desiredPosition = cameraTransform.position
            + forward * distance
            + Vector3.up * verticalOffset;

        // Only follow if the panel is outside the angle threshold
        float angle = Vector3.Angle(
            transform.position - cameraTransform.position,
            cameraTransform.forward
        );

        if (angle > angleTreshold)
        {
            targetPosition = desiredPosition;
        }

        // Smoothly move toward target
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed
        );

        // Always face the camera
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(transform.position - cameraTransform.position),
            smoothSpeed
        );
    }

    void SnapToTarget()
    {
        Vector3 forward = cameraTransform.forward;
        targetPosition = cameraTransform.position
            + forward * distance
            + Vector3.up * verticalOffset;
        transform.position = targetPosition;
        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position
        );
    }
}