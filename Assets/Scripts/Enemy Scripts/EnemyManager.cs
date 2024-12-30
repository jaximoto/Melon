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
        foreach(KeyValuePair<GameObject, Vector3> kvp in enemyPositions)
        {
            kvp.Key.gameObject.SetActive(true);
            kvp.Key.transform.position = kvp.Value;
        }
    }


    public void HandleEnemyDeath(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        destroyed[pos] = obj;
    }


}
