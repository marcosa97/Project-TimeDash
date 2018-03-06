using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour {
	public DashState dashState;
	public float dashTime;
	public float dashSpeed;
	private float dashTimer; //private

	//New Feature: chain 3 different dashes one right after another w/out cooldown enabled
	//public int dashChainMax; //Max amount of dashes allowed: Set in inspector 
	//public int dashChainCount; //UPDATE NEEDED: set to private

	//References we need
	public Rigidbody2D playerBody; //We need reference so we can move the body
	public PlayerController playerController;
	private BoxCollider2D playerCollider;
	////private AbilitiesHandler abilitiesHandler;

	void Start() {
		playerBody = GetComponent<Rigidbody2D> ();
		playerController = GetComponent<PlayerController> ();
		playerCollider = GetComponent<BoxCollider2D> ();
		////abilitiesHandler = GetComponent<AbilitiesHandler> ();
	}

	//Updates every frame
	//Use this to handle timer
	void Update() {
		//Check every frame if timer is active or not

	}
		
	//Note: This Dash Ability doesn't get the Input, the PlayerController does
	//Handles the Dash Ability
	// @moveHorizontal: x component of the player movement direction
	// @moveVertical: y component of the player movement direction
	// @playerState: the state the player is currently in
	//               -> This function will be called when playerState == Dashing. 
	//               -> playerState will be changed to Default once a Dash is completed
	// @coolDownTimer: The timer that will count down how much time the player has until 
	//                 he/she can use the Dash ability again
	public void Dash(Vector2 moveDirection, ref PlayerState playerState, ref bool playerDashing) {

		switch (dashState) {
		case DashState.Ready:
			//Set up dash velocity and timer
			playerBody.velocity = new Vector2 (moveDirection.x * dashSpeed, moveDirection.y * dashSpeed);
			dashState = DashState.Dashing;
			dashTimer = dashTime; //How long the dash lasts

			//Keep track of last move made so the animator knows which way to make the player face
			///////playerController.lastMove.x = moveDirection.x;
			///////playerController.lastMove.y = moveDirection.y;

			//If Dash Chain hasn't been activated
			/*
			if (!dashChainActive) {
				dashChainActive = true;
				dashChainCount = dashChainMax;
			}
            */
			break;


		case DashState.Dashing:
			dashTimer -= Time.deltaTime * 3;

			//Set dashing animation
			playerDashing = true;

			if (dashTimer <= 0f) {
				//Physics2D.IgnoreCollision (playerCollider, other.gameObject.GetComponent<Collider2D> (), false);
				dashState = DashState.Done;

				//Time window that Chain Dash can be used within
				//dashTimer = 0.2f;
				dashTimer = 0f; //Reset timer
			}
			break;

		case DashState.Done://Change to DashState.Done
			//dashTimer -= Time.deltaTime;

			//Reset States
			dashState = DashState.Ready;
			playerState = PlayerState.Default;

			//Activate Cooldown Timer
			////abilitiesHandler.activateDashTimer(); !!!!

			//Where I left off: handle movement so that the player can do anything except dash right after dashing
			//Stop dash movement
			//playerController.MovePlayer ();

			/*
			//dashChainCount can only be 0 when the player has exhausted all chain attacks
			if (dashChainCount <= 0) {
				dashChainActive = false;
				coolDownTimer = coolDownTime;
				playerState = PlayerState.Default;
				dashState = DashState.Ready; //Reset dashState
				break;
			}

			//If Chain Dash is activated
			if (Input.GetKeyDown (KeyCode.Space)) {
				dashChainCount--;
				dashState = DashState.Ready;
				break;
			}

			//If Chain Dash time window is over
			if (dashTimer <= 0f) {
				dashChainActive = false;
				dashChainCount = 0; //Reset chain counter
				dashTimer = 0;
				dashState = DashState.Ready; //Reset dashState
				playerState = PlayerState.Default; 
			}
			*/

			break;
		} //switch
	} //Dash

	//NOTE: Dashing through enemies will probably be added to the secondary dash ability: the sonic dash attack
	/*
	//Dash through enemies
	void OnCollisionEnter2D(Collision2D other) {
		if (dashState == DashState.Dashing) {
			if (other.gameObject.tag == "Enemy") {
				Physics2D.IgnoreCollision (playerCollider, other.gameObject.GetComponent<Collider2D>(), true);
			}
		}
	}


	void OnCollisionExit2D(Collision2D other) {
			if (other.gameObject.tag == "Enemy") {
				Physics2D.IgnoreCollision (playerCollider, other.gameObject.GetComponent<Collider2D> (), false);
			}
	}
	*/

}

public enum DashState {
	Ready,
	Dashing,
	Done
}
