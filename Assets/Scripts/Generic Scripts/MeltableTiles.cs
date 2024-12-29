using UnityEngine;
using UnityEngine.Tilemaps;

public class MeltableTiles : MonoBehaviour
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
        if (col.gameObject.TryGetComponent<Bullet>(out b))
        {
            if (b.shotType == Bullet.ShotType.FIRE_SHOT)
            {
                // Melt platform
                Melt(col);
            }
            Destroy(b.gameObject);
        }
    }


    public Vector3Int GetPos(Collision2D col)
    {
        bool wasFound = false;
        Vector3 collisionPos = col.GetContact(0).point;
        Vector3Int tilePos = tilemap.WorldToCell(collisionPos);

        if (tilemap.HasTile(tilePos)) return tilePos;

        tilePos = tilePos + new Vector3Int(-1, 1, 0);

        for (int i=0; i<3; i++)
        {
            for (int j=0; j<3; j++)
            {
                Vector3Int thisPos = tilePos + new Vector3Int(j, -i, 0);
                if (tilemap.HasTile(thisPos))
                {
                    tilePos = thisPos;
                    wasFound = true;
                    break;
                }
            }
        }

        return tilePos;
    }



    public void Melt(Collision2D col)
    {
        // Get tile, destroy it if it exists in the tilemap
        Vector3Int tilePos = GetPos(col);

        //tilemap.DeleteCells(tilePos, new Vector3Int(1, 1, 1));
        tilemap.SetTile(tilePos, null);
        tilemap.RefreshTile(tilePos);

        //TODO: Animation
    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
