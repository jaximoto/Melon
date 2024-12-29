using UnityEngine;
public class Detection : MonoBehaviour
{
    public Enemy enemy;
    Fluffy fluffy;
    Elemental elemental;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("hit");
        if (enemy.TryGetComponent<Fluffy>(out fluffy))
        {
            fluffy = enemy.GetComponent<Fluffy>();
            fluffy.LookAtPlayer(collider);
        }
        if (enemy.TryGetComponent<Elemental>(out elemental))
        {
            Debug.Log("elemental saw him");
            elemental = enemy.GetComponent<Elemental>();
            elemental.LookAtPlayer(collider);
        }
    }
}
