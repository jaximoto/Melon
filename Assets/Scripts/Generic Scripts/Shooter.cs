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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.Shot += Shot;
        timeSinceLastShot = float.PositiveInfinity;
    }


    // Update is called once per frame
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetAxis("Horizontal") != 0)
            direction = Input.GetAxis("Horizontal") < 0 ? Vector3.left : Vector3.right;
    }


    void Shot()
    {
        // Has it been enough time since the last shot
        if (timeSinceLastShot >= fireRate)
        {
            // Instantiate the bullet from prefab at firing point
            Bullet bullet = Instantiate(dummyBullet, firingPoint.transform.position, Quaternion.identity);

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
        Debug.Log(bullet.velocity);
    }

}
