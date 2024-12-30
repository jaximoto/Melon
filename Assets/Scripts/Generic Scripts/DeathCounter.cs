using System;
using TMPro;

using UnityEngine;

public class DeathCounter : MonoBehaviour
{
    public PlayerMovement player;
    public int counter;
    public TMP_Text text;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player = GameObject.Find("PlayerMovement").GetComponent<PlayerMovement>();
        player.Death += Increment;
        counter = 0;
        text.text = "0";
    }


    // Update is called once per frame
    void Update()
    {
        Render();
    }


    void Increment()
    {
        counter += 1;
    }


    void Render()
    {
        text.text = $"{counter}";
    }

}
