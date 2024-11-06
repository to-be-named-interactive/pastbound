using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour 
{
	[Header("Gravity")]
	[HideInInspector] 
	public float gravityStrength; // Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] 
	public float gravityScale; // Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
	[Space(5)]
	public float fallGravityMultiplier; // Multiplier to the player's gravityScale when falling.
	public float maxFallSpeed; // Maximum fall speed (terminal velocity) of the player when falling.
	
	[Space(5)]

	[Header("Run")]
	public float runMaxSpeed; // Target speed we want the player to reach.
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .1f;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; // Height of the player's jump
	public float jumpTimeToApex; // Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	public float minJumpHeight; // Minimum height of the jump 
	
	[HideInInspector] 
	public float jumpForce; // The actual force applied (upwards) to the player when they jump.
	public float jumpCutGravityMult; // Multiplier to increase gravity if the player reaches jump apex

	[Space(0.5f)]
	
    [Space(20)]
	[Header("Roll")]
	public int rollAmount;
	public float rollSpeed;

	[Space(5)]
	public float rollAttackTime;
	[Space(5)]
	public float rollEndTime; // Time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
	public Vector2 rollEndSpeed; // Slows down player, makes dash feel more responsive (used in Celeste)

	[Space(5)]
	public float rollRefillTime;
	[Space(5)]

	[Range(0.01f, 0.5f)] 
	public float rollInputBufferTime;

	[Space(10)]

	[Header("Combat")]
	[SerializeField] 
	private float nextAttackTimeWindow;

	[Space(10)]

	[Header("Assists")]
	[Range(0.01f, 0.5f)] 
	public float coyoteTime; // Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] 
	public float jumpInputBufferTime; // Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    [Space(20)]

	[Header("Properties")]
	[SerializeField] 
    Vector2 hurtLaunchPower; // How much force should be applied to the player when getting hurt?
	private float launch; // The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
	[SerializeField] 
	private float launchRecovery; //How slow should recovering from the launch be? (Higher the number, the longer the launch will last)

	[Space(20)]

	[Header("References")]
	[SerializeField] public float SC_RaycastMaxDistance;
	[SerializeField] LayerMask layerMask;

	[Space(20)]

	[Header("Sounds")]
	[SerializeField] public AudioClip[] grassFootstepSounds;
	
	// Private Variables
	[HideInInspector]
	public bool IsJumping { get; private set; } // Bool when is player jumping
	[HideInInspector] 
	public float LastPressedJumpTime { get; private set; } // Time when the jump button was last presed
	[HideInInspector] 
	public float LastPressedRollTime { get; private set; }

	bool isJumpKeyUp; // bool to check when is Jumop button up/ released 
	private bool _isJumpCut; // Bool to Indicate when to Increase gravity

	// Rolling
	private int _rollesLeft;
	private bool _rollRefilling;
	private Vector2 _lastRollDir;
	private bool _isRollAttacking;
	[HideInInspector] 
	public bool IsRolling { get; private set; }

	// Timers
	[HideInInspector] 
	public float LastOnGroundTime { get; private set; }
	[HideInInspector] 
	public float JustLandedTime { get; private set; }
	
	// Movement
	float gravity;
	[HideInInspector] 
	public Vector3 velocity;
	float velocityXSmoothing;
	[HideInInspector] 
	public Vector2 input;
	[HideInInspector] 
	private bool freezePlayer;
	[HideInInspector] 
	public int direction;
    bool isLastOnGroundTimeSmall;

	// Combat
	[HideInInspector] 
	public bool isAttacking;
	bool playNextAttack;

	// AnimatorFunctions EventDatas
	EventData isEventAttack;
	EventData isEventAttackEnd;
	EventData isEventFootStep;
	EventData isEventUlfSumomingStartEnd;
	EventData isEventNextAttackWindow;

	// Variables for animatorFunctions
	private const string ATTACK = "AttackHit";
	private const string NEXT_ATTACK_WINDOW = "NextAttackTimeWindow";
	private const string ATTACK_END = "AttackEnd";
	private const string ULF_SUMMONING_START_END = "UlfSumomingStartEnd";

	private const string RUN = "Traversing/Run";
	private const string IDLE = "Traversing/Idle";
	private const string JUMP = "Traversing/Jump";
	private const string FALL = "Traversing/Fall";
	private const string LAND = "Traversing/Land";
	private const string ROLL = "Traversing/Roll";

	private const string FOOTSTEP = "FootStep";
	
	// Scripts
	Controller2D controller;
	[HideInInspector] 
	public AnimationFunctions animatorFunctions;
	InputHandler inputHandler;
	Attack attackHandler;
	AudioPlayer audioPlayer;

	// Dialouge triggering
	[HideInInspector] 
	public DialogueTrigger dialogueTrigger;
	
	// Screenbox detection
	GameObject previousScreenBox;
	[HideInInspector] 
	public GameObject nextScreenBox;
	GameObject sc;
	PolygonCollider2D nextScreenBoxCollider;
	
	// We need this for Camera Controler script...
	[HideInInspector] public float screenBoxEntireWidth;
	
    #region Instance
    // Singleton Instance
    private static Player instance;

	public static Player Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<Player>();
			}
			return instance;
		}
	}
    #endregion
    
    private void OnValidate()
    {
		gravityStrength = -(2 * jumpHeight) / Mathf.Pow(jumpTimeToApex, 2);
		gravityScale = gravityStrength;
	}
    
    private void Awake()
    {
		controller = GetComponent<Controller2D>();
		animatorFunctions = GetComponent<AnimationFunctions>();
		attackHandler = GetComponent<Attack>();
		inputHandler = GetComponent<InputHandler>();
		attackHandler = GetComponent<Attack>();
		audioPlayer = GetComponent<AudioPlayer>();
	}
    
    void Start() 
    {
		// Calulate Gravity
		SetGravityScale(gravityScale);

		// Calculate Jump force 
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		// Set the direction "right" by default
		direction = 1;

		// Input Event subscription
		// Preformed Input
		inputHandler.OnJumpPreformed += InputHandler_OnJumpPreformed;
		inputHandler.OnRollPreformed += InputHandler_OnRollpreformed;
		inputHandler.OnInteractPreformed += InputHandler_OnInteractPreformed;
        inputHandler.OnAttackPreformed += InputHandler_OnAttackPreformed;
        
		// Cancelled Input
		inputHandler.OnJumpCanceled += InputHandler_OnJumpCanceled;
		
		// Animation Event subscription
		// skeletondata fetching
		isEventAttack = animatorFunctions.skeletonAnimation.skeleton.Data.FindEvent(ATTACK);
		isEventAttackEnd = animatorFunctions.skeletonAnimation.skeleton.Data.FindEvent(ATTACK_END);
		isEventFootStep = animatorFunctions.skeletonAnimation.skeleton.Data.FindEvent(FOOTSTEP);
		isEventUlfSumomingStartEnd = animatorFunctions.skeletonAnimation.skeleton.Data.FindEvent(ULF_SUMMONING_START_END);
		isEventNextAttackWindow = animatorFunctions.skeletonAnimation.skeleton.Data.FindEvent(NEXT_ATTACK_WINDOW);

		// Own Events
		animatorFunctions.skeletonAnimation.AnimationState.Event += AnimatorEventHandiler;
	}
    
	#region Events Subscription
	#region AnimatorFunctions Event Subscription
	private void AnimatorEventHandiler(TrackEntry trackEntry, Spine.Event e)
	{
		if (isEventAttack == e.Data)
		{
			attackHandler.PerformAttack(direction);
		}

		if(isEventUlfSumomingStartEnd == e.Data && isAttacking)
        {
			animatorFunctions.PlayAnimation(1, "Combat/UlfSummoningHold", true);
		}

		// Playing Sounds
		if(isEventFootStep == e.Data && !isAttacking)
        {
			if (grassFootstepSounds != null) audioPlayer.PlaySound(grassFootstepSounds[Random.Range(0, grassFootstepSounds.Length)], 0.2f);
        }
	}
	#endregion

	#region InputHandeler Events Subscription
	// If pressed attack
	private void InputHandler_OnAttackPreformed(object sender, System.EventArgs e)
	{
		if (playNextAttack && !freezePlayer && !IsRolling) 
		{
			int attackQueue = 1;

			attackQueue = (attackQueue <= 3) ? attackQueue + 1 : attackQueue;

			Attack(attackQueue);

			if (playNextAttack) playNextAttack = false;
		} 

		if (!isAttacking && !freezePlayer && !IsRolling) Attack();
	}

	// if pressed Interact
	private void InputHandler_OnInteractPreformed(object sender, System.EventArgs e)
	{
		if (dialogueTrigger != null && !isAttacking) dialogueTrigger.InteractWithNPC();
	}

	// is roll input performed/pressed
	private void InputHandler_OnRollpreformed(object sender, System.EventArgs e)
	{
		if (!freezePlayer) OnRollInput();
	}

	// Is Jump Input performed/pressed event
	private void InputHandler_OnJumpPreformed(object sender, System.EventArgs e)
	{
		if (!freezePlayer)
		{
			OnJumpInput();
			isJumpKeyUp = false;
		}
	}
	
	private void InputHandler_OnJumpCanceled(object sender, System.EventArgs e)
	{
		isJumpKeyUp = true;
	}

	#endregion
	#endregion
	
	void Update() 
	{
		Debug.Log("isAttacking: " + isAttacking);
		Debug.Log("playNextAttack: " + playNextAttack);
		
		// Getting input direction
		if (!freezePlayer)
		{
			input = inputHandler.inputDirection;
		}
        else
        {
			input = Vector2.zero;
        }

		// Timers
		LastOnGroundTime -= Time.deltaTime;
		LastPressedJumpTime -= Time.deltaTime;
		LastPressedRollTime -= Time.deltaTime;
		
		// Moving
		CalculateVelocity(1);
		
		FlipPlayerGraphic();
		
		// JumpChecks
		if (!IsJumping)
        {
			if (controller.collisions.below)
			{
				JustLandedTime -= Time.deltaTime;

				if (LastOnGroundTime <= -0.05f)
				{
					isLastOnGroundTimeSmall = false;
					
					if (JustLandedTime >= -0.15f)
					{
						animatorFunctions.PlayAnimation(0, LAND);
					}
                }
                else if (LastOnGroundTime <= 0f)
                {
					isLastOnGroundTimeSmall = true;
                }
				     
				LastOnGroundTime = coyoteTime;		
				
				if(JustLandedTime <= -1f)
                {
					JustLandedTime = -1f;
                }
            }
        }
        else
        {
			JustLandedTime = 0;
		}
		if (LastOnGroundTime > 0 && !IsJumping)
		{
			_isJumpCut = false;
		}
		
		// Add force if we are on top of a bouncer (a trumpoline type object that send player flying upward!)
        if (controller.collisions.isPlayerOnTopOfBouncer != null) StartCoroutine(controller.collisions.isPlayerOnTopOfBouncer.Bounce());
	}

    #region Movement And Gravity
    void CalculateVelocity(float lerpAmount)
	{
		// Lerp launch back to zero at all times
		launch += (0 - launch) * Time.deltaTime * launchRecovery;

		// Roll checks
		if (CanRoll() && LastPressedRollTime > 0)
		{
			// If not direction pressed, dash forward
			if (input != Vector2.zero)
				_lastRollDir = input;
			else
				_lastRollDir = (direction == 1) ? Vector2.right : Vector2.left;
			
			IsRolling = true;
			IsJumping = false;
			_isJumpCut = false;
			
			StartCoroutine(nameof(StartRoll), _lastRollDir);
		}
		
		if (controller.collisions.below)
		{
			if (!IsRolling && !isAttacking)
            {
				if (input.x != 0 && JustLandedTime < -0.15f || input.x != 0 && isLastOnGroundTimeSmall)
				{
					 animatorFunctions.PlayAnimation(0, RUN, true);
				}
				else if (input.x == 0 && JustLandedTime < -0.15f || input.x == 0 && isLastOnGroundTimeSmall)
				{
					 animatorFunctions.PlayAnimation(0, IDLE, true);
				}
			}
			
			IsJumping = false;
		}
		else
		{
			IsJumping = true;
			
			if (velocity.y > 0) animatorFunctions.PlayAnimation(0, JUMP);

			if (velocity.y < -13) animatorFunctions.PlayAnimation(0, FALL);
           
			if (!_isJumpCut && velocity.y < 0 && IsJumping)
			{
				_isJumpCut = true;	
			}

			if (isJumpKeyUp)
			{
				if (!_isJumpCut && velocity.y < minJumpHeight && IsJumping)
				{
					_isJumpCut = true;
				}
			}
		}
		
		// Jump
		if (CanJump() && LastPressedJumpTime > 0)
		{
			IsJumping = true;
			Jump();

		}

		// Gravity
		if (!_isRollAttacking)
		{
			if (_isJumpCut)
			{
				SetGravityScale(gravityScale * jumpCutGravityMult);
				velocity = new Vector2(velocity.x, Mathf.Max(velocity.y, -maxFallSpeed));
			}
			else
			{
				SetGravityScale(gravityScale);
			}
		}
		
		var runSpeed = (isAttacking) ? 0 : runMaxSpeed;

		float targetVelocityX = input.x * runSpeed + launch;

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
    
    // Flipping character graphic when moving on the opposite direction 
    void FlipPlayerGraphic()
	{
		if (input.x > 0 && !isAttacking && !IsRolling)
		{
			animatorFunctions.skeletonAnimation.skeleton.ScaleX = 1;
			direction = 1;
		}
		else if (input.x < 0 && !isAttacking && !IsRolling)
		{
			animatorFunctions.skeletonAnimation.skeleton.ScaleX = -1;
			direction = -1;
		}
	}

    #region ScreenBox System
    public GameObject[] ScreenBoxDetection()
    {
		float skinWidth = 0.1f;
		Vector2 raycastOrigin = new Vector2(transform.position.x, transform.position.y + controller.boxCollider.bounds.size.y + skinWidth);
		
		// Cast ray cast to see in what screenbox we are and if the next screen box is close
		RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, input, SC_RaycastMaxDistance, layerMask);
		// Debug.DrawRay(raycastOrigin, inputHandler.inputDirection * SC_RaycastMaxDistance, Color.red);
		
		if (hit)
		{
			// Beginning setup where both are null. So the very first screenbox will have this only
			if (nextScreenBox == null) nextScreenBox = hit.collider.gameObject;
			if (previousScreenBox == null) previousScreenBox = nextScreenBox;

			//When we collide with the other screenbox stting the pervious and the next screenbox(the next being the one we now collided with)
			if (hit.distance == 0 && hit.collider.gameObject != nextScreenBox)
			{
				previousScreenBox = nextScreenBox;
				nextScreenBox = hit.collider.gameObject;

				GameObject[] screenBoxes = new GameObject[2];
				screenBoxes[0] = previousScreenBox;	
				screenBoxes[1] = nextScreenBox;
				return screenBoxes;
			}

			return null;
	    }

		return null;
	}
    
	public float PlayerDistanceFromScreenBoxCenter()
    {
		if(nextScreenBox != null)
        {
			// Get the screenbox collider
			if (nextScreenBoxCollider == null || sc != nextScreenBox) nextScreenBoxCollider = nextScreenBox.GetComponent<PolygonCollider2D>();

			sc = nextScreenBox;

			screenBoxEntireWidth = nextScreenBoxCollider.bounds.size.x;	

			// Get the center of the active screenBox and the position of the player
			float screenBoxCenterPosition = nextScreenBoxCollider.bounds.center.x;
			float playerPosition = transform.position.x;

			// Calculate distance from the player and the center of the active screenBox
			float distanceFromCenter = playerPosition - screenBoxCenterPosition;

			return distanceFromCenter;
		}

		// If there is no active screen box gameobject return not a number
		return float.NaN;
	}
    #endregion
    
	#region Jump Checks and ability
	// Jump ability
	void Jump()
    {
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		velocity.y = jumpForce;	
	}

	// Methods which handle input detected in Update()
	public void OnJumpInput()
	{	
		LastPressedJumpTime = jumpInputBufferTime;
	}

	// Check if player can jump or not
	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping && !IsRolling;
	}
    #endregion
    
	#region Roll ability
	// Roll Coroutine
	private IEnumerator StartRoll(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedRollTime = 0;

		float startTime = Time.time;

		IsRolling = true;
		_rollesLeft--;
		_isRollAttacking = true;

		animatorFunctions.PlayAnimation(0, ROLL, false, 1.1f);

		while (Time.time - startTime <= rollAttackTime)
		{
			velocity = dir.normalized * rollSpeed;
			yield return null;
		}
		
		startTime = Time.time;

		_isRollAttacking = false;
		
		velocity = rollEndSpeed * dir.normalized;

		while (Time.time - startTime <= rollEndTime)
		{
			yield return null;
		}

		// Dash over
		IsRolling = false;
	}

	// Short period before the player is able to roll again
	private IEnumerator RefillRoll(int amount)
	{
		// SHoet cooldown, so we can't constantly roll along the ground
		_rollRefilling = true;
		yield return new WaitForSeconds(rollRefillTime);
		_rollRefilling = false;
		_rollesLeft = Mathf.Min(rollAmount, _rollesLeft + 1);
	}

	private bool CanRoll()
	{
		if (!IsRolling && _rollesLeft < rollAmount && LastOnGroundTime > 0 && !_rollRefilling)
		{
			StartCoroutine(nameof(RefillRoll), 5);
		}

		return _rollesLeft > 0 && controller.collisions.below && !isAttacking;
	}

	public void OnRollInput()
	{
		LastPressedRollTime = rollInputBufferTime;
	}
    #endregion
    
    #region Player Freeze And UnFreeze
    public void FreezePlayer()
    {
		freezePlayer = true;
    }

	public void UnFreezePlayer()
    {
		freezePlayer = false;
    }
	#endregion

	#region Attack
	public IEnumerator PlayNextAttackTimeWindow()
    {
		float startTime = Time.time;

		while (Time.time - startTime <= nextAttackTimeWindow)
        {
			var runs = 0;

			if (!playNextAttack && runs == 0)
            {
				playNextAttack = true;
				runs++;
            }
        }
		
		if (playNextAttack) playNextAttack = false;
		yield return null;
	}
	
	private void Attack(int attackQueue = 1)
    {
		isAttacking = true;

		animatorFunctions.PlayAnimation(1, "Combat/UlfSummoningStart");
		animatorFunctions.PlayAnimation(2, "Combat/Attack" + attackQueue);

		launch = direction * hurtLaunchPower.x;
		
		if(attackQueue > 1)
        {
			Debug.Log("NextAttack");
        }

		animatorFunctions.skeletonAnimation.AnimationState.Event += delegate (TrackEntry trackEntry, Spine.Event e)
		{
			if(isEventNextAttackWindow == e.Data)
            {
				StartCoroutine(nameof(PlayNextAttackTimeWindow));
            }
			
			if(isEventAttackEnd == e.Data)
            {			
				isAttacking = false;
				animatorFunctions.PlayAnimation(1, "Combat/UlfSummoningEnd");
				animatorFunctions.skeletonAnimation.state.AddEmptyAnimation(1, 0.2f, 0);
			}
		};
		
		animatorFunctions.skeletonAnimation.state.AddEmptyAnimation(2, 0, 0);
	}
    #endregion
}