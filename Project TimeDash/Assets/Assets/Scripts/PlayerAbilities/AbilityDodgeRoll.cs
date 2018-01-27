using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDodgeRoll : MonoBehaviour {
	//Player collider is going to be disabled for enemy attacks, but enabled for colliding with bodies, objects, and walls

	private enum DodgeState {
		Setup,
		Dodging,
		Done
	}

	//Public settings
	public float moveSpeed;
	public float dodgeTime;

	//References and variables needed
	public float dodgeTimer;
	private bool playerDodging;
	//private Rigidbody2D playerBody;
	private DodgeState dodgeState;

	// Use this for initialization
	void Start () {
		//playerBody = GetComponent<Rigidbody2D> ();
	}

	//Performs the DodgeRoll from shielding
	public void DodgeRoll(ref PlayerState playerState, Vector2 moveDirection) {
		//Ignore enemy attack colliders

		switch (dodgeState) {
		case DodgeState.Setup:
			dodgeState = DodgeState.Dodging;
			dodgeTimer = dodgeTime;
			playerDodging = true;
			break;

		case DodgeState.Dodging:
			dodgeTimer -= Time.deltaTime;

			transform.position = Vector2.MoveTowards (transform.position, moveDirection, moveSpeed * Time.deltaTime);
			//Ignore collisions with enemy hitboxes

			if (dodgeTimer <= 0f) {
				dodgeTimer = 0f;
				dodgeState = DodgeState.Done;
				playerDodging = false;
			}

			break;
		
		//Is this state necessary?
		case DodgeState.Done:
			//Reset
			if (Input.GetButton("ShieldPS4") )
				playerState = PlayerState.Shielding;
			else
				playerState = PlayerState.Default;
			
			dodgeState = DodgeState.Setup;
			break;
		}

	}

}
