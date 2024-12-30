using JetBrains.Annotations;
using UnityEngine;

public class Enemy : Shooter, IShootable
{
    public float moveSpeed;
    public int health;


    public EnemyManager enemyManager;

    private Rigidbody2D rb;
    private Animator animator;


    public enum EnemyState
    {
        Frozen,
        Fire,
        Default
    };


    public EnemyState myState;

    public void OnEnable()
    {
	    rb = GetComponent<Rigidbody2D>();	
        animator = GetComponent<Animator>();

        if (gameObject.TryGetComponent<Elemental>(out _))
            rb.bodyType = RigidbodyType2D.Static;
        else
            rb.bodyType = RigidbodyType2D.Dynamic;

        animator.SetTrigger(ResetKey);

		foreach (var param in animator.parameters)
		{
			if (param.type == AnimatorControllerParameterType.Trigger)
			{
				animator.ResetTrigger(param.name);
			}
		}
    }

    
    public override void Start()
    {
        
        Debug.Log($"OLD: {this.gameObject.transform.position}");
        base.Start();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        Debug.Log("Called find");

        enemyManager.enemyPositions[this.gameObject] = new Vector3(this.gameObject.transform.position.x,    
                                                                    this.gameObject.transform.position.y,
																		0.0f);
    }



    public virtual void OnShat(Bullet b)
    {
        enemyManager.HandleEnemyDeath(this.gameObject);
    }


    public virtual void EnemyBehaviour()
    {

    }


    public void LateUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            HandlePlayerCollision(player);
        }
    }


    public void HandlePlayerCollision(PlayerMovement player)
    {
        if (myState == EnemyState.Default)
        {
            player.HandleDeath();
        }
        else if (myState == EnemyState.Frozen)
        {
            if (player.mode == Mode.FIRE_MODE)
                player.HandleDeath();
        }
        else if (myState == EnemyState.Fire)
        {
            if (player.mode == Mode.ICE_MODE)
                player.HandleDeath();
        }
        HandleCollisionAnimation();

    }

    public virtual void HandleCollisionAnimation()
    {

    }

    private static readonly int ResetKey = Animator.StringToHash("Reset");

}
