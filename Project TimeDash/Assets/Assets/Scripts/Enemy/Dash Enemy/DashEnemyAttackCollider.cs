using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemyAttackCollider : MonoBehaviour {

	public DashEnemyController controllerScript;
	private PlayerController playerController;
	private AttackInfoContainer attackInfo;
	private CircleCollider2D attackCollider;
	private bool hitPlayer; //To avoid hitting multiple times
	                        //in one attack

	// Use this for initialization
	void Start () {
		hitPlayer = false;
		attackCollider = GetComponent<CircleCollider2D> ();
		playerController = GameObject.Find ("Player").GetComponent<PlayerController> ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		//Only check collisions when in attacking state
		if (other.tag == "Player" && hitPlayer == false) {
			//Get attack info
			attackInfo = GetComponentInParent<AttackInfoContainer> ();

			Debug.Log ("Dash hit player");
			playerController.HurtPlayer (attackInfo);
			hitPlayer = true;
			return;
		}

		//If the player's shield was hit, stop attack
		if (other.tag == "Player Shield") {
			//controllerScript.AttackBlocked ();

			//Instantiate some particles
		}
	}

	public void EnableAttackCollider() {
		attackCollider.enabled = true;
	}

	public void DisableAttackCollider() {
		hitPlayer = false;
		attackCollider.enabled = false;
	}
}
