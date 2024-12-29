using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MeltableTiles : MonoBehaviour
{
    public Tilemap tilemap;

    static public float regenTime = 1.0f; //seconds

    private class MeltedTile
    {
        public float timeSinceMelted = 0.0f;
        public Vector3Int pos {get; set;}
        public TileBase tile {get; set;}

        public MeltedTile(Vector3Int pos, TileBase tile)
        {
            this.pos = pos;
            this.tile = tile;
        }

        public bool ShouldRegen()
        {
            return timeSinceMelted >= regenTime;
        }
    };

    List<MeltedTile> meltedTiles = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        UpdateMeltedTiles();
    }


    void UpdateMeltedTiles()
    {
        List<MeltedTile> toDelete = new();
        for(int i=0; i<meltedTiles.Count; i++)
        {
            var meltedTile = meltedTiles[i];

            meltedTile.timeSinceMelted += Time.deltaTime;

            if (meltedTile.ShouldRegen())
            {
                tilemap.SetTile(meltedTile.pos, meltedTile.tile);
                toDelete.Add(meltedTile);
            }
        }

        foreach(var meltedTile in toDelete)
        {
            meltedTiles.Remove(meltedTile);
        }
    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        Bullet b;
        if (col.gameObject.TryGetComponent<Bullet>(out b))
        {
            if (b.shotType == Bullet.ShotType.FIRE_SHOT)
            {
                // Melt platform
                MeltShot(col);
            }
            Destroy(b.gameObject);
        }

        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            if (player.mode == Mode.FIRE_MODE)
            {
                MeltBelow(col); //TODO: Delayed melt
            }
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


    public void MeltShot(Collision2D col)
    {
        // Get tile, destroy it if it exists in the tilemap
        Vector3Int tilePos = GetPos(col);
        DoMelt(tilePos);
    }


    public void MeltBelow(Collision2D col)
    {
        // Get tile, destroy it if it exists in the tilemap
        Vector3 collisionPos = col.GetContact(0).point;
        Vector3Int tilePos = tilemap.WorldToCell(collisionPos) + Vector3Int.down;

        for (int i=-1; i<2; i++)
        {
            Vector3Int offset = new Vector3Int(i, 0, 0);

            // Do not melt spikes
            if (col.gameObject.TryGetComponent<SpikeTiles>(out _))
                break;

            DoMelt(tilePos + offset);
        }

    }


    public void DoMelt(Vector3Int tilePos)
    {
        meltedTiles.Add(new MeltedTile( tilePos, tilemap.GetTile(tilePos)) );

        tilemap.SetTile(tilePos, null);
        tilemap.RefreshTile(tilePos);

        //TODO: Animation
    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
