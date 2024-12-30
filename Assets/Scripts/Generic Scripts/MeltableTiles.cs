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
        public Quaternion rot{get; set;}
        public TileBase tile {get; set;}

        public MeltedTile(Vector3Int pos, TileBase tile, float regenTime, Quaternion rot)
        {
            this.pos = pos;
            this.tile = tile;
            this.regenTime = regenTime;
            this.rot = rot;
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
        UpdateMeltingTiles();
    }


    void UpdateMeltingTiles()
    {
        List<MeltingTile> toDelete = new();
        for(int i=0; i<meltingTiles.Count; i++)
        {
            var meltingTile = meltingTiles[i];

            meltingTile.timeMelting += Time.deltaTime;

            if (meltingTile.ShouldMelt())
            {
                //tilemap.SetTile(meltingTile.pos, meltingTile.tile);
                DoMelt(meltingTile.pos);
                toDelete.Add(meltingTile);
            }
        }

        foreach(var meltingTile in toDelete)
        {
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
                Color color = tilemap.GetColor(meltedTile.pos);
                color.a = 1.0f;
                tilemap.SetColor(meltedTile.pos, color);
                SetTileRotation(tilemap, meltedTile.pos, meltedTile.rot);
                toDelete.Add(meltedTile);
            }
        }

        foreach(var meltedTile in toDelete)
        {
            meltedTiles.Remove(meltedTile);
        }
    }



    public void OnCollisionExit2D(Collision2D col)
    {
        /*
        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            RemoveMeltingTile(col);
        }
        */
    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        Bullet b;
        if (col.gameObject.TryGetComponent<Bullet>(out b))
        {
            if (b.shotType == Bullet.ShotType.FIRE_SHOT && b.gameObject.layer != 9) // Enemy bullet layer 9
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


    public void OnCollisionStay2D(Collision2D col)
    {
        PlayerMovement p;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out p))
        {
            if (p.mode == Mode.FIRE_MODE)
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
        DoMeltPropogate(tilePos);
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
            if (FindMeltingTile(tilePos + offset)==-1)
                AddMeltingTile(tilePos + offset);
        }

    }


	// Get the rotation of a tile
    public Quaternion GetTileRotation(Tilemap tilemap, Vector3Int position)
    {
        Matrix4x4 tileMatrix = tilemap.GetTransformMatrix(position);
        return Quaternion.LookRotation(tileMatrix.GetColumn(2), tileMatrix.GetColumn(1));
    }


    // Set the rotation of a tile
    public void SetTileRotation(Tilemap tilemap, Vector3Int position, Quaternion rotation)
    {
        Matrix4x4 currentMatrix = tilemap.GetTransformMatrix(position);
        Matrix4x4 newMatrix = Matrix4x4.TRS(currentMatrix.GetColumn(3), rotation, currentMatrix.lossyScale);
        tilemap.SetTransformMatrix(position, newMatrix);
    }


    public void AddMeltingTile(Vector3Int tilePos)
    {
        if (tilemap.HasTile(tilePos))
        {
            var color = tilemap.GetColor(tilePos);
            color.a = 0.5f;
            tilemap.SetTileFlags(tilePos, TileFlags.None);
            tilemap.SetColor(tilePos, color);
            tilemap.RefreshTile(tilePos);

            //meltingTiles.Add(new MeltingTile( tilePos, tilemap.GetTile(tilePos), meltTime, GetTileRotation(tilemap, tilePos)) );
            meltingTiles.Add(new MeltingTile( tilePos, tilemap.GetTile(tilePos), meltTime) );
        }
    }


    public List<Vector3Int> PropogateMelt(Vector3Int nexus)
    {

        List<Vector3Int> result = new();

        var offsetArr = new (int, int)[]
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1)
        };

        Stack<Vector3Int> s = new();
        s.Push(nexus);

        int tilesMelted = 0;
        int maxTilesMelted = 5;

        while (s.Count > 0 && tilesMelted < maxTilesMelted)
        {
            // Check plus shape around nexus for more tiles
            var curr = s.Pop();
            result.Add(curr);
            tilesMelted++;
            
            for (int i=0; i<4; i++)
            {
                if (tilesMelted > maxTilesMelted)
                    break;

                Vector3Int currPos = curr + new Vector3Int(offsetArr[i].Item1, offsetArr[i].Item2, 0);

                if (result.Contains(currPos))
                    continue;

                if (tilemap.HasTile(currPos))
                {
                    s.Push(currPos);
                }
            }
        }
        
        return result;
    }


    public void DoMeltPropogate(Vector3Int nexus)
    {
        var meltingTiles = PropogateMelt(nexus);
        foreach (var tile in meltingTiles)
        {
            DoMelt(tile);
        }
    }


    public void DoMelt(Vector3Int tilePos)
    {
        TileBase t = tilemap.GetTile(tilePos);

        // Only regen platforms and spikes
        if (t.name.Contains("Floating") || t.name.Contains("Stalagmite") || t.name.Contains("Falling"))
        {
            meltedTiles.Add(new MeltedTile( tilePos, tilemap.GetTile(tilePos), regenTime, GetTileRotation(tilemap, tilePos)) );
        }
            //meltingTiles.Add(new MeltingTile( tilePos, tilemap.GetTile(tilePos), meltTime, GetTileRotation(tilemap, tilePos)) );
        else
        {
            // Propogate melt
        }

        tilemap.SetTile(tilePos, null);
        tilemap.RefreshTile(tilePos);

        //TODO: Animation
    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
