using UnityEngine;

public class ArenaBoundary : MonoBehaviour
{
    public static ArenaBoundary Instance;

    [Header("Arena Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    void Awake()
    {
        Instance = this;
    }

    // Called by attacks when hitting a wall
    public void HandleRangedHit(GameObject projectile)
    {
        Destroy(projectile);
    }

    // Called by melee attacks to trim themselves
    public void TrimMeleeHitbox(BoxCollider2D box, Transform attackTransform)
    {
        Vector2 size = box.size;
        Vector2 offset = box.offset;

        float left = attackTransform.position.x + offset.x - size.x / 2f;
        float right = attackTransform.position.x + offset.x + size.x / 2f;
        float bottom = attackTransform.position.y + offset.y - size.y / 2f;
        float top = attackTransform.position.y + offset.y + size.y / 2f;

        // Trim left
        if (left < minX)
        {
            float excess = minX - left;
            size.x -= excess;
            offset.x += excess / 2f;
        }

        // Trim right
        if (right > maxX)
        {
            float excess = right - maxX;
            size.x -= excess;
            offset.x -= excess / 2f;
        }

        // Trim bottom
        if (bottom < minY)
        {
            float excess = minY - bottom;
            size.y -= excess;
            offset.y += excess / 2f;
        }

        // Trim top
        if (top > maxY)
        {
            float excess = top - maxY;
            size.y -= excess;
            offset.y -= excess / 2f;
        }

        box.size = size;
        box.offset = offset;
    }
}