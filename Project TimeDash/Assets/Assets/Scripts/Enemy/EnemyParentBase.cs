using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyParentBase : MonoBehaviour {

	protected enum EnemyBaseState {
		Roaming, //Player not detected
		Pursuit, //Chasing player
		Searching, //When enemy loses sight of a player
		ChargingAttack, //Only some enemies can charge attack
		Attacking, //Performing Attack
		Hurt
		//Cutscene
	}

	[SerializeField]
	protected EnemyBaseState enemyState; 

	[Header ("Attack Settings")]
	public float baseAttackForce = 0f; //Assuming one attack
	public LayerMask whatToHit;

	[Header ("Movement Settings")]
	public float roamSpeed;
	public float pursuitSpeed;
	public float pursuitRange;
	public float stoppingDistance; //for when approaching player
	public float retreatDistance; //When player gets too close to unit
	public float flinchTime;
	public float searchTime;

	protected Rigidbody2D rb;
	protected AttackInfoContainer attackInfo;

	//Reference to player's health script
	//Reference to player's controller
	//Reference to animator

	// Use this for initialization
	protected virtual void Start () {
		//Get player's script and controller
		//Get animator component
		//anim = GetComponent<Animator>();
		Debug.Log("Base Start");

		//Initialize to default values
		attackInfo = GetComponent<AttackInfoContainer> ();
		attackInfo.attackType = AttackType.MeleeWeakAttack;
		attackInfo.direction = Vector2.zero;
		attackInfo.force = baseAttackForce;

		rb = GetComponent<Rigidbody2D> ();
		enemyState = EnemyBaseState.Roaming;
	}

	//NOTE: Update() will be used for checking 
	//      logic and switching states
	//      FixedUpdate() will be used for 
	//      moving the enemy unit

	// Update is called once per frame
	void Update () {

		switch (enemyState) {
		case EnemyBaseState.Roaming:
			Roam ();
			break;
	
		case EnemyBaseState.Pursuit:
			Pursuit ();
			break;

		case EnemyBaseState.Searching:
			Search ();
			break;

		case EnemyBaseState.ChargingAttack:
			ChargeAttack ();
			break;

		case EnemyBaseState.Attacking:
			Attack ();
			break;

		case EnemyBaseState.Hurt:
			Hurt ();
			break;
		}
	}

	void FixedUpdate() {

		switch (enemyState) {
		case EnemyBaseState.Roaming:
			FixedRoam ();
			break;

		case EnemyBaseState.Pursuit:
			FixedPursuit ();
			break;

		case EnemyBaseState.Searching:
			FixedSearch ();
			break;

		case EnemyBaseState.ChargingAttack:
			FixedChargeAttack ();
			break;

		case EnemyBaseState.Attacking:
			FixedAttack ();
			break;

		case EnemyBaseState.Hurt:
			FixedHurt ();
			break;
		}
	}

	//==================================//
	// FUNCTIONS FOR HANDLING STATES    //
	// Note: State switching has to be  //
	// handled in derived classes       //
	//==================================//

	protected virtual void Roam() {}
	protected virtual void Pursuit() {}
	protected virtual void Search() {}
	protected virtual void ChargeAttack() {}
	protected virtual void Attack() {}
	protected virtual void Hurt() {}

	//These functions are called in FixedUpdate() -> used for
	//moving enemy and dealing with physics

	protected virtual void FixedRoam() {}
	protected virtual void FixedPursuit() {}
	protected virtual void FixedSearch() {}
	protected virtual void FixedChargeAttack() {}
	protected virtual void FixedAttack() {}
	protected virtual void FixedHurt() {}

	//Function for when the enemy is hit
	protected virtual void ObjectHit(AttackInfoContainer obj) {
		//Debug.Log ("ENEMY HIT");
	}
}
