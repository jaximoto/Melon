using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLife;
    public float lifeTime ;
    public Vector3 direction;

    public Vector3 velocity;

    public enum ShotType
    {
        ICE_SHOT,
        FIRE_SHOT
    }

    public ShotType shotType;

    void Start()
    {
        // Set death timer for lifeTime seconds
        Destroy(gameObject, lifeTime);
    }


    void Update()
    {
        // Move in direction
        transform.Translate(velocity * Time.deltaTime);

        // TODO: Check for collision?
        
        // TODO: Unload if bullet is off screen, right now it is time based which works but not best

    }
    
}
