﻿using UnityEngine;

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

	//Structures that handle each state's logic (abilities, movement)
	private AbilityBasicMovement basicMovement;
	private DashAbility dashAbility;
	private AttackAbility attackAbility;
	private AbilityShield shieldAbility;
	private AbilityDodgeRoll dodgeAbility;
	private AbilitySprintAttack sprintAttackAbility;
	private AbilityChargedAttack chargedAttackAbility;
    private AbilityWarpStrike warpStrikeAbility;
	private AbilityGrab grabAbility;
	private PlayerStateFlinch flinchState;
    private AbilityFall fallState;
    private AbilityInteract interactState;
    private AbilityDialogue dialogueState;
	private HurtInfoReceiver hurtInfo;
    private PlayerHealthComponent playerHealthComponent;
    //public AbilityHyperDash abilityHyperDash;

	// Use this for initialization
	void Start () {
		//Physics.gravity = new Vector3 (0, 0, -9.81f);
		anim = GetComponent<Animator> ();
		basicMovement = GetComponent<AbilityBasicMovement> ();
		attackAbility = GetComponent<AttackAbility> ();
		shieldAbility = GetComponent<AbilityShield> ();
		dodgeAbility = GetComponent<AbilityDodgeRoll> ();
		sprintAttackAbility = GetComponent<AbilitySprintAttack> ();
		chargedAttackAbility = GetComponent<AbilityChargedAttack> ();
        warpStrikeAbility = GetComponent<AbilityWarpStrike>();
		grabAbility = GetComponent<AbilityGrab> ();
		flinchState = GetComponent<PlayerStateFlinch> ();
        fallState = GetComponent<AbilityFall> ();
        interactState = GetComponent<AbilityInteract> ();
        dialogueState = GetComponent<AbilityDialogue> ();
		hurtInfo = GetComponent<HurtInfoReceiver> ();
        playerHealthComponent = GetComponent<PlayerHealthComponent>();
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
		playerDashing = false;
		playerAttacking = false;
		playerSprintAttacking = false;

		//Switch Design: Make each Ability class/state take in the playerState
		//  and modify it only when that Ablity action is done executing

		switch (playerState) {

		/*
		case PlayerState.Default:
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
			grabAbility.Grab (ref playerState);
			break;

		case PlayerState.DodgeRolling:
			dodgeAbility.DodgeRoll (ref playerState, shieldAbility.GetDodgeRollDirection ());
			break;

		case PlayerState.Flinch:
			flinchState.Flinch (ref playerState);
			break;

        case PlayerState.Falling:
            fallState.Fall(ref playerState);
            break;

        case PlayerState.Interacting:
            interactState.Interact(ref playerState);
            break;

        case PlayerState.Dialogue:
            dialogueState.Dialogue(ref playerState);
            break;

        case PlayerState.WarpStrike:
            warpStrikeAbility.WarpStrike(ref playerState, basicMovement.GetAttackDirection());
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
    //returns true if state was successfully cancelled
	private bool CancelCurrentState() {
		switch (playerState) {
		case PlayerState.Default:
			//Stay in this state, so do nothing
			basicMovement.ResetState (ref playerState);
            return true;
		case PlayerState.Moving:
			basicMovement.ResetState (ref playerState);
			return true;
		case PlayerState.Attacking:
			attackAbility.ResetState (ref playerState);
            return true;
		case PlayerState.ChargedAttacking:
			chargedAttackAbility.ResetState (ref playerState);
			return true;
		case PlayerState.SprintAttacking:
			sprintAttackAbility.ResetState (ref playerState);
			return true;
		case PlayerState.Shielding:
			shieldAbility.ResetState (ref playerState);
			return true;
		case PlayerState.Flinch:
			flinchState.ResetState (ref playerState);
			return true;
		case PlayerState.Grabbing:
			grabAbility.ResetState (ref playerState);
			return true;
		//Add cases for grabbing and other states 
		}

        return false;
	}

	//pass in AttackType and an AttackInfoContainer
	public void HurtPlayer(AttackInfoContainer enemyAttack) {
		//Update info on hurt script
		hurtInfo.UpdateHurtInfo(enemyAttack);

        //Deal Damage
        playerHealthComponent.TakeDamage(enemyAttack.damage);

        //If state was cancelled, make player flinch
		if ( CancelCurrentState () )
		    switch(enemyAttack.attackType) {
		    case AttackType.MeleeWeakAttack:
			    playerState = PlayerState.Flinch;
			    break;
		    case AttackType.MeleeStrongAttack:
			    //playerState = PlayerState.KnockBackAir;
			    break;
		}
	}

    public void MakePlayerFall() {
        //Cancel state
        CancelCurrentState();

        playerState = PlayerState.Falling;
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
	Grabbing,
	Flinch, //Hurt from weak hit
	WarpStrike,
    Falling,
    Interacting,
    Dialogue
	//Knocked Back
	//Cutscene
}

public enum ControllerPreference {
	KeyboardMouse,
	PS4Controller
}
