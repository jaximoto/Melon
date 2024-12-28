using UnityEngine;

public class Elemental : Enemy
{
    public Sprite redElemental;
    public Sprite blueElemental;


   


    private void Update()
    {
        EnemyBehaviour();
    }
    public override void EnemyBehaviour()
    {
        Shot();
       
    }

    

    public override void OnShat(Bullet b)
    {

        if (b.shotType == Bullet.ShotType.ICE_SHOT &&
            myState == EnemyState.Fire)
        {
            // Freeze platform 
            Freeze();
        }
        else if (b.shotType == Bullet.ShotType.FIRE_SHOT &&
                myState == EnemyState.Frozen)
        {
            // Melt platform
            Melt();
        }

    }


    public void Freeze()
    {
        Destroy(gameObject);
    }


    public void Melt()
    {
        Destroy(gameObject);
    }
}
