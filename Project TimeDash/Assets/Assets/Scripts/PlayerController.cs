using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	//Indicates to the state machine the current state
	public PlayerState playerState; //make private when done debugging

	public ControllerPreference controllerPref;
	public float walkSpeed;   //Set in inspector
	public float sprintSpeed; //Set in inspector
	public float moveSpeed; //Should be set private when done testing speeds
	public float acceleration;

	//For handling animations
	private Animator anim;
	private bool playerMoving;
	private bool playerSprinting;
	private bool playerSprintAttacking;
	private bool playerDashing;
	private bool playerAttacking;
	private bool playerShielding;
	private bool playerHyperDashing;
	public Vector2 lastMove; //public so another script can access it
	//private PlayerDirections playerDirection; //Direction player faces when attacking

	//For handling collisions
	private Rigidbody2D playerBody;

	//I MAY HAVE TO MAKE THESE PUBLIC AND THEN GET RID OF THE ONES IN AttackAbility.cs AND USE THESE INSTEAD
	private SwordCollider swordCollider;
	private SwordCollider swordColliderUp;
	private SwordCollider swordColliderLeft;
	private SwordCollider swordColliderRight;

	//For fixing duplicate Player glitch
	//Static -> all objects that have this script will use this playerExists instance
	private static bool playerExists;
	private Vector3 attackDirection; 

	//Structures that handle each ability's logic
	//Set to private when done debugging
	public AbilitiesHandler abilitiesHandler;
	public DashAbility dashAbility;
	public AttackAbility attackAbility;
	private AbilityShield shieldAbility;
	private AbilityDodgeRoll dodgeAbility;
	private AbilitySprintAttack sprintAttackAbility;
	//public AbilityHyperDash abilityHyperDash;

	// Use this for initialization
	void Start () {
		//moveSpeed = walkSpeed;
		anim = GetComponent<Animator> ();
		playerBody = GetComponent<Rigidbody2D> ();
		attackAbility = GetComponent<AttackAbility> ();
		shieldAbility = GetComponent<AbilityShield> ();
		dodgeAbility = GetComponent<AbilityDodgeRoll> ();
		sprintAttackAbility = GetComponent<AbilitySprintAttack> ();
		//abilityHyperDash = GetComponent<AbilityHyperDash> ();
		swordCollider = GameObject.Find ("Sword Collider Down").GetComponent<SwordCollider> (); //Down
		swordColliderUp = GameObject.Find ("Sword Collider Up").GetComponent<SwordCollider> ();
		swordColliderLeft = GameObject.Find ("Sword Collider Left").GetComponent<SwordCollider> ();
		swordColliderRight = GameObject.Find ("Sword Collider Right").GetComponent<SwordCollider> ();

		//Disable colliders 
		swordCollider.Disable();
		swordColliderUp.Disable ();
		swordColliderLeft.Disable ();
		swordColliderRight.Disable ();

		//If Player doesn't exist yet
		if (!playerExists) {
			playerExists = true;
			DontDestroyOnLoad (transform.gameObject);
		} 
		else {
			Destroy (gameObject);
		}

		//Starting state for player
		playerState = PlayerState.Default;
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdatePlayer ();
	}

	//Handles player movement from user input
	public void MovePlayer() {
		//Get player input
		float moveHorizontal = (Input.GetAxis ("Horizontal")) * acceleration;
		float moveVertical = (Input.GetAxis ("Vertical")) * acceleration;

		Sprint ();
		
		//Move the Player according to raw input
		//if (moveHorizontal > 0.5f || moveHorizontal < -0.5f || moveVertical > 0.5f || moveVertical < -0.5f) {
		if (moveHorizontal > 0f || moveHorizontal < -0f || moveVertical > 0f || moveVertical < -0f) {
			//Move player
			Mathf.Clamp (moveVertical, 0f, 1f);
			Mathf.Clamp (moveHorizontal, 0f, 1f);
			playerBody.velocity = new Vector2 (moveHorizontal * moveSpeed, moveVertical * moveSpeed);

			playerMoving = true;
			lastMove = new Vector2 (moveHorizontal, moveVertical);
		}

		//Stop the player from moving when velocity is 0
		if ((moveHorizontal < 0.1f && moveHorizontal > -0.1f) || (moveVertical < 0.1F && moveVertical > -0.1f)) {
			playerBody.velocity = new Vector2 (moveHorizontal * moveSpeed, moveVertical * moveSpeed);
		}
			
	}

	//Makes player move faster if LEFT SHIFT / X is held down
	void Sprint() {
		if (Input.GetButton ("SprintPS4")) {
			//Debug.Log ("SPRINTING");
			moveSpeed = sprintSpeed;
			playerSprinting = true;
		} else {
			moveSpeed = walkSpeed;
			//playerSprinting = false;
		}
			
	}

	//===========UNUSED================
	//May need it if I decide to implement bow and arrow 
	//Makes the Player look towards a given point on the world map (like the mouse position)
	/*
	public void LookTowardsPoint(Vector3 target) {
		//Rotate towards target
		target = target - transform.position;

		float angle = Mathf.Atan2 (target.y, target.x) * Mathf.Rad2Deg;
        
		//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	}
	*/

	//Handles and updates the Player logic with a state machine
	public void UpdatePlayer() {
		//Animator bools 
		playerMoving = false;
		//playerSprinting = false;
		playerDashing = false;
		playerAttacking = false;
		playerSprintAttacking = false;

		//Switch Design: Make each Ability class/state take in the playerState
		//  and modify it only when that Ablity action is done executing
		abilitiesHandler.HandleTimers();

		switch (playerState) {
		case PlayerState.Default:
			//A function should go here that checks whether a certain state is available?
			if (controllerPref == ControllerPreference.KeyboardMouse)
				GetKeyboardInput ();
			else
				GetControllerInput ();
			
			MovePlayer ();
			break;

		case PlayerState.Dashing:
			//Note: Normalize first Dash because Player may be accelerating or slowing down, causing shorter dashes
			dashAbility.Dash (lastMove.normalized, ref playerState, ref playerDashing);
			break;

		case PlayerState.Attacking:
			attackAbility.Attack(ref playerState, ref playerAttacking, ref playerSprintAttacking, 
				                 ref playerSprinting, attackDirection); 
			break;

		case PlayerState.SprintAttacking:
			sprintAttackAbility.SprintAttack (ref playerState, attackDirection);
			break;

		case PlayerState.Shielding:
			shieldAbility.Shield (ref playerState);
			break;

		case PlayerState.Grabbing:

			break;

		case PlayerState.DodgeRolling:
			dodgeAbility.DodgeRoll (ref playerState, shieldAbility.GetDodgeRollDirection ());
			break;

		case PlayerState.HyperDashing:
			//if (abilityHyperDash.hyperDashingActive)
			attackDirection = GetMousePositionVector ();
			//abilityHyperDash.HyperDash (ref playerState, ref hyperDashCoolDownTimer, ref playerHyperDashing, attackDirection,
			//	                        moveHorizontal, moveVertical);
			break;

		} //switch

		//Let the animator know what just happened in this frame (like which way the player was facing
		anim.SetFloat ("MoveX", lastMove.x);
		anim.SetFloat ("MoveY", lastMove.y);
		anim.SetBool ("PlayerMoving", playerMoving);
		anim.SetFloat ("LastMoveX", lastMove.x);
		anim.SetFloat ("LastMoveY", lastMove.y);
		anim.SetBool ("PlayerDashing", playerDashing);
		anim.SetBool ("PlayerAttacking", playerAttacking);
		//anim.SetBool ("PlayerSprinting", playerSprinting);

		//anim.SetInteger ("PlayerAttackDirection", (int)playerDirection);
	}

	//This is where I am going to have to separate Keyboard+Mouse and Controller inputs
	//Handles all inputs other than player movement
	void GetKeyboardInput() {
		//If player pressed Dash button and player is moving
		if (Input.GetKeyDown (KeyCode.Space) && (!playerBody.velocity.Equals (Vector2.zero))) {
			//If cool down is done, allow dashing again
			// i.e., if state is available (no cooldown active), then allow state switch
			if (abilitiesHandler.isDashAvailable ()) {
				playerState = PlayerState.Dashing;
			} else
				playerState = PlayerState.Default;
		}
		// Handle mouse button inputs for attacks -> 0 is left click, 1 is right, 2 is middle
		else if (Input.GetButtonDown("Attack")) {
			//playerAttacking = true;
			if (abilitiesHandler.isAttackAvailable()) {
				attackDirection = GetMousePositionVector ();
				playerState = PlayerState.Attacking;

			}
		}
		//Handle input for Hyper Dash -> Right mouse click
		else if (Input.GetMouseButtonDown(1)) {
			playerState = PlayerState.HyperDashing;
		}
		//Handle Sonic Attack (V or LEFT SHIFT button)
	}

	//FOR PS4 CONTROLLER
	//Handles all inputs other than player movement
	void GetControllerInput() {
		//If player pressed Dash button and player is moving
		if (Input.GetKeyDown (KeyCode.Space) && (!playerBody.velocity.Equals (Vector2.zero))) {
			//If cool down is done, allow dashing again
			// i.e., if state is available (no cooldown active), then allow state switch
			if (abilitiesHandler.isDashAvailable ()) {
				playerState = PlayerState.Dashing;
			} else
				playerState = PlayerState.Default;
		}
		// Handle mouse button inputs for attacks -> 0 is left click, 1 is right, 2 is middle
		else if (Input.GetButtonDown ("AttackPS4")) {
			//playerAttacking = true;
			if (abilitiesHandler.isAttackAvailable ()) {
				//Attack in direction player is facing
				//NOTE: Multiplied by 10 so that the player moves far enough when attacking
				lastMove.Normalize ();
				//Change attack distance depending on which move is executed
				if (!playerSprinting) {
					attackDirection = new Vector3 (transform.position.x + lastMove.x * 10f, 
						transform.position.y + lastMove.y * 10f);
					
					playerState = PlayerState.Attacking;
				} else {
					attackDirection = new Vector3 (transform.position.x + lastMove.x,
						transform.position.y + lastMove.y);
					
					playerState = PlayerState.SprintAttacking;
				}

			}
		}
		//Handle shielding 
		else if (Input.GetButton ("ShieldPS4")) {
			playerState = PlayerState.Shielding;
		}
		//Handle input for Hyper Dash -> Right mouse click
		else if (Input.GetMouseButtonDown(1)) {
			playerState = PlayerState.HyperDashing;
		}
		//Handle Sonic Attack (V or LEFT SHIFT button)
	}

	//Gets the position of where the mouse is in the world
	//Currently used by:
	//  Attack Ability
	//  HyperDash Ability
	public Vector3 GetMousePositionVector() {
		Vector3 target;

		//Vector3 implicitly converted to Vector2
		target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		target.z = transform.position.z;

		//target.Normalize();
		//transform.position = Vector2.MoveTowards (transform.position, target, 10 * Time.deltaTime);
		Debug.Log("Target: " + target);
		return target;
	}
		
	//Ignores sword collider
	// @other: the player
	/*
	void OnTriggerEnter (Collider player) {
		if (player.tag == "Sword") {
			//Line below calls the function ApplyDamage(10)
			//other.gameObject.SendMessage("ApplyDamage", 10);
			return;
		}
	}
	*/

}



//State Machine for player
public enum PlayerState {
	Default, 
	Dashing,
	Attacking,
	SprintAttacking,
	HyperDashing,
	Shielding,
	DodgeRolling,
	Grabbing
	//DashGrabbing
	//Charging Attack
	//Hurt
	//Knocked Back
	//Interacting with Object
	//Talking with NPC
	//Climbing
	//Jumping
	//Cutscene
}

public enum ControllerPreference {
	KeyboardMouse,
	PS4Controller
}
