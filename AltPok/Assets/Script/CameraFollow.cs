using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Zoom Settings")]
    public float defaultSize = 5f;       // Normal camera size
    public float battleSize = 7f;        // Zoomed out during battle
    public float zoomSpeed = 2f;         // How fast the camera zooms

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = defaultSize;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        // Smoothly adjust zoom
        float targetSize = (target == null) ? defaultSize : cam.orthographicSize;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
    }

    // Call this to lock the camera to a position (battle center)
    public void LockTo(Vector3 position)
    {
        target = null; // Stop following player
        transform.position = new Vector3(position.x, position.y, transform.position.z);

        // Smoothly zoom out
        StartCoroutine(ZoomOut());
    }

    private System.Collections.IEnumerator ZoomOut()
    {
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * zoomSpeed;
            cam.orthographicSize = Mathf.Lerp(startSize, battleSize, elapsed);
            yield return null;
        }
    }

    // Reset camera after battle
    public void ResetCamera(Transform newTarget)
    {
        target = newTarget;
        StartCoroutine(ZoomBack());
    }

    private System.Collections.IEnumerator ZoomBack()
    {
        float startSize = cam.orthographicSize;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * zoomSpeed;
            cam.orthographicSize = Mathf.Lerp(startSize, defaultSize, elapsed);
            yield return null;
        }
    }
}