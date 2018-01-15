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
	private float grabTimer;
	//private bool playerGrabbing;
	private Rigidbody2D playerBody; //To stop player movement
	private GrabState grabState;
	//Probably some 2D colliders here too

	// Use this for initialization
	void Start () {
		playerBody = GetComponent<Rigidbody2D> ();
	}

	void Grab(ref PlayerState playerState) {
		switch (grabState) {
		case GrabState.Setup:
			grabTimer = grabTime;
			grabState = GrabState.Grab;
			playerBody.velocity = Vector2.zero;
			break;


		case GrabState.Grab:
			//Play grab animation

			//if hitbox grabs something
				//transition to grabbing state
				//Reset stuff
				//break;

			//Grab move is done, and something was not grabbed
			if (grabTimer <= 0f) {
				grabTimer = 0f;
				grabState = GrabState.Done;
			}

			break;


		case GrabState.HoldingEnemy:

			break;

		
		case GrabState.Done:
			//Reset
			grabState = GrabState.Setup;
			playerState = PlayerState.Default;
			break;
		}
	}
}
