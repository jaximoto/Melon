using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Elemental : Enemy
{
    public Sprite redElemental;
    public Sprite blueElemental;

    public Sprite redElementalAggro;
    public Sprite blueElementalAggro;

    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator myAnimator;

    [SerializeField]
    private RuntimeAnimatorController redAnimator;

    [SerializeField]
    private AnimatorOverrideController blueAnimator;

    [Range(0f, 1f)]
    public float chargeTime;

    [Range(0f, 1f)]
    public float anticipation;

    [Range(0f, 1f)]
    public float offset;
    bool aggroed = false;
    bool dying = false;
    bool anticipating = false;

    float maxY, minY;
    Vector3 moveDir = Vector3.up;

    Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        maxY = transform.position.y + offset;
        minY = transform.position.y - offset;

        if (myState == EnemyState.Fire)
        {
            myAnimator.runtimeAnimatorController = redAnimator;
        }
        else if (myState == EnemyState.Frozen)
        {
            myAnimator.runtimeAnimatorController = blueAnimator;
        }

    }
    private void Update()
    {
        if (aggroed)
        {
            EnemyBehaviour();
        }

    }
    public override void EnemyBehaviour()
    {
        Movement();
        Shot();
       
    }

    

    public override void OnShat(Bullet b)
    {

        if (b.shotType == Bullet.ShotType.ICE_SHOT &&
            myState == EnemyState.Fire)
        {
            Freeze();
        }
        else if (b.shotType == Bullet.ShotType.FIRE_SHOT &&
                myState == EnemyState.Frozen)
        {
            Melt();
        }

    }


    public void Freeze()
    {
        rb.useGravity = true;
        myAnimator.SetTrigger(DieKey);
    }


    public void Melt()
    {
        rb.useGravity = true;
        myAnimator.SetTrigger(DieKey);
    }


    public void LookAtPlayer(Collider2D col)
    {
        GameObject hit = col.gameObject;
        if (hit.layer == 3 && !aggroed && !dying)
        {
            myAnimator.SetTrigger(AggroKey);
            aggroed = true;
            Aggro();
        }
    }

    void Aggro()
    {

        aggroed = true;
        Debug.Log("seen player");
    }


    public override void Shot()
    {
        Vector3 offset = direction * 0.75f;
        if (timeSinceLastShot >= fireRate * anticipation && !anticipating)
        {
            myAnimator.SetTrigger(ShootKey);
            myAnimator.ResetTrigger(AggroKey);
            StartCoroutine("WaitAndReset");
        }
        if (timeSinceLastShot >= fireRate)
        {
            
            // Instantiate the bullet from prefab at firing point
            Bullet bullet = Instantiate(dummyBullet, firingPoint.transform.position + offset,
                                        Quaternion.identity);

            // Set parameters of the new bullet, epic gamer style
            InitBullet(bullet);
            timeSinceLastShot = 0.0f;
            
        }
    }


    void Movement()
    {
        if (!dying)
        {
            if (transform.position.y >= maxY || transform.position.y <= minY)
            {
                moveDir = moveDir * -1;
            }
            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }
        
    }


    IEnumerator WaitAndReset()
    {
        yield return new WaitForSeconds(chargeTime);
        myAnimator.SetTrigger(AggroKey);
        myAnimator.ResetTrigger(ShootKey);
    }

    IEnumerator WaitAndDie()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    private static readonly int AggroKey = Animator.StringToHash("Aggro");
    private static readonly int ShootKey = Animator.StringToHash("Shooting");
    private static readonly int DieKey = Animator.StringToHash("Dying");
}
