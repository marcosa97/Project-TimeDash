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



	protected override void ObjectHit(AttackInfoContainer obj) {
		Debug.Log ("DASH ENEMY HIT");

		//Change to hurt state

		rb.AddForce (obj.direction * obj.force);
	}
}
