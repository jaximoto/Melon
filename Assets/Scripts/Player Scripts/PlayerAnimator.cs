

using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    [Header("References")]
    [SerializeField]
    private Animator _anim;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip[] _footsteps;

    private AudioSource _source;
    [SerializeField]
    private RuntimeAnimatorController _iceAnim;

    [SerializeField]
    private AnimatorOverrideController _fireAnim;

    public AudioSource shootSource;
    public AudioClip shoot;


    [SerializeField] private Sprite _iceSprite;
    [SerializeField] private Sprite _fireSprite;

    [SerializeField] private SpriteRenderer _sprite;

    private IPlayerController _player;
    private bool _grounded;


    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.volume = GlobalAudio.Instance.GlobalVolume;
        shootSource.volume = GlobalAudio.Instance.GlobalVolume;
        shootSource.time = 1.5f;
        shootSource.clip = shoot;
        _player = GetComponentInParent<IPlayerController>();
        if (_player == null) Debug.Log("Player is null for anim");
    }

    private void OnEnable()
    {
        _player.Jumped += OnJumped;
        _player.GroundedChanged += OnGroundedChanged;
        _player.Falling += OnFall;
        _player.Switch += OnSwitch;
        _player.Death += OnDeath;
        _player.AfterDeath += AfterDeath;
        _player.OnShoot += OnShoot;


    }
    public void OnShoot()
    {
        shootSource.Play();
    }
    public void OnDeath()
    {
        _anim.SetTrigger(DeathKey);
    }

    public void AfterDeath()
    {
        _anim.ResetTrigger(DeathKey);
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
        _source.Play();
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

    public int[] soundTimes = { 0, 3, 6 };
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
    private static readonly int DeathKey = Animator.StringToHash("Death");



}
