using UnityEngine;
using UnityEngine.Tilemaps;

public class MeltableTiles : MonoBehaviour, IShootable
{

    public Sprite blueTileSprite;

    public Tilemap tilemap;

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
        Debug.Log("Called on collision");
        if (col.gameObject.TryGetComponent<Bullet>(out b))
        {
            Debug.Log("Found Bullet");
            if (b.shotType == Bullet.ShotType.FIRE_SHOT)
            {
                // Melt platform
                Debug.Log("Melting");
                Melt(col);
            }
        }
    }


    public void OnShat(Bullet b)
    {
    }


    public void Melt(Collision2D col)
    {
        // Get tile, destroy it
        Vector3 collisionPos = col.GetContact(0).point;
        Vector3Int tilePos = tilemap.WorldToCell(collisionPos);
        Debug.Log(tilePos);
        if (!tilemap.HasTile(tilePos))
            return;
        Debug.Log("Worked");

        tilemap.DeleteCells(tilePos, new Vector3Int(1, 1, 1));

        //TODO: Animation

    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
