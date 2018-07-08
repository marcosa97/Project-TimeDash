using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemyController : EnemyParentBase {

	// Use this for initialization
	protected override void Start () {
		base.Start ();
		Debug.Log ("Derived Start");
	}
		
	//FUNCTIONS FROM PARENT CLASS FOR HANDLING STATES
	//Note: Will have to handle state switching in here

	//protected virtual void Roam() {}
	//protected virtual void Pursuit() {}
	//protected virtual void Search() {}
	//protected virtual void ChargeAttack() {}
	//protected virtual void Attack() {}
	//protected virtual void Hurt() {}

	//These functions are called in FixedUpdate() -> used for
	//moving enemy and dealing with physics

	//protected virtual void FixedRoam() {}
	//protected virtual void FixedPursuit() {}
	//protected virtual void FixedSearch() {}
	//protected virtual void FixedChargeAttack() {}
	//protected virtual void FixedAttack() {}
	//protected virtual void FixedHurt() {}

	/*
	protected override void Pursuit() {
		//Check radius to see if player got out of range
		float distance = Vector2.Distance (transform.position, playerTransform.position);
		if (distance > pursuitRange) {
			//Player has escaped enemy
			enemyState = EnemyBaseState.Searching;
			pursuitState = PursuitState.Chasing; //reset
			timer = searchTime;
		}

		//Handle pursuit substate transitions
		//Check if we are within attacking distance and not too close to player
		if (distance <= stoppingDistance && distance >= retreatDistance) {
			//Begin charging an attack
			pursuitState = PursuitState.Stopped;
			enemyState = EnemyBaseState.ChargingAttack;
			timer = 0f; //reset
			attackTimer = attackChargeTime;

			//Face the target as well

		} else if (distance < retreatDistance) {
			//If too close to player, retreat
			pursuitState = PursuitState.Retreating;
		} else {
			//Keep chasing
			pursuitState = PursuitState.Chasing;
		}
	}
	*/

	protected override void FixedAttack() {
		rb.velocity = attackInfo.direction * attackSpeed;
	}
}
