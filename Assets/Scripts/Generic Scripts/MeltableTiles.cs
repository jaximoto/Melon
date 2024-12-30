using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MeltableTiles : MonoBehaviour
{
    public Tilemap tilemap;

    [Range(0, 5)] public float regenTime = 1.0f; //seconds
    [Range(0, 5)] public float meltTime = 1.0f; //seconds

    private class MeltedTile
    {
        public float regenTime;
        public float timeSinceMelted = 0.0f;
        public Vector3Int pos {get; set;}
        public TileBase tile {get; set;}

        public MeltedTile(Vector3Int pos, TileBase tile, float regenTime)
        {
            this.pos = pos;
            this.tile = tile;
            this.regenTime = regenTime;
        }

        public bool ShouldRegen()
        {
            return timeSinceMelted >= regenTime;
        }
    };


    //This is basically the same as melted tile
    private class MeltingTile
    {
        public float meltTime;
        public float timeMelting = 0.0f;
        public Vector3Int pos {get; set;}
        public TileBase tile {get; set;}

        public MeltingTile() {this.pos=Vector3Int.zero; this.tile=null; this.meltTime=0.0f;}

        public MeltingTile(Vector3Int pos, TileBase tile, float meltTime)
        {
            this.pos = pos;
            this.tile = tile;
            this.meltTime = meltTime;
        }

        public bool ShouldMelt()
        {
            return timeMelting >= meltTime;
        }

    };

    List<MeltedTile> meltedTiles = new();
    List<MeltingTile> meltingTiles = new();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        UpdateMeltedTiles();
    }


    void UpdateMeltingTile(Vector3Int pos)
    {
        //Using dictionary would be better for this, but this list should always be small so this is fine
        MeltingTile meltingTile = new();
        bool found = false;
        foreach (var tile in meltingTiles)
        {
            if (tile.pos == pos)
            {
                meltingTile = tile;
                found = true;
                break;
            }
        }
        if (!found) return;

        meltingTile.timeMelting += Time.deltaTime;

        if (meltingTile.ShouldMelt())
        {
            DoMelt(meltingTile.pos);
            meltingTiles.Remove(meltingTile);
        }
    }


    void RemoveMeltingTile(Collision2D col)
    {
        var pos = GetPos(col);

        //Using a dictionary would be better for this, but this list should always be small so this is fine
        MeltingTile meltingTile = new();
        bool found = false;
        foreach (var tile in meltingTiles)
        {
            if (tile.pos == pos)
            {
                meltingTile = tile;
                found = true;
                break;
            }
        }
        if (!found) return;

        meltingTiles.Remove(meltingTile);
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


    public void OnCollisionStay2D(Collision2D col)
    {
        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            //UpdateMeltingTile(col);
            if (player.mode == Mode.FIRE_MODE)
            {
                MeltBelow(col); 
            }

        }
    }


    public void OnCollisionExit2D(Collision2D col)
    {
        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            RemoveMeltingTile(col);
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
                MeltBelow(col); 
            }
        }
    }


    public Vector3Int GetPos(Collision2D col)
    {
        if (col.contactCount<1)
            return new Vector3Int(-100000000, 10000000, 10);

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


    public int FindMeltingTile(Vector3Int pos)
    {
        for(int i=0; i<meltingTiles.Count; i++)
        {
            var tile = meltingTiles[i];
            if (tile.pos==pos)
                return i;
        }
        return -1;
    }



    public void MeltShot(Collision2D col)
    {
        // Get tile, destroy it if it exists in the tilemap
        Vector3Int tilePos = GetPos(col);
        DoMelt(tilePos);
    }


    public void MeltBelow(Collision2D col)
    {
        // Do not melt spikes
        if (col.gameObject.TryGetComponent<SpikeTiles>(out _))
            return;

        // Get tile, destroy it if it exists in the tilemap
        Vector3 collisionPos = col.GetContact(0).point;
        Vector3Int tilePos = tilemap.WorldToCell(collisionPos) + Vector3Int.down;

        for (int i=-1; i<2; i++)
        {
            Vector3Int offset = new Vector3Int(i, 0, 0);

            //DoMelt(tilePos + offset);
            if (FindMeltingTile(tilePos + offset)!=-1)
                UpdateMeltingTile(tilePos+offset);
            else
                AddMeltingTile(tilePos + offset);
        }

    }


    public void AddMeltingTile(Vector3Int tilePos)
    {
        if (tilemap.HasTile(tilePos))
            meltingTiles.Add(new MeltingTile( tilePos, tilemap.GetTile(tilePos), meltTime) );
    }



    public void DoMelt(Vector3Int tilePos)
    {
        meltedTiles.Add(new MeltedTile( tilePos, tilemap.GetTile(tilePos), regenTime) );

        tilemap.SetTile(tilePos, null);
        tilemap.RefreshTile(tilePos);

        //TODO: Animation
    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
