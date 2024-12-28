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


}
