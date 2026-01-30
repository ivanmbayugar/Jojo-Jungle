using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow")]
    public float smoothTime = 0.12f;

    [Header("Vertical Dead Zone")]
    public float deadZoneTop = 0.185f;
    public float deadZoneBottom = 0.27f;

     [Header("Camera Bounds")]
    public bool useLimits = true;
    public BoxCollider2D bounds;

    private Vector3 velocity = Vector3.zero;
    private float minX, maxX, minY, maxY;

    void Start()
    {
        if (!useLimits || !bounds) return;

        Bounds b = bounds.bounds;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        minX = b.min.x + camWidth;
        maxX = b.max.x - camWidth;
        minY = b.min.y + camHeight;
        maxY = b.max.y - camHeight;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = transform.position;

        // --------- Horizontal (siempre sigue) ----------
        targetPos.x = target.position.x;

        // --------- Vertical (con dead zone) ----------
        float camY = transform.position.y;
        float playerY = target.position.y;

        if (playerY > camY + deadZoneTop) targetPos.y = playerY - deadZoneTop;
        else if (playerY < camY - deadZoneBottom)targetPos.y = playerY + deadZoneBottom;

        // Mantener Z
        targetPos.z = transform.position.z;

        if (useLimits)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        // Suavizado
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
