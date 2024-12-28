using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField] private SpriteRenderer _sprite;

    
    private IPlayerController _player;
    private bool _grounded;
    

    private void Awake()
    {
        
        _player = GetComponentInParent<IPlayerController>();
        if (_player == null) Debug.Log("Player is null for anim");
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _player.Falling += OnFall;

        
    }

    private void OnDisable()
    {
        _player.Jumped -= OnJumped;
        _player.GroundedChanged -= OnGroundedChanged;

       
    }

    private void Update()
    {
        if (_player == null) return;

        
        HandleSpriteFlip();

        if (_grounded && _player.FrameInput.x != 0f)
        {
            OnWalk();
        }
        
    }

    private void HandleSpriteFlip()
    {
        if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x > 0;
    }

    

    

    private void OnWalk()
    {
        
        _anim.SetTrigger(WalkKey);
    }
    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);
        _anim.ResetTrigger(GroundedKey);


        if (_grounded) // Avoid coyote
        {
           
        }
    }

    private void OnFall()
    {
        _anim.ResetTrigger(JumpKey);
        _anim.SetTrigger(FallKey);
    }

    private void OnGroundedChanged(bool grounded, float impact)
    {
        
        _anim.ResetTrigger(FallKey);
        _grounded = grounded;
        //_anim.SetTrigger(GroundedKey);
        

        if (grounded)
        {
            //DetectGroundColor();
            //SetColor(_landParticles);

            _anim.SetTrigger(GroundedKey);
            //_source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            //_moveParticles.Play();

            //_landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
            //_landParticles.Play();
        }
        else
        {
            //_moveParticles.Stop();
        }
    }

    private void DetectGroundColor()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

        if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r)) return;
        var color = r.color;
       
    }

    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
       
    }

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
    private static readonly int WalkKey = Animator.StringToHash("Walk");
    private static readonly int FallKey = Animator.StringToHash("Fall");

}
