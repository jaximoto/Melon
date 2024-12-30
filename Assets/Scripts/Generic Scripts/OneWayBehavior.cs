using UnityEngine;

public class OneWayBehavior : MonoBehaviour
{
    private Collider2D tilemapCollider;

    void Start()
    {
        // Get the Tilemap's Collider
        tilemapCollider = GetComponent<Collider2D>();
        if (!tilemapCollider)
        {
            Debug.LogError("No Collider2D found on the Tilemap object.");
        }
        Debug.Log(tilemapCollider);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle collision only if a valid collider is involved
        if (collision.collider != null)
        {
            foreach (var contact in collision.contacts)
            {
                // Check if the collision is happening from below
                if (IsBelow(contact.point, collision.collider.bounds.center))
                {
                    // Ignore collision
                    Physics2D.IgnoreCollision(tilemapCollider, collision.collider, true);
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Re-enable collision once the object exits
        if (collision.collider != null)
        {
            Physics2D.IgnoreCollision(tilemapCollider, collision.collider, false);
        }
    }

    private bool IsBelow(Vector2 contactPoint, Vector2 objectCenter)
    {
        // Check if the contact point is below the object
        return contactPoint.y < objectCenter.y;
    }
}
