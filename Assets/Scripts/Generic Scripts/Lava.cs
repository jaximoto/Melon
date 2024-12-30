using UnityEngine;

public class Lava : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D col)
    {
        PlayerMovement p;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out p))
        {
            if (p.mode == Mode.ICE_MODE)
            {
                p.HandleDeath();
            }
        }

    }
}
