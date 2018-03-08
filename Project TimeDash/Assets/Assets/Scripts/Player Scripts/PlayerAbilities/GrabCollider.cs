using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCollider : MonoBehaviour {
	public AbilityGrab playerGrabAbility;
	public Transform holdPosition;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Enemy Unit Medium") {
			//Tell the enemy unit to switch to "grabbed" state
			//  and also get info from enemy, like how long it can be held for

			//Tell the player's state machine that object has been grabbed
			//Pass in an object containing a time as well
			playerGrabAbility.EnemyDetected(1f);

			//Move enemy's transform position to the grab position
			other.gameObject.transform.position = holdPosition.position;

		}
	}
}
