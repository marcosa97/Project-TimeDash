using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class controls the player's state machine. Contains and manages all the ability scripts
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
	private PlayerStateFlinch flinchState;
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
		flinchState = GetComponent<PlayerStateFlinch> ();
		//abilityHyperDash = GetComponent<AbilityHyperDash> ();

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
			anim.Play ("Idle Direction");
			break;

		case PlayerState.Moving:
			basicMovement.Move (ref playerState);
			anim.Play ("Player Movement");
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

		case PlayerState.Flinch:
			flinchState.Flinch (ref playerState);
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

	//====================FOR SWITCHING INTO HURT STATE=========================

	//Changes the current player state to default
	private void CancelCurrentState() {
		switch (playerState) {
		case PlayerState.Default:
			//Stay in this state, so do nothing
			break;
		case PlayerState.Moving:
			basicMovement.ResetState (ref playerState);
			break;
		case PlayerState.Attacking:
			attackAbility.ResetState (ref playerState);
			break;
		case PlayerState.ChargedAttacking:
			chargedAttackAbility.ResetState (ref playerState);
			break;
		case PlayerState.SprintAttacking:
			sprintAttackAbility.ResetState (ref playerState);
			break;
		case PlayerState.Shielding:

			break;
		case PlayerState.Flinch:
			flinchState.ResetState (ref playerState);
			break;
		//Add cases for grabbing and other states 
		}
	}

	//pass in AttackType and an AttackInfoObject
	public void HurtPlayer(AttackType attackType) {
		CancelCurrentState ();

		switch(attackType) {
		case AttackType.MeleeWeakAttack:
			playerState = PlayerState.Flinch;
			break;
		case AttackType.MeleeStrongAttack:
			//playerState = PlayerState.KnockBackAir;
			break;
		}
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

public enum AttackType {
	MeleeWeakAttack,
	MeleeStrongAttack
	//Arrow
	//Stun
	//etc
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
	Grabbing,
	Flinch //Hurt from weak hit
	//DashGrabbing
	//Hurt
	//Knocked Back
	//OnGroundFromKnockBack
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
