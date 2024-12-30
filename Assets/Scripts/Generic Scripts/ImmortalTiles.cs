using UnityEngine;

public class ImmortalTiles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        Bullet b;
        if (col.gameObject.TryGetComponent<Bullet>(out b))
        {
            Destroy(b.gameObject);
        }
    }
}
