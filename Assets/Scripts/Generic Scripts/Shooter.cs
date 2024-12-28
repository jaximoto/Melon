using UnityEngine;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.Shot += Shot;
        player.Switch += Switch;
        timeSinceLastShot = float.PositiveInfinity;
        direction = Vector3.right;
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

    void Shot()
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

    void InitBullet(Bullet bullet)
    {
        // This is stupid
        bullet.shotType = this.shotType;
        bullet.velocity = direction * bullet.bulletSpeed;
        bullet.s.sprite = (this.shotType == Bullet.ShotType.ICE_SHOT) ? blueBulletSprite : redBulletSprite;
    }

}
