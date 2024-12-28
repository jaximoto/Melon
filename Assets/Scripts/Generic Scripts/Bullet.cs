using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletLife;
    public float lifeTime ;
    public Vector3 direction;
    public Vector3 velocity;

    public SpriteRenderer s;

    public enum ShotType
    {
        ICE_SHOT,
        FIRE_SHOT
    }

    public ShotType shotType;

    private Animator _anim;
    public RuntimeAnimatorController _iceAnim;
    public AnimatorOverrideController _fireAnim;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }


    void Start()
    {
        // Set death timer for lifeTime seconds
        s = gameObject.GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifeTime);

        if (shotType == ShotType.ICE_SHOT)
            _anim.runtimeAnimatorController = _iceAnim;
        else
            _anim.runtimeAnimatorController = _fireAnim;
    }


    void Update()
    {
        // Move in direction
        transform.Translate(velocity * Time.deltaTime);

        // TODO: Unload if bullet is off screen, right now it is time based which works but not best

    }


    void OnCollisionEnter2D(Collision2D col)
    {
        CheckAndHandleCollision(col.gameObject);
    }


    void CheckAndHandleCollision(GameObject obj)
    {
        IShootable i;
        if (obj.TryGetComponent<IShootable>(out i))
        {
            i.OnShat(this);
            Destroy(gameObject);
        }

    }
    
}
