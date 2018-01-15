using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LinkController : MonoBehaviour {

	public float moveSpeed;
	private Rigidbody2D myRigidbody;
	private bool isMoving;

	//These are to get the enemy to move in time intervals
	public float timeBetweenMove;
	private float timeBetweenMoveCounter;
	public float timeToMove;
	private float timeToMoveCounter;

	private Vector3 moveDirection;

	//For when player dies
	public float waitToReload;
	private bool reloading;
	private GameObject player;

	//bool to avoid continuous damage from one attack
	//public bool wasHit;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();

		//timeBetweenMoveCounter = timeBetweenMove;
		//timeToMoveCounter = timeToMove;

		timeBetweenMoveCounter = Random.Range(timeBetweenMove * 0.75f, timeBetweenMove * 1.25f);
		timeToMoveCounter = Random.Range (timeToMove * 0.75f, timeToMove * 0.75f);
	}
	
	// Update is called once per frame
	void Update () {
		
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

		if (reloading) {
			waitToReload -= Time.deltaTime;
			if (waitToReload < 0) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				player.SetActive (true);
			}
		}
	}

	//Called when 2 objects with colliders attached to them meet
	//Other object is going to be Player
	void OnCollisionEnter2D (Collision2D other) {
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
	}


}
