using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator _anim;
    public Sprite checkpointedSprite;
    bool reached;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reached = false;
        _anim = GetComponent<Animator>();
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
            _anim.SetTrigger(FireRaiseKey);
        }
    }

    private readonly int FireRaiseKey = Animator.StringToHash("FlagRaised");
}
