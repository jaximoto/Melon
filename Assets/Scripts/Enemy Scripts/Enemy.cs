
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Animations;
using AnimatorControllerParameter = UnityEngine.AnimatorControllerParameter;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

public class Enemy : Shooter, IShootable
{
    public float moveSpeed;
    public int health;
    public int flipper;
    public int lastFlipper; 
    public bool aggroed, dying, celebrating, spawnedOnce;
    public EnemyManager enemyManager;

    private Rigidbody2D rigidBody;
    public Animator animator;
    public List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>();

    private Transform lastTransform;
    
    public enum EnemyState
    {
        Frozen,
        Fire,
        Default
    };

    
    public EnemyState myState;
    public DefaultDirection lastDirection;
    public void OnEnable()
    {
        if (spawnedOnce)
        {
            transform.localScale = lastTransform.localScale;
        }
        lastTransform = transform;
        spawnedOnce = true;


        flipper = lastFlipper * -1;
        rigidBody = GetComponent<Rigidbody2D>();	
        animator = GetComponent<Animator>();
        aggroed = false;
        Debug.Log("got here");
        dying = false;
        celebrating = false;
        if (gameObject.TryGetComponent<Elemental>(out _))
            rigidBody.bodyType = RigidbodyType2D.Static;
        else
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
        ResetAllTriggers(animator);
        animator.SetTrigger(ResetKey);
        animator.ResetTrigger(ResetKey);

    }


    public void ResetAllTriggers(Animator animator)
    {

        /*for (int i = 0; i < animator.parameterCount; i++)
		{
            Debug.Log("param " + parameters[i].ToString());
			if (parameters[i].type == AnimatorControllerParameterType.Bool)
			{

                Debug.Log("GOING ");
                animator.ResetTrigger(parameters[i].ToString());
			}
		}    
        */

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {

                animator.SetBool(parameter.name, false);
            }
        }
    }


    public override void Start()
    {
        
        Debug.Log($"OLD: {this.gameObject.transform.position}");
        base.Start();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        Debug.Log("Called find");

        enemyManager.enemyPositions[this.gameObject] = new Vector3(this.gameObject.transform.position.x,    
                                                                    this.gameObject.transform.position.y,
																		0.0f);

        lastTransform = transform;
    }



    public virtual void OnShat(Bullet b)
    {
        enemyManager.HandleEnemyDeath(this.gameObject);
    }


    public virtual void EnemyBehaviour()
    {

    }


    public void LateUpdate()
    {
        timeSinceLastShot += Time.deltaTime;
    }


    public void OnCollisionEnter2D(Collision2D col)
    {
        PlayerMovement player;
        if (col.gameObject.TryGetComponent<PlayerMovement>(out player))
        {
            HandlePlayerCollision(player);
        }
    }


    public void HandlePlayerCollision(PlayerMovement player)
    {
        if (myState == EnemyState.Default)
        {
            player.HandleDeath();
        }
        else if (myState == EnemyState.Frozen)
        {
            if (player.mode == Mode.FIRE_MODE)
                player.HandleDeath();
        }
        else if (myState == EnemyState.Fire)
        {
            if (player.mode == Mode.ICE_MODE)
                player.HandleDeath();
        }
        HandleCollisionAnimation();

    }

    public virtual void HandleCollisionAnimation()
    {

    }

    private static readonly int ResetKey = Animator.StringToHash("Reset");

}
