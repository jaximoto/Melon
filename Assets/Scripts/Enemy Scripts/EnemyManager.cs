using System;
using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerMovement player;

    public Dictionary<GameObject, Vector3> enemyPositions;

    public Dictionary<Vector3, GameObject> destroyed;

    public Enemy fluffyPrefab, fireElementalPrefab, iceElementalPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Debug.Log("Awake");
        destroyed = new();
        enemyPositions = new();

        player.Death += RespawnEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void RespawnEnemies()
    {
        List<Vector3> toDestroy = new();
        Debug.Log($"SIZE OF DEST: {destroyed.Count}");
        foreach(KeyValuePair<Vector3, GameObject> kvp in destroyed)
        {
            Debug.Log($"NEW: {enemyPositions[kvp.Value]}");
            kvp.Value.transform.position = enemyPositions[kvp.Value];
            toDestroy.Add(kvp.Key);

            var oldPos = enemyPositions[kvp.Value];

            if (kvp.Value.TryGetComponent<Fluffy>(out _))
            {
                var newEnemy = Instantiate(fluffyPrefab, oldPos, Quaternion.identity);
                newEnemy.transform.position = oldPos;
            }
            else {
                var elemental = kvp.Value.GetComponent<Elemental>();
                if (elemental.myState == Enemy.EnemyState.Fire)
                {
                    var newEnemy = Instantiate(fireElementalPrefab, oldPos, Quaternion.identity);
                    newEnemy.transform.position = oldPos;
                }
                else if (elemental.myState == Enemy.EnemyState.Frozen)
                {
                    var newEnemy = Instantiate(iceElementalPrefab, oldPos, Quaternion.identity);
                    newEnemy.transform.position = oldPos;
                }
            }
            Destroy(kvp.Value);
        }

        foreach(var key in toDestroy)
        {
            destroyed.Remove(key);
        }

    }


    public void HandleEnemyDeath(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        destroyed[pos] = obj;
    }


}
