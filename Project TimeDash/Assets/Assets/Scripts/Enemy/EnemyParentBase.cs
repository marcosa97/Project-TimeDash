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
	[Header ("Speed")]
	public float roamSpeed;
	public float pursuitSpeed;
	[Header ("Ranges")]
	public float pursuitRange;
	public float stoppingDistance; //for when approaching player
	public float retreatDistance; //When player gets too close to unit
	[Header ("Time Durations")]
	public float roamPauseTime = 1.5f; //Time to pause movement for when roaming
	public float flinchTime = 0.5f;
	public float searchTime = 3f;

	//List of spots that the enemy can move to when patroling
	public Transform[] moveSpotsArray;
	protected int targetSpotIndex;

	protected Rigidbody2D rb;
	protected Transform playerTransform;
	protected AttackInfoContainer attackInfo;
	protected float timer;

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

		playerTransform = GameObject.Find ("Player").GetComponent<Transform> ();
		rb = GetComponent<Rigidbody2D> ();
		enemyState = EnemyBaseState.Roaming;
		timer = 0f;
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

	protected virtual void Roam() {
		//Check radius to see if player is within range
		float distance = Vector2.Distance (transform.position, playerTransform.position);
		if (distance <= pursuitRange) {
			//If within range, switch to pursuit state
			enemyState = EnemyBaseState.Pursuit;
		}
	}

	protected virtual void Pursuit() {
		//Check radius to see if player got out of range
		float distance = Vector2.Distance (transform.position, playerTransform.position);
		if (distance > pursuitRange) {
			//Player has escaped enemy
			enemyState = EnemyBaseState.Searching;
			timer = searchTime;
		}

		//Handle pursuit substates
		//Check if we are within attacking distance and not too close to player
		if (distance <= stoppingDistance && distance >= retreatDistance) {
			//Stop moving for a better attack shot
			//pState = PursuitState.Stopped;

			//Face the target as well

		} else if (distance < retreatDistance) {
			//If too close to player, retreat
			//pState = PursuitState.Retreating;
		} else {
			//Keep chasing
			//pState = PursuitState.Chasing;
		}
	}

	protected virtual void Search() {
		timer -= Time.deltaTime;

		if (timer <= 0f) {
			timer = roamPauseTime;
			enemyState = EnemyBaseState.Roaming;
		}

		//Check if player is within range
		float distance = Vector2.Distance (transform.position, playerTransform.position);
		if (distance <= pursuitRange) { 
			//If player is within range, switch to pursuit state
			enemyState = EnemyBaseState.Pursuit;
			timer = 0f;
		}
	}
	protected virtual void ChargeAttack() {}
	protected virtual void Attack() {}
	protected virtual void Hurt() {
		timer -= Time.deltaTime;

		if (timer <= 0f) {
			timer = 0f;
			enemyState = EnemyBaseState.Pursuit;
		}
	}

	//These functions are called in FixedUpdate() -> used for
	//moving enemy and dealing with physics

	protected virtual void FixedRoam() {
		//Move towards target spot
		Vector2 newPos = Vector2.MoveTowards (transform.position, 
			                 moveSpotsArray [targetSpotIndex].position,
			                 Time.fixedDeltaTime * roamSpeed);
		rb.MovePosition (newPos);

		//Handle roam (patrol) movement timings
		//If enemy has reached move spot
		if (Vector2.Distance (transform.position, moveSpotsArray [targetSpotIndex].position) < 0.2f) {
			if (timer <= 0f) {
				//Cycle through move spots in order
				targetSpotIndex = (targetSpotIndex + 1) % moveSpotsArray.Length;
				timer = roamPauseTime;
			} else {
				//Time.deltaTime is already fixedDeltaTime in FixedUpdate
				timer -= Time.fixedDeltaTime;
			}
		}
	}
	protected virtual void FixedPursuit() {
		Vector2 moveDirection = Vector2.zero;

		//Handle sub states movement
	}
	protected virtual void FixedSearch() {}
	protected virtual void FixedChargeAttack() {}
	protected virtual void FixedAttack() {}
	protected virtual void FixedHurt() {}

	//Function for when the enemy is hit
	protected virtual void ObjectHit(AttackInfoContainer obj) {
		Debug.Log ("DASH ENEMY HIT");

		//Change to hurt state
		timer = flinchTime;
		enemyState = EnemyBaseState.Hurt;

		rb.AddForce (obj.direction * obj.force);
	}

	//For debugging
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, pursuitRange);
	}
}
