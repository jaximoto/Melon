using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Mode
{
    ICE_MODE,
    FIRE_MODE
};

public class PlayerMovement : MonoBehaviour, IPlayerController, IShootable
{

    [SerializeField] private PlayerScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    public Animator _animator;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    public bool dead = false;
   
    public Mode mode;

    #region Interface
    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public event Action JumpApex;
    public event Action Falling;
    public event Action Shot;
    public event Action Death;
    public event Action AfterDeath;
    public event Action OnShoot;


    public event Action<Mode> Switch;
    public bool takingInput = true;
    #endregion

    private float _time;

    private Checkpoint checkpoint;

    public TilemapCollider2D oneWayCollider;
    
    private void Awake()
    {
        //oneWayCollider.excludeLayers.
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), oneWayCollider);
        //Physics2D.IgnoreLayerCollision(3, 10);
        
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;

        mode = Mode.ICE_MODE;
    }


    // Update is called once per frame
    void Update()
    {
        
        _time += Time.deltaTime;
        if (_time > .5f && takingInput)
        {
            GatherInput();

            HandleSwitch();

            HandleShot();
        }
       

        //GoingUp();
    }
    public void GoingUp()
    {
        //If y is increasing, dissallow upward movement
        if (_frameVelocity.y > 0 && !_grounded)
        {
            Debug.Log("going up");
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), oneWayCollider);
        }
        Debug.Log("Collisions between player and oneway: " + Physics2D.GetIgnoreLayerCollision(3, 10));
        /*
        else 
        {
            Physics.IgnoreLayerCollision(3, 10, false);
        } 
        */
    }

    public void HandleDeath()
    {
        // Move to last checkpoint
        if (!dead)
        {
            Death?.Invoke();
            StartCoroutine(WaitForIdleThenTeleport());
        }
       
        // And move the camera
    }

    private IEnumerator WaitForIdleThenTeleport() { 
    
        dead = true;
        takingInput = false;
        // Get the Animator's current state info
        AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
        float startTime = 0f;
        float endTime = .67f;
        // Wait until the animator's current state matches Idle
        while (startTime < endTime)
        {
            startTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        AfterDeath?.Invoke();
        gameObject.transform.position = checkpoint.gameObject.transform.position;
        takingInput = true;
        dead = false;
        //Debug.Log("Animation is now idle. Continuing...");
        // Place the code to execute after idle state here
    }

    public void SetCheckpoint(Checkpoint c)
    {
        this.checkpoint = c;
    }


    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            SwitchHeld = Input.GetMouseButtonDown(1),
            ShotHeld = Input.GetMouseButtonDown(0),
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }


    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    # region Switch


    private void HandleSwitch()
    {
        if (_frameInput.SwitchHeld)
            ExecuteSwitch();
    }


    private void ExecuteSwitch()
    {
        Debug.Log("Executing switch");
        if (mode==Mode.ICE_MODE)
            mode = Mode.FIRE_MODE;
        else
            mode = Mode.ICE_MODE;
        Switch?.Invoke(mode);
        Debug.Log("Invoked switch");
    }

    #endregion

    #region Collisions

    public LayerMask coyoteIgnoreLayers; // Public LayerMask to select layers to ignore
    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(
            _col.bounds.center,
            _col.size,
            _col.direction,
            0,
            Vector2.down,
             _stats.GrounderDistance
            //~_stats.PlayerLayer & ~(1 << LayerMask.NameToLayer("OneWay"))
        );

        bool ceilingHit = Physics2D.CapsuleCast(
            _col.bounds.center,
            _col.size,
            _col.direction,
            0,
            Vector2.up,
            _stats.GrounderDistance,
            //~_stats.PlayerLayer & ~(1 << LayerMask.NameToLayer("OneWay")) & ~(1 << LayerMask.NameToLayer("Enemy"))
            ~coyoteIgnoreLayers.value // Invert the LayerMask to ignore selected layers
        );

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }


    #endregion

    #region Shot

    private void HandleShot()
    {
        if (_frameInput.ShotHeld)
        {
            Shot?.Invoke();
            OnShoot?.Invoke();
        }
            
        else
            return;
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            else if(_frameVelocity.y < 0) Falling?.Invoke();
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            
        }
    }

    #endregion


    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif


    public void OnShat(Bullet b)
    {
        if (b.shotType == Bullet.ShotType.ICE_SHOT)
        {
            if (mode == Mode.FIRE_MODE)
                HandleDeath();
        }
        else if (b.shotType == Bullet.ShotType.FIRE_SHOT)
        {
            if (mode == Mode.ICE_MODE)
                HandleDeath();
        }
    }

}


public struct FrameInput
{
    public bool SwitchHeld;
    public bool ShotHeld;
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}


public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;

    public event Action JumpApex;

    public event Action Falling;

    public event Action Death;
    public event Action AfterDeath;
    public event Action OnShoot;
    public Vector2 FrameInput { get; }

    public event Action Shot;
    public event Action<Mode> Switch;
}

