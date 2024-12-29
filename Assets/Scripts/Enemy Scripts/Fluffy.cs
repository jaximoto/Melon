using System.Collections;
using UnityEngine;


public class Fluffy : Enemy
{
    Animator animator;
    public Collider2D viewRadius;
    bool aggroed = false;
    bool dying = false;
    

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    
    void Update()
    {
        EnemyBehaviour();
    }

    public override void EnemyBehaviour()
    {
        if (aggroed && animator.GetBool(WalkKey))
        {
            Debug.Log("go");
            Walk();
        }
    }

    public override void OnShat(Bullet b)
    {
        if (b.shotType == Bullet.ShotType.ICE_SHOT)
        {
            Freeze();
        }
        else if (b.shotType == Bullet.ShotType.FIRE_SHOT)
        {
            Melt();
        }
    }


    public void Freeze()
    {
        animator.SetTrigger(FreezeKey);
        StartCoroutine("Die");
    }


    public void Melt()
    {
        animator.SetTrigger(BurnKey);
        StartCoroutine("Die");
    }


    public void Walk()
    {
        
        transform.Translate(moveSpeed * Vector2.left * Time.deltaTime);
        Debug.Log(moveSpeed * Vector2.left * Time.deltaTime);
    }


    public void LookAtPlayer(Collider2D col)
    {
        GameObject hit = col.gameObject;
        if (hit.layer == 3 && !aggroed && !dying) 
        {
            aggroed = true;
            if (hit.transform.position.x > transform.position.x)
            {
                defaultDirection = DefaultDirection.RIGHT;
            }
            else if (hit.transform.position.x < transform.position.x)
            {
                defaultDirection = DefaultDirection.LEFT;
            }
            StartCoroutine("Aggro");
        } 
    }

    IEnumerator Aggro()
    {
        animator.SetTrigger(SeenKey);
        Debug.Log("seenKey");
        yield return new WaitForSeconds(1);
        animator.SetTrigger(WalkKey);
        animator.ResetTrigger(SeenKey);
        Debug.Log("walkkey");
    }

    IEnumerator Die()
    {
        dying = true;
        aggroed = false;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }



    private static readonly int FreezeKey = Animator.StringToHash("IsFrozen");
    private static readonly int BurnKey = Animator.StringToHash("IsBurned");
    private static readonly int SeenKey = Animator.StringToHash("SeenPlayer");

    // Change walk key  it isnt found TODOOO
    private static readonly int WalkKey = Animator.StringToHash("IsMoving");


    
}
