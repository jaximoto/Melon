using System;
using System.Collections.Generic;

using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerMovement player;

    public Dictionary<Vector3, GameObject> destroyed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyed = new();

        player.Death += RespawnEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void RespawnEnemies()
    {
        foreach(KeyValuePair<Vector3, GameObject> kvp in destroyed)
        {
            Instantiate(kvp.Value, kvp.Key, Quaternion.identity);
            destroyed.Remove(kvp.Key);
        }
    }


    void HandleEnemyDeath(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        destroyed[pos] = obj;
    }


}
