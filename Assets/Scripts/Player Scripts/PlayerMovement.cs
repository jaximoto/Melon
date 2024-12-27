using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private PlayerScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
