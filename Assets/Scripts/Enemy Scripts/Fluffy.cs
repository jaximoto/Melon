using System.Collections;
using UnityEngine;


public class Fluffy : Enemy
{
    public Collider2D viewRadius;


    SpriteRenderer spriteRenderer;
    Rigidbody2D myRigidbody2D;


    public void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Start()
    {
        base.Start();
        player = null;//
        //BounceEnemy.HitWall += FlipFluffy;
  
    }


    private void OnDisable()
    {
        //BounceEnemy.HitWall -= FlipFluffy;   
    }
    void Update()
    {
        if(!celebrating)
        {
            EnemyBehaviour();
        }
        
    }

    public void FlipFluffy()
    {
        Debug.Log("FLIP");
        if (flipper == 1)
        {
            flipper = -1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            //spriteRenderer.flipY = true;
        }
        else
        {
            flipper = 1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            //spriteRenderer.flipY = false;
        }
    }

    public override void EnemyBehaviour()
    {
        if (aggroed && animator.GetBool(WalkKey))
        {
            Debug.Log("go");
            Walk(flipper);
        }
    }

    public override void OnShat(Bullet b)
    {
        base.OnShat(b);
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
        ResetAllTriggers(animator);
        animator.SetTrigger(FreezeKey);
        StartCoroutine("Die");
    }


    public void Melt()
    {
        ResetAllTriggers(animator);
        animator.SetTrigger(BurnKey);
        StartCoroutine("Die");
    }


    public void Walk(int flipper)
    {
        
        transform.Translate(moveSpeed * flipper * Vector2.left * Time.deltaTime);
        Debug.Log(moveSpeed * Vector2.left * Time.deltaTime);
    }


    public void LookAtPlayer(Collider2D col)
    {
        GameObject hit = col.gameObject;
        if (hit.layer == 3 && !aggroed && !dying) 
        {
            Debug.Log("aggroed");
            aggroed = true;
            if (hit.transform.position.x > transform.position.x)
            {
                flipper = -1;
                defaultDirection = DefaultDirection.RIGHT;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (hit.transform.position.x < transform.position.x)
            {
                flipper = 1;
                defaultDirection = DefaultDirection.LEFT;
            }
            StartCoroutine("Aggro");
        } 
    }


    public override void HandleCollisionAnimation()
    {

        celebrating = true;
        Debug.Log("Handling?");
        ResetAllTriggers(animator);
        animator.SetTrigger(CelebrateKey);
            
        

    }


    IEnumerator Aggro()
    {
        ResetAllTriggers(animator);
        animator.SetTrigger(SeenKey);
        
        Debug.Log("seenKey");
        yield return new WaitForSeconds(1);
        ResetAllTriggers(animator);
        animator.SetTrigger(WalkKey);
        
        Debug.Log("walkkey");
    }

    IEnumerator Die()
    {
        lastFlipper = flipper;
        myRigidbody2D.bodyType = RigidbodyType2D.Static;
        dying = true;
        aggroed = false;
        yield return new WaitForSeconds(0.5f);
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }



    private static readonly int FreezeKey = Animator.StringToHash("IsFrozen");
    private static readonly int BurnKey = Animator.StringToHash("IsBurned");
    private static readonly int SeenKey = Animator.StringToHash("SeenPlayer");

    // Change walk key  it isnt found TODOOO
    private static readonly int WalkKey = Animator.StringToHash("IsMoving");
    private static readonly int CelebrateKey = Animator.StringToHash("Celebrating");
 
}
