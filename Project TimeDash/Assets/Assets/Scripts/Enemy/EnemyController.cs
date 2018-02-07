using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour {

	public float moveSpeed;
	private Rigidbody2D myRigidbody;
	private Animator anim;
	private bool isMoving;

	//These are to get the enemy to move in time intervals
	public float timeBetweenMove;
	private float timeBetweenMoveCounter;
	public float timeToMove;
	private float timeToMoveCounter;

	private Vector3 moveDirection; 

	//For attacking player
	public float PursuitRange; //set in inspector
	public float AttackRange;

	//For when player dies
	public float waitToReload;
	private bool reloading;
	private GameObject player;

	//TEMPORARY
	public GameObject damagerBurst;

	// Use this for initialization
	void Start () {
		player = PlayerManager.instance.player;
		myRigidbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		//timeBetweenMoveCounter = timeBetweenMove;
		//timeToMoveCounter = timeToMove;

		timeBetweenMoveCounter = Random.Range(timeBetweenMove * 0.75f, timeBetweenMove * 1.25f);
		timeToMoveCounter = Random.Range (timeToMove * 0.75f, timeToMove * 0.75f);
	}

	// Update is called once per frame
	void Update () {

		//If enemy is close enough to player
		if (PlayerIsWithinRange (PursuitRange)) {
            
			if (PlayerIsWithinRange(AttackRange)) {
				anim.SetBool ("EnemyAttack", true);
			} else {
			    anim.SetBool ("EnemyPursuit", true);
			    //move towards player
		    	//moveDirection = new Vector3(player.transform.position.x * moveSpeed * Time.deltaTime, player.transform.position.y * moveSpeed * Time.deltaTime);
			    moveDirection = transform.InverseTransformPoint(player.transform.position);
			    myRigidbody.velocity = moveDirection;
			}
		} else {
			anim.SetBool("EnemyAttack", false);		
			anim.SetBool ("EnemyPursuit", false);
			RandomEnemyMovement ();
		}

		if (reloading) {
			waitToReload -= Time.deltaTime;
			if (waitToReload < 0) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				player.SetActive (true);
			}
		}
	}

	//Checks if the player is within the range specified
	// Changes 
	bool PlayerIsWithinRange(float range) {
		float distance = Vector3.Distance (transform.position, player.transform.position);

		if (distance <= range) {
			//Debug.Log ("DISTANCE: " + distance);
			return true;
			//moveDirection = player.transform.position;
			//myRigidbody.velocity = moveDirection;
		} else {
			return false;
		}
	}

	void RandomEnemyMovement() {
		//If enemy is currently moving
		if (isMoving) {
			timeToMoveCounter -= Time.deltaTime;
			myRigidbody.velocity = moveDirection;

			if (timeToMoveCounter < 0f) {
				isMoving = false;
				//timeBetweenMoveCounter = timeBetweenMove;
				timeBetweenMoveCounter = Random.Range(timeBetweenMove * 0.75f, timeBetweenMove * 1.25f);
			}
		} 
		else {
			//Time.deltaTime is the time it takes to make one update
			timeBetweenMoveCounter -= Time.deltaTime;
			myRigidbody.velocity = Vector2.zero;

			if (timeBetweenMoveCounter < 0f) {
				isMoving = true;
				//timeToMoveCounter = timeToMove;
				timeToMoveCounter = Random.Range (timeToMove * 0.75f, timeToMove * 0.75f);

				moveDirection = new Vector3(Random.Range(-1f, 1f) * moveSpeed, 
					Random.Range(-1f, 1f) * moveSpeed, 0f);
			}
		}
	}

	//Called when 2 objects with colliders attached to them meet
	//Other object is going to be Player
	void OnCollisionEnter2D (Collision2D other) {
		Debug.Log ("ENTERED SWORD COLLIDER");
		if (other.gameObject.tag == "Player") {
			/*
			//Deactivate player
			other.gameObject.SetActive(false);
			reloading = true;
			player = other.gameObject;
			*/

			//Damage player
			//10 is placeholder for now
			//other.gameObject.GetComponent<HealthManager> ().ReceiveDamage (10);

		}

		if (other.gameObject.tag == "Sword") {
			Instantiate (damagerBurst, transform.position, transform.rotation);
		}
			
	}
}
