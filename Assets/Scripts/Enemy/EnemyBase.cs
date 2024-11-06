using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(Controller2D))]
public class EnemyBase : MonoBehaviour
{
    [SerializeField] 
    private EnemyInfo enemyInfo;
    
    [HideInInspector] 
    public SkeletonAnimation skeletonAnimation;

    // Variables
    [HideInInspector] 
    public float enemyHealth;
    
    [HideInInspector] 
    public float enemyStamina;

    [HideInInspector] 
    public float gravityStrength;
    
    [HideInInspector] 
    public float gravityScale;


    public float jumpHeight; 
    public float jumpTimeToApex;
    public float minJumpHeight;
    [HideInInspector] public float jumpForce; 

    // Movement
    float gravity;
    Vector3 velocity;
    float velocityXSmoothing;
    float lerpAmount = 1;
    float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;


    // Bools
    bool isBeingLaunched;

    // Scripts
    Controller2D controller;
    
    private void OnValidate()
    {
        gravityStrength = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToApex, 2);
        gravityScale = gravityStrength;
    }
    
    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        controller = GetComponent<Controller2D>();
    }
    
    private void Start()
    {
        enemyHealth = enemyInfo.enemyMaxHealth;
        enemyStamina = enemyInfo.enemyMaxStamina;

        SetGravityScale(gravityScale);

        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }
    
    private void Update()
    {
        CalculateVelocity();
    }
    
    #region Movement
    private void CalculateVelocity(int direction = 1)
    {
        float targetVelocityX = 0;

        targetVelocityX = Mathf.Lerp(velocity.x, targetVelocityX, lerpAmount);

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    public void SetGravityScale(float scale)
    {
        gravity = scale;
    }
    #endregion

    public void GetHurt(float damagePower, float launchStartSpeed, float launchEndSpeed, int direction, float launchAttackTime, float launchEndTime)
    {
        enemyHealth -= (damagePower - enemyInfo.defencePower);

        StartCoroutine(LaunchEnemy(launchStartSpeed, launchEndSpeed, direction, launchAttackTime, launchEndTime));

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator LaunchEnemy(float launchStartSpeed, float launchEndSpeed, int direction, float launchAttackTime, float launchEndTime)
    {
        isBeingLaunched = true;
        float startTime = Time.time;

        while (Time.time - startTime <= launchAttackTime)
        {
            velocity.x = direction * launchStartSpeed;
            yield return null;
        }
        
        startTime = Time.time;

        velocity.x = launchEndSpeed * direction;

        while (Time.time - startTime <= launchEndTime)
        {
            yield return null;
        }

        isBeingLaunched = false;
    }

    public IEnumerator FreezeAnimationFrame(float animationFreezeAmount, float durationAmount)
    {
        skeletonAnimation.timeScale = animationFreezeAmount;
        yield return new WaitForSeconds(durationAmount);
        skeletonAnimation.timeScale = 1;
    }
}