using JetBrains.Annotations;
using UnityEngine;

public class Enemy : Shooter, IShootable
{
    public float moveSpeed;
    public int health;
    //shooter variables

    public enum EnemyState
    {
        Frozen,
        Fire,
        Default
    };

    public EnemyState myState;

    public virtual void OnShat(Bullet b)
    {

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
