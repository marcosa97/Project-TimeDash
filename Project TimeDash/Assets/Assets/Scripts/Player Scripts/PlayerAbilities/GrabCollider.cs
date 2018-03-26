using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCollider : MonoBehaviour {
	public AbilityBasicMovement moveInfo;
	public AbilityGrab playerGrabAbility;
	public Transform holdPosition;
	private OrientationSystem oSystem;

	// Use this for initialization
	void Start () {
		oSystem = GetComponentInParent<OrientationSystem> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Enemy Unit Medium") {
			//Tell the enemy unit to switch to "grabbed" state
			//  and also get info from enemy, like how long it can be held for

			//Tell the player's state machine that object has been grabbed
			//Pass in an object containing a time as well
			playerGrabAbility.EnemyDetected(1f);
			playerGrabAbility.RetreiveEnemyBodyCollider (other);


			//Move enemy's transform position to the grab position
			ModifyHoldPosition();
			other.gameObject.transform.position = holdPosition.position;
		}
	}

	/// <summary>
	///    Modifies the hold position to the direction the player is 
	///    facing when performing a grab.
	/// </summary>
	void ModifyHoldPosition () {
		switch (oSystem.DetermineDirectionFromVector( moveInfo.GetLastMove() )) {
		case EightDirections.North:
			holdPosition.localPosition = new Vector2 (0, .15f);
			break;
		case EightDirections.NorthEast:
			holdPosition.localPosition = new Vector2 (.106f, .106f);
			break;
		case EightDirections.East:
			holdPosition.localPosition = new Vector2 (.15f, 0);
			break;
		case EightDirections.SouthEast:
			holdPosition.localPosition = new Vector2 (.106f, -.106f);
			break;
		case EightDirections.South:
			holdPosition.localPosition = new Vector2 (0, -.15f);
			break;
		case EightDirections.SouthWest:
			holdPosition.localPosition = new Vector2 (-.106f, -.106f);
			break;
		case EightDirections.West:
			holdPosition.localPosition = new Vector2 (-.15f, 0);
			break;
		case EightDirections.NorthWest:
			holdPosition.localPosition = new Vector2 (-.106f, .106f);
			break;
		}
	}
}
