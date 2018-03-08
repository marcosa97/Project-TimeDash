using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the grabbing state
public class AbilityGrab : MonoBehaviour {
	//enums
	private enum GrabState {
		Setup,
		Grab,
		HoldingEnemy,
		Done
		//HoldingItem
	}

	//Public Settings
	public float grabTime;

	//References and variables needed
	private float timer; //how long a grab animation takes
	private bool enemyGrabbed; 
	private Rigidbody2D playerBody; //To stop player movement
	private GrabState grabState;
	private Animator playerAnimator;
	private CircleCollider2D grabCollider;

	// Use this for initialization
	void Start () {
		playerBody = GetComponent<Rigidbody2D> (); 
		playerAnimator = GetComponent<Animator> ();
		grabCollider = GameObject.Find ("Grab Collider Up").GetComponent<CircleCollider2D> ();
		grabCollider.enabled = false;
		grabState = GrabState.Setup;
		enemyGrabbed = false;
	}

	public void Grab(ref PlayerState playerState) {
		switch (grabState) {
		case GrabState.Setup:
			timer = grabTime;
			grabState = GrabState.Grab;
			playerBody.velocity = Vector2.zero;


			//Activate collider
			grabCollider.enabled = true;
			playerAnimator.Play("Grab State");

			break;


		case GrabState.Grab:
			timer -= Time.deltaTime;
			//Play grab animation

			//if hitbox grabs something
			if (enemyGrabbed) {
				//transition to grabbing state
				//Reset stuff
				//break;
			}

			//Grab move is done, and something was not grabbed
			if (timer <= 0f) {
				timer = 0f;
				grabState = GrabState.Done;
				grabCollider.enabled = false;
			}

			break;


		case GrabState.HoldingEnemy:
			timer -= Time.deltaTime;


			break;

		
		case GrabState.Done:
			//Reset
			grabState = GrabState.Setup;
			playerState = PlayerState.Default;

			break;
		}
	}

	/// <summary>
	///   To be called by the grab collider's script.
	///   This function makes the bool "enemyGrabbed" true
	///   to let the state machine know that a grabbable enemy
	///   has been detected by the grab collider.
	///   To Implement: A float will also be passed in,
	///   which represents the time for how long the enemy
	///   can be held for before they break free
	/// </summary>
	public void EnemyDetected(float holdTime) {
		Debug.Log ("Grabbed Enemy");
		enemyGrabbed = true;
		timer = holdTime;
	}

	public void ResetState(ref PlayerState playerState) {
		timer = 0f;
		grabState = GrabState.Setup;
		grabCollider.enabled = false;
		playerState = PlayerState.Default;
	}
}
