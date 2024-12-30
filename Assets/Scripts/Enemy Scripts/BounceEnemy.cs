using System;
using UnityEngine;

public class BounceEnemy : MonoBehaviour
{
    public Fluffy fluffy;

    public void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log("hit = " + collision.gameObject.layer.ToString());
        if (collision.gameObject.layer == 0)
        {
            Debug.Log("hithard");
            fluffy.FlipFluffy();
        }
    }

}
