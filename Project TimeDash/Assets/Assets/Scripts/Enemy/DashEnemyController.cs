using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemyController : EnemyParentBase {

	private DashEnemyAttackCollider colliderScript;
	private Collider2D bodyCollider; 
	private Collider2D playerCollider; 

	// Use this for initialization
	protected override void Start () {
		//pc = playerObject.GetComponent<PlayerController> ();
		base.Start ();
		colliderScript = GetComponentInChildren<DashEnemyAttackCollider> ();
		//colliderScript.DisableAttackCollider ();
		bodyCollider = GetComponent<Collider2D> ();
		playerCollider = GameObject.Find ("Player").GetComponent<Collider2D> ();
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

	//Override pursuit to customize pursuit state
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
		if (distance <= stoppingDistance) {
			//Begin charging an attack
			pursuitState = PursuitState.Stopped;
			enemyState = EnemyBaseState.ChargingAttack;
			timer = 0f; //reset
			attackTimer = attackChargeTime;

			//Face the target as well

		} else {
			//Keep chasing
			pursuitState = PursuitState.Chasing;
		}
	}

	protected override void ChargeAttack() {
		//When attack charge animation is over, 
		//switch to attack state
		attackTimer -= Time.deltaTime;

		if (attackTimer <= 0f) {
			attackTimer = attackDurationTime;
			enemyState = EnemyBaseState.Attacking;

			//Get player's position at this point in time
			Vector2 attackDirection = (Vector2)playerTransform.position - (Vector2)transform.position;
			attackDirection.Normalize ();
			attackInfo.direction = attackDirection;
			colliderScript.EnableAttackCollider ();

			//Ignore collisions between enemy body and player
			Physics2D.IgnoreCollision(bodyCollider, playerCollider, true);
		}
	}

	//Override so we can use colliders
	protected override void Attack() {

		switch (attackState) {
		case AttackState.Attacking:
			//NOTE: Will have to change this so that
			//      we switch out of this state when 
			//      the attack animation is over.

			attackTimer -= Time.deltaTime;

			if (attackTimer <= 0f) {
				attackTimer = 0f; //reset
				attackState = AttackState.Cooldown;
				timer = attackCooldownTime;
				colliderScript.DisableAttackCollider ();
				Physics2D.IgnoreCollision(bodyCollider, playerCollider, false);
			}

			break;

		case AttackState.Cooldown:
			//When cooldown animation is over, switch to pursuit state
			//For this, we use the regular "timer" instead of 
			//attack timer because cooldown can be interrupted
			//by the player attacking
			timer -= Time.deltaTime;

			if (timer <= 0f) {
				timer = 0f; //reset
				attackState = AttackState.Attacking; //reset
				enemyState = EnemyBaseState.Pursuit;
			}

			break;
		}
	}

	protected override void FixedAttack() {
		switch(attackState) {
		case AttackState.Attacking:
			rb.velocity = attackInfo.direction * attackSpeed;
			break;
		
		case AttackState.Cooldown:
			rb.velocity = Vector2.zero;
			break;
		}
	}

	/*
	void OnCollisionEnter2D(Collision2D other) {
		//Only check collisions when in attacking state
		if (enemyState == EnemyBaseState.Attacking && attackState == AttackState.Attacking) {
			if (other.gameObject.tag == "Player") {
				Debug.Log ("Dash hit player");
				playerController.HurtPlayer (attackInfo);
			}
		}
	}
	*/
}
