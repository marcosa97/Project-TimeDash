using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: May have to pass in the ControllerPref variable to distinguish inputs in each class
//This class handles the player's basic movement states
public class AbilityBasicMovement : MonoBehaviour {

	//There will be a time window when the player begins to move in which "Attack" button can be 
	//    pressed to initiate a charged attack
	private enum MoveState {
		ChargeAttackTimeWindow,
		Moving
	}

	//Public settings
	public float walkSpeed;   //Set in inspector
	public float sprintSpeed; //Set in inspector
	public float acceleration;

	//References and variables neded
	private float moveSpeed;
	private MoveState moveState;
	private Rigidbody2D playerBody;
	private AbilitiesHandler abilitiesHandler; //Get rid of this eventually

	private Vector2 lastMove;
	private Vector2 attackDirection; //used when an attack happens
	public bool playerMoving;
	private bool playerSprinting;

	// Use this for initialization
	void Start () {
		playerBody = GetComponent<Rigidbody2D> ();
		abilitiesHandler = GetComponent<AbilitiesHandler> ();

	}

	public void Idle(ref PlayerState playerState) {
		//Set player velocity to 0? Probably no need to do so

		//Get Input
		//GetControllerInput(ref playerState);
		MovePlayer();
		if (playerMoving) {
			playerState = PlayerState.Moving; //change to chargeAttackTime when implemented
			moveState = MoveState.Moving;
		}

		GetControllerInput(ref playerState);

		//Play Idle animation
	}

	public void Move(ref PlayerState playerState) {

		switch (moveState) {
		case MoveState.ChargeAttackTimeWindow:

			break;

		case MoveState.Moving:
			//Get Input
			MovePlayer ();
			if (!playerMoving)
				playerState = PlayerState.Default;

			GetControllerInput (ref playerState);

			break;
		}

	}

	//===============HELPER FUNCTIONS THAT DO THE MOVING=======================
	//Handles player movement from user input
	private void MovePlayer() {
		//Get player input
		float moveHorizontal = (Input.GetAxis ("Horizontal")) * acceleration;
		float moveVertical = (Input.GetAxis ("Vertical")) * acceleration;

		Sprint (ref moveHorizontal, ref moveVertical);

		//Move the Player according to raw input
		//if (moveHorizontal > 0.5f || moveHorizontal < -0.5f || moveVertical > 0.5f || moveVertical < -0.5f) {
		if (moveHorizontal > 0f || moveHorizontal < -0f || moveVertical > 0f || moveVertical < -0f) {
			//Move player
			Mathf.Clamp (moveVertical, 0f, 1f);
			Mathf.Clamp (moveHorizontal, 0f, 1f);
			playerBody.velocity = new Vector2 (moveHorizontal * moveSpeed, moveVertical * moveSpeed);

			playerMoving = true;
			lastMove = new Vector2 (moveHorizontal, moveVertical);
		} else {
			playerMoving = false;
		}

		//Stop the player from moving when velocity is 0
		if ((moveHorizontal < 0.1f && moveHorizontal > -0.1f) || (moveVertical < 0.1F && moveVertical > -0.1f)) {
			playerBody.velocity = new Vector2 (moveHorizontal * moveSpeed, moveVertical * moveSpeed);
		}

	}

	//Makes the player sprint (increases velocity) if SprintPS4 button is being pressed
	private void Sprint(ref float moveHorizontal, ref float moveVertical) {
		Vector2 directionNormalized;

		if (Input.GetButton ("SprintPS4")) {
			//If player is not moving, don't activate sprinting bool
			if (moveHorizontal == 0f && moveVertical == 0f) {
				playerSprinting = false;
			} else {
				//This is so that sprint speed is always the same
				directionNormalized = new Vector2 (moveHorizontal, moveVertical);
				directionNormalized.Normalize ();
				moveHorizontal = directionNormalized.x;
				moveVertical = directionNormalized.y;
				//Debug.Log ("SPRINTING");
				moveSpeed = sprintSpeed;
				playerSprinting = true;
			}
		} else {
			moveSpeed = walkSpeed;
			playerSprinting = false;
		}

	}


	//FOR PS4 CONTROLLER
	//Handles all inputs other than player movement
	private void GetControllerInput(ref PlayerState playerState) {
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

	//================GETTER FUNCTIONS=================
	public Vector2 GetAttackDirection() {
		return attackDirection;
	}

	public Vector2 GetLastMove() {
		return lastMove;
	}

	public bool GetPlayerMoving() {
		return playerMoving;
	}

}
