using UnityEngine;
using static Unity.Mathematics.math;

public class Shooter : MonoBehaviour
{

    public PlayerMovement player;

    public GameObject firingPoint;
    public float fireRate;
    public Bullet dummyBullet;
    public float timeSinceLastShot;
    public Bullet.ShotType shotType;
    public Vector3 direction;

    public Sprite redBulletSprite, blueBulletSprite;

    public enum DefaultDirection
    {
        RIGHT,
        LEFT
    };
    public DefaultDirection defaultDirection;

    public enum ShooterType
    {
        PLAYER,
        ENEMY
    };
    public ShooterType shooterType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if(player != null)
        {
            player.Shot += Shot;
            player.Switch += Switch;
        }
        
        timeSinceLastShot = float.PositiveInfinity;
        
        if (defaultDirection==DefaultDirection.RIGHT)
            direction = Vector3.right;
        else
            direction = Vector3.left;
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetAxis("Horizontal") != 0)
            direction = Input.GetAxis("Horizontal") < 0 ? Vector3.left : Vector3.right;
    }


    void Switch(Mode currMode)
    {
        if (this.shotType == Bullet.ShotType.ICE_SHOT)
        {
            this.shotType = Bullet.ShotType.FIRE_SHOT;
        }
        else
        {
            this.shotType = Bullet.ShotType.ICE_SHOT;
        }
    }


    public virtual void Shot()
    {
        Vector3 offset = direction * 0.75f;
        // Has it been enough time since the last shot
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


    public void InitBullet(Bullet bullet)
    {
        // This is stupid
        bullet.shotType = this.shotType;
        bullet.velocity = direction * bullet.bulletSpeed;
        bullet.s.sprite = (this.shotType == Bullet.ShotType.ICE_SHOT) ? blueBulletSprite : redBulletSprite;

        //bullet.gameObject.layer = gameObject.layer;

        // By default, all bullets are facing left, so flip them if shooter direction is right
        if (direction==Vector3.right)
        {
            bullet.s.flipX = !bullet.s.flipX;
        }

        if (shooterType==ShooterType.PLAYER)
        {
            bullet.layerMask = LayerMask.GetMask("EnemyBullet", "Player");
            bullet.gameObject.layer = (int)log2(LayerMask.GetMask("PlayerBullet"));
        }
        else if (shooterType==ShooterType.ENEMY)
        {
            bullet.layerMask = LayerMask.GetMask("PlayerBullet", "Enemy");
            bullet.gameObject.layer = (int)log2(LayerMask.GetMask("EnemyBullet"));
        }
    }

}
