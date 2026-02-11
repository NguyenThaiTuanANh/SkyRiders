using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;        // máy bay

    [Header("Position Settings")]
    public Vector3 offset = new Vector3(0, 6f, -10f); // cao + phía sau
    public float followSmoothTime = 0.25f;

    [Header("Rotation Settings")]
    public float rotateSmoothSpeed = 5f;

    public float fixedXAngle = 45f;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        // ===== POSITION FOLLOW (theo hướng máy bay) =====
        Vector3 desiredPos = target.position
                           + target.rotation * offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref velocity,
            followSmoothTime
        );

        // ===== ROTATION FOLLOW (nhìn theo hướng bay) =====
        float targetY = target.eulerAngles.y;

        Quaternion desiredRot = Quaternion.Euler(
            fixedXAngle,    // 🔒 khóa X = 40
            targetY,        // xoay theo hướng bay
            0f
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            rotateSmoothSpeed * Time.deltaTime
        );
    }
}
