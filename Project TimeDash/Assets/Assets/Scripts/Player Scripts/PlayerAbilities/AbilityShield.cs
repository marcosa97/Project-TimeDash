using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityShield : MonoBehaviour {
	//Collider is going to need to be resizable (maybe able to rotate too)
	private enum ShieldState {
		Setup,
		Shielding
		//ShieldStun //-> when player's shield is hit
	}

	//Public settings
	public float moveSpeed;

	//References and variables needed
	private bool playerMoving; //Maybe use this for animator
	private Vector2 moveDirection; //Relative to position in world
	private Rigidbody2D playerBody;
	private Animator animator;
	private OrientationSystem orientationSystem;
	private Vector2 rawFaceDirection; //Vector representing face direction
	private EightDirections playerFaceDirection;
	private ShieldState shieldState;
	private AbilityBasicMovement moveInfo;

	private PolygonCollider2D shieldColliderUp;
	private PolygonCollider2D shieldColliderDown;
	private PolygonCollider2D shieldColliderRight;
	private PolygonCollider2D shieldColliderLeft;

	// Use this for initialization
	void Start () {
		shieldState = ShieldState.Setup;
		playerMoving = false;
		moveDirection = Vector2.zero;
		playerBody = GetComponent<Rigidbody2D> ();
		animator = GetComponent<Animator> ();
		orientationSystem = GetComponent<OrientationSystem> ();
		moveInfo = GetComponent<AbilityBasicMovement> ();
		shieldColliderUp = GameObject.Find ("Shield Collider Up").GetComponent<PolygonCollider2D> ();
		shieldColliderDown = GameObject.Find ("Shield Collider Down").GetComponent<PolygonCollider2D> ();
		shieldColliderRight = GameObject.Find ("Shield Collider Right").GetComponent<PolygonCollider2D> ();
		shieldColliderLeft = GameObject.Find ("Shield Collider Left").GetComponent<PolygonCollider2D> ();

		shieldColliderUp.enabled = false;
		shieldColliderDown.enabled = false;
		shieldColliderRight.enabled = false;
		shieldColliderLeft.enabled = false;
	}

	//Activates collider corresponding to the direction
	// the player is facing
	//NOTE: Writing 2 functions for enabling and disabling
	//      instead of just flipping the value just to be safe
	private void ActivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			shieldColliderUp.enabled = true;
			break;
		case EightDirections.NorthEast:

			break;
		case EightDirections.East:
			shieldColliderRight.enabled = true;
			break;
		case EightDirections.SouthEast:

			break;
		case EightDirections.South:
			shieldColliderDown.enabled = true;
			break;
		case EightDirections.SouthWest:

			break;
		case EightDirections.West:
			shieldColliderLeft.enabled = true;
			break;
		case EightDirections.NorthWest:

			break;
		}
	}

	//Deactivates collider corresponding to the direction
	// the player is facing
	private void DeactivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			//.Log ("NORTH COLLIDER");
			shieldColliderUp.enabled = false;
			break;
		case EightDirections.NorthEast:

			break;
		case EightDirections.East:
			//Debug.Log ("EAST COLLIDER");
			shieldColliderRight.enabled = false;
			break;
		case EightDirections.SouthEast:

			break;
		case EightDirections.South:
			shieldColliderDown.enabled = false;
			break;
		case EightDirections.SouthWest:

			break;
		case EightDirections.West:
			shieldColliderLeft.enabled = false;
			break;
		case EightDirections.NorthWest:

			break;
		}
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
		switch(shieldState) {
		case ShieldState.Setup:
			//Activate shield collider here
			rawFaceDirection = moveInfo.GetLastMove();
			rawFaceDirection.x += transform.position.x;
			rawFaceDirection.y += transform.position.y;
			playerFaceDirection = orientationSystem.GetDirection (rawFaceDirection);
			Debug.Log (moveInfo.GetLastMove ());
			ActivateCorrespondingCollider (playerFaceDirection);
			Debug.Log (playerFaceDirection);

			//Play animation
			animator.Play ("Shield State");

			shieldState = ShieldState.Shielding;

			break;
		case ShieldState.Shielding:
			//NOTE: if strafing, play strafe animation
			Strafe (); 

			//If player is shielding, stay in this state; else, exit state
			if (isShielding ()) {
				//Stay in this state
			} else {
				DeactivateCorrespondingCollider (playerFaceDirection);

				shieldState = ShieldState.Setup;
				playerState = PlayerState.Default;
				return;
			}

			//Check for Grab, Dodge, or (maybe) Jump inputs
			//Grab
			if (Input.GetButtonDown ("AttackPS4")) {
				//shieldColliderUp.enabled = false;
				//Switch to grab state
			} else if (Input.GetButtonDown ("SprintPS4")) {
				//Check if there's any movement
				if (playerMoving) {
					//Switch to DodgeRoll state
					DeactivateCorrespondingCollider (playerFaceDirection);
					shieldState = ShieldState.Setup;
					playerState = PlayerState.DodgeRolling;
				}
			}

			break;

	   	}

	} //Shield

	public Vector2 GetDodgeRollDirection() {
		return moveDirection;
	}

	public void ResetState(ref PlayerState playerState) {
		//Disable colliders
		shieldColliderUp.enabled = false;
		shieldColliderDown.enabled = false;
		shieldColliderLeft.enabled = false;
		shieldColliderRight.enabled = false;

		playerMoving = false;
		moveDirection = Vector2.zero;
		playerState = PlayerState.Default;
	}

}
