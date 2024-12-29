using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class FreezableTiles : MonoBehaviour
{
    public Tilemap myTilemap;
    public Tilemap meltableTilemap;

    public Tile meltableTile;

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
            if (b.shotType == Bullet.ShotType.ICE_SHOT)
            {
                // Melt platform
                Freeze(col);
            }
            Destroy(b.gameObject);
        }
    }


    public Vector3Int GetPos(Collision2D col)
    {
        //Ass
        Tilemap tilemap = myTilemap;

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


    public List<Vector3Int> PropogateFreeze(Vector3Int nexus)
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

        int tilesFrozen = 0;
        int maxTilesFrozen = 10;

        while (s.Count > 0 && tilesFrozen < maxTilesFrozen)
        {
            // Check plus shape around nexus for more tiles
            var curr = s.Pop();
            result.Add(curr);
            Debug.Log(curr);
            tilesFrozen++;
            
            for (int i=0; i<4; i++)
            {
                if (tilesFrozen > maxTilesFrozen)
                    break;

                Vector3Int currPos = curr + new Vector3Int(offsetArr[i].Item1, offsetArr[i].Item2, 0);

                if (result.Contains(currPos))
                    continue;

                if (myTilemap.HasTile(currPos))
                {
                    s.Push(currPos);
                }
            }
        }
        
        return result;
    }


    public void Freeze(Collision2D col)
    {
        Vector3Int tilePos = GetPos(col);

        var tilePositions = PropogateFreeze(tilePos);

        foreach(Vector3Int pos in tilePositions)
        {
            // Add to meltable tilemap
            meltableTilemap.SetTile(pos, meltableTile);

            // Remove from my tilemap
            myTilemap.SetTile(pos, null);
        }
        
    }
}
