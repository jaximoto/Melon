using JetBrains.Annotations;
using UnityEngine;

public class Enemy : Shooter, IShootable
{
    public float moveSpeed;
    public int health;


    public EnemyManager enemyManager;


    public enum EnemyState
    {
        Frozen,
        Fire,
        Default
    };


    public EnemyState myState;

    
    public override void Start()
    {
        Debug.Log($"OLD: {this.gameObject.transform.position}");
        base.Start();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        Debug.Log("Called find");

        /*
        if (enemyManager.enemyPositions == null)
            enemyManager.enemyPositions = new();
            */

        enemyManager.enemyPositions[this.gameObject] = new Vector3(this.gameObject.transform.position.x,    
                                                                    this.gameObject.transform.position.y,
                                                                    0);
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


}
