﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour {
	public SpiderState spiderState;

	//public settings
	public float fireRate = 0f;
	public float baseAttackForce = 0f;
	public LayerMask whatToHit;
	public float pursuitRange;
	public float flinchTime;
	public float searchTime;

	float timeToFire = 0f;
	Transform firePoint;

	//For handling patrol/random movement
	public float patrolSpeed;
	public float pursuitSpeed;
	[SerializeField]
	private float hurtTimer;
	private float searchTimer;
	private float waitTime;
	public float startWaitTime;

	//List of spots that the enemy can move to when patroling
	public Transform[] moveSpots;
	private int targetSpot;

	private GameObject player;
	private Rigidbody2D rb;
	private AttackInfoContainer attackInfo; 

	// Use this for initialization
	void Start () {
		firePoint = transform.Find ("FirePoint");
		if (firePoint == null) {
			Debug.LogError ("No Firepoint!");
		}

		attackInfo = GetComponent<AttackInfoContainer> (); 
		attackInfo.attackType = AttackType.MeleeWeakAttack;
		attackInfo.direction = -transform.up.normalized;
		attackInfo.force = baseAttackForce;

		//Get reference of player
		player = GameObject.Find("Player");
		rb = GetComponent<Rigidbody2D> ();

		//Starting state
		spiderState = SpiderState.Roaming;
		waitTime = startWaitTime;
		//targetSpot = Random.Range (0, moveSpots.Length);
		targetSpot = 0; //first spot
	}

	//NOTE: Update() will be used for checking 
	//      distances and switching states
	//      FixedUpdate() will be used for 
	//      moving the enemy unit

	// Update is called once per frame
	void Update () {
		if (Time.time > timeToFire) {
			timeToFire = Time.time + (1 / fireRate);
			Shoot ();
		}

		//State machine goes here
		switch (spiderState) {
		case SpiderState.Roaming:
			Roam ();
			break;
		
		case SpiderState.Pursuit:
			Pursuit ();
			break;

		case SpiderState.Searching:
			Search ();
			break;

		case SpiderState.Attacking:
			break;

		case SpiderState.Hurt:
			Hurt ();
			break;
		}
	}

	//Will be used for handling enemy movement
	void FixedUpdate() {
		
		switch (spiderState) {
		case SpiderState.Roaming:
			PatrolMovement ();
			break;

		case SpiderState.Pursuit:
			PursuitMovement ();
			break;

		case SpiderState.Searching:
			//Do nothing for now
			break;

		case SpiderState.Attacking:
			break;

		case SpiderState.Hurt:
			//Do nothing
			break;
		}
	}

	///===============================///
	/// FUNCTIONS FOR HANDLING STATES ///
	///===============================///

	//In the hurt state, this enemy is flinching and won't move
	void Hurt() {
		hurtTimer -= Time.deltaTime;

		if (hurtTimer <= 0f) {
			hurtTimer = 0f;
			spiderState = SpiderState.Pursuit;
		}
	}

	//If the player runs away from the enemy, the enemy 
	//will stop pursuing and pause for a bit, searching for
	//the player and then return roaming if player not found
	void Search() {
		searchTimer -= Time.deltaTime;

		if (searchTimer <= 0f) {
			searchTimer = 0f;
			spiderState = SpiderState.Roaming;
		}
	}

	void Pursuit() {
		//Check radius to see if player out of range
		float distance = Vector2.Distance (transform.position, player.transform.position);
		if (distance > pursuitRange) { 
			//Player has escaped enemy
			spiderState = SpiderState.Searching;
			searchTimer = searchTime;
		}
	}

	void Roam() {
		//Check radius to see if player is within range
		float distance = Vector2.Distance (transform.position, player.transform.position);
		if (distance <= pursuitRange) { 
			//If player is within range, switch to pursuit state
			spiderState = SpiderState.Pursuit;
		} 
	}

	void PursuitMovement() {
		//Follow player
		Vector2 moveDirection = (Vector2)player.transform.position - (Vector2)transform.position;
		moveDirection.Normalize();
		rb.velocity = moveDirection * pursuitSpeed;
	}

	//Called in FixedUpdate cuz we deal with rigidbody movement
	void PatrolMovement() {
		//Move towards target spot
		Vector2 newPos = Vector2.MoveTowards(transform.position, moveSpots[targetSpot].position, Time.fixedDeltaTime * patrolSpeed);
		rb.MovePosition(newPos);

		//Handle patrol movement timings
		//If enemy has reached move spot
		if (Vector2.Distance(transform.position, moveSpots[targetSpot].position) < 0.2f) {
			if (waitTime <= 0f) {
				//targetSpot = Random.Range (0, moveSpots.Length);
				//Cycle through move spots in order
				targetSpot = ( targetSpot + 1) % moveSpots.Length;
				waitTime = startWaitTime;
			} else {
				//Time.deltaTime is already fixedDeltaTime in FixedUpdate
				waitTime -= Time.fixedDeltaTime;
			}
		}
	}

	void Shoot() {
		//Debug.Log ("Turret Fire!");
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, Vector2.down, Mathf.Infinity, whatToHit);
		Debug.DrawRay (firePointPosition, Vector2.down);

		if (hit.collider != null) {
			//Debug.Log ("Raycast Hit");
			Debug.DrawLine (firePointPosition, hit.point, Color.red);
			Debug.Log ("We hit " + hit.collider.name);

			//Check if the object hit was the player or the shield
			if (hit.collider.tag == "Player") {
				hit.collider.SendMessage ("HurtPlayer", attackInfo); 
			} else if (hit.collider.tag == "Player Shield") {

			}

		}
	}

	//@obj: contains attack direction and attack force
	void ObjectHit(AttackInfoContainer obj) {
		Debug.Log ("ENEMY HIT");

		//Change state to hurt
		hurtTimer = flinchTime;
		spiderState = SpiderState.Hurt;

		rb.AddForce (obj.direction * obj.force );
	}

	//For debugging
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, pursuitRange);
	}
}

//State Machine states for this enemy
// -> WILL NEED TO CREATE A BASE STATE MACHINE FOR MORE ENEMIES
public enum SpiderState {
	Roaming,
	Pursuit,
	Searching, //When enemy loses sight of player
	Attacking,
	Hurt
}