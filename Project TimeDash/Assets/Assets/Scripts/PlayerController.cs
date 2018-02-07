using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	//Indicates to the state machine the current state
	public PlayerState playerState; //make private when done debugging

	public ControllerPreference controllerPref;

	//For handling animations
	private Animator anim;
	private bool playerMoving;
	private bool playerSprinting;
	private bool playerSprintAttacking;
	private bool playerDashing;
	private bool playerAttacking;
	private bool playerShielding;

	//I MAY HAVE TO MAKE THESE PUBLIC AND THEN GET RID OF THE ONES IN AttackAbility.cs AND USE THESE INSTEAD
	private SwordCollider swordCollider;
	private SwordCollider swordColliderUp;
	private SwordCollider swordColliderLeft;
	private SwordCollider swordColliderRight;

	//For fixing duplicate Player glitch
	//Static -> all objects that have this script will use this playerExists instance
	private static bool playerExists;

	//Structures that handle each ability's logic
	//Set to private when done debugging
	private AbilityBasicMovement basicMovement;
	private DashAbility dashAbility;
	private AttackAbility attackAbility;
	private AbilityShield shieldAbility;
	private AbilityDodgeRoll dodgeAbility;
	private AbilitySprintAttack sprintAttackAbility;
	private AbilityChargedAttack chargedAttackAbility;
	//public AbilityHyperDash abilityHyperDash;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		basicMovement = GetComponent<AbilityBasicMovement> ();
		attackAbility = GetComponent<AttackAbility> ();
		shieldAbility = GetComponent<AbilityShield> ();
		dodgeAbility = GetComponent<AbilityDodgeRoll> ();
		sprintAttackAbility = GetComponent<AbilitySprintAttack> ();
		chargedAttackAbility = GetComponent<AbilityChargedAttack> ();
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
		//abilitiesHandler.HandleTimers();

		switch (playerState) {

		/*
		case PlayerState.Default:
			//A function should go here that checks whether a certain state is available?
			if (controllerPref == ControllerPreference.KeyboardMouse)
				GetKeyboardInput ();
			else
				GetControllerInput ();
			
			MovePlayer ();
			break;
		*/

		case PlayerState.Default:
			basicMovement.Idle (ref playerState);
			break;

		case PlayerState.Moving:
			basicMovement.Move(ref playerState);
			break;

		case PlayerState.Dashing:
			//Note: Normalize first Dash because Player may be accelerating or slowing down, causing shorter dashes
			dashAbility.Dash (basicMovement.GetLastMove().normalized, ref playerState, ref playerDashing);
			break;

		case PlayerState.Attacking:
			attackAbility.Attack(ref playerState, basicMovement.GetAttackDirection() ); 
			break;

		case PlayerState.ChargedAttacking:
			chargedAttackAbility.ChargeAttack (ref playerState, basicMovement.GetAttackDirection ());
			break;

		case PlayerState.SprintAttacking:
			sprintAttackAbility.SprintAttack (ref playerState, basicMovement.GetAttackDirection() );
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
			////////attackDirection = GetMousePositionVector ();
			//abilityHyperDash.HyperDash (ref playerState, ref hyperDashCoolDownTimer, ref playerHyperDashing, attackDirection,
			//	                        moveHorizontal, moveVertical);
			break;

		} //switch

		//Let the animator know what just happened in this frame (like which way the player was facing
		anim.SetFloat ("MoveX", basicMovement.GetLastMove().x);
		anim.SetFloat ("MoveY", basicMovement.GetLastMove().y);
		anim.SetBool ("PlayerMoving", basicMovement.GetPlayerMoving() );
		anim.SetFloat ("LastMoveX", basicMovement.GetLastMove().x );
		anim.SetFloat ("LastMoveY", basicMovement.GetLastMove().y );
		anim.SetBool ("PlayerDashing", playerDashing);
		anim.SetBool ("PlayerAttacking", attackAbility.GetPlayerAttacking() );
		//anim.SetInteger ("PlayerAttackDirection", (int)playerDirections);
		//anim.SetBool ("PlayerSprinting", playerSprinting);

		//anim.SetInteger ("PlayerAttackDirection", (int)playerDirection);
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
	Moving,
	Dashing,
	Attacking,
	ChargedAttacking,
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
