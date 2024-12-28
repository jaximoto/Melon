

using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    
    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [SerializeField]
    private RuntimeAnimatorController _iceAnim;

    [SerializeField]
    private AnimatorOverrideController _fireAnim;



    [SerializeField] private Sprite _iceSprite;
    [SerializeField] private Sprite _fireSprite;

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
        _player.Switch += OnSwitch;

        
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

        else if (_grounded && _player.FrameInput.x == 0f)
            _anim.ResetTrigger(WalkKey);
        
    }

    private void HandleSpriteFlip()
    {
        if (_player.FrameInput.x != 0) _sprite.flipX = _player.FrameInput.x > 0;
    }

    

    private void OnSwitch(Mode mode)
    {
        Debug.Log("Switch was called and mode is:" + mode);
        if (mode == Mode.FIRE_MODE)
        {
            _sprite.sprite = _fireSprite;
            _anim.runtimeAnimatorController = _fireAnim;
            
        }

        else
        {
            _sprite.sprite = _iceSprite;
            _anim.runtimeAnimatorController = _iceAnim;
            
        }
            
    }

    private void OnWalk()
    {
        
        _anim.SetTrigger(WalkKey);
    }
    private void OnJumped()
    {
        _anim.SetTrigger(JumpKey);
        _anim.ResetTrigger(GroundedKey);
        _anim.ResetTrigger(WalkKey);


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
        _anim.ResetTrigger(JumpKey);
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

  

   
    

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int JumpKey = Animator.StringToHash("Jump");

    // Change walk key  it isnt found TODOOO
    private static readonly int WalkKey = Animator.StringToHash("Moving");
    private static readonly int FallKey = Animator.StringToHash("AtApex");

}
