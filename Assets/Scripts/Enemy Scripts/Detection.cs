using UnityEngine;
public class Detection : MonoBehaviour
{
    public Enemy enemy;
    Fluffy fluffy;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("hit");
        if (enemy.TryGetComponent<Fluffy>(out fluffy))
        {
            fluffy = enemy.GetComponent<Fluffy>();
            fluffy.LookAtPlayer(collider);
        }
    }
}
