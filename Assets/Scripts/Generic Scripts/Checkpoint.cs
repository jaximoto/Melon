using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    Animator animator;
    public Sprite checkpointedSprite;
    bool reached;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reached = false;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter2D(Collider2D col)
    {
        PlayerMovement player;
        if (!reached && col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            Debug.Log("Checkpoint");
            player.SetCheckpoint(this);
            reached = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = checkpointedSprite;
            animator.SetTrigger(RaiseKey);
        }
    }
    private static readonly int RaiseKey = Animator.StringToHash("raise");

}
