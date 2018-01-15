using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShield : MonoBehaviour {
	//Collider is going to need to be resizable (maybe able to rotate too)

	//Public settings
	public float moveSpeed;

	//References and variables needed
	private bool playerMoving; //Maybe use this for animator
	private Vector2 moveDirection; 
	private Rigidbody2D playerBody;
	private AbilitiesHandler abilitiesHandler;

	// Use this for initialization
	void Start () {
		playerMoving = false;
		moveDirection = Vector2.zero;
		playerBody = GetComponent<Rigidbody2D> ();
		abilitiesHandler = GetComponent<AbilitiesHandler> ();
	}

	//Like PlayerController's Move() function, except this one makes the player move while shielding
	// and without rotating directions (MAYBE)
	private void Strafe() {
		//Get player input
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Mathf.Clamp (moveVertical, 0f, 1f);
		Mathf.Clamp (moveHorizontal, 0f, 1f);
		playerBody.velocity = new Vector2 (moveHorizontal * moveSpeed, moveVertical * moveSpeed);
		moveDirection = new Vector2 (moveHorizontal, moveVertical);
		moveDirection.Normalize ();
		moveDirection = new Vector2 (moveDirection.x + transform.position.x, moveDirection.y + transform.position.y);

		//if (!(moveVertical <= 0f) || !(moveHorizontal <= 0f))
		if (playerBody.velocity != Vector2.zero)
			playerMoving = true;
		else
			playerMoving = false;
		
	}

	//Checks if player is holding down shielding button
	private bool isShielding() {
		if (Input.GetButton ("ShieldPS4"))
			return true;
		else
			return false;
	}

	//Performs Shield ability
	//NOTE: Also need to pass in bool if that's how I'm gonna handle animationss
	public void Shield(ref PlayerState playerState) {
		Strafe ();

		//Activate shield collider here

		//If player is shielding, stay in this state; else, go to default
		if (isShielding ()) {
			//Stay in this state
		} else {
			playerState = PlayerState.Default;
			return;
		}

		//Check for Grab, Dodge, or (maybe) Jump inputs
		//Grab
		if (Input.GetButtonDown ("AttackPS4")) {
			//Make a Dodging script?
		} else if (Input.GetButtonDown ("SprintPS4")) {
			//Check if there's any movement
			if (playerMoving) {
				//Switch to DodgeRoll state
				abilitiesHandler.activateDodgeTimer(); 
				playerState = PlayerState.DodgeRolling;
			}
		}

	} //Shield

	public Vector2 GetDodgeRollDirection() {
		return moveDirection;
	}

}

public enum ShieldState{
	Shielding,
	Transition
}
