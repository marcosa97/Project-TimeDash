using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Sword collider will be a Trigger collider because that way it can ignore the Player's
//        collision box and it won't push the player away either.
public class SwordCollider : MonoBehaviour {
	private List<int> targetsHit; //targets hit in one sword slash
	private bool attackHasEnded; 

	// Use this for initialization
	void Start () {
		//swordCollider = GetComponent<Collider2D> ();
		targetsHit = new List<int> ();
		attackHasEnded = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*
	//Ignore collisions with the player collider 
	// @other: the player
	void OnTriggerEnter (Collider player) {
		if (player.tag == "Player") {
			//Line below calls the function ApplyDamage(10)
			//other.gameObject.SendMessage("ApplyDamage", 10);
			return;
		}
	} */

	// @other: could be an enemy, object, or switch
	void OnTriggerStay2D (Collider2D other) {
		if (enabled) {
			if (other.tag == "Enemy") {
				//if the enemy that was attacked hasn't already been damaged during this attack
				if (!targetsHit.Contains (other.gameObject.GetInstanceID ())) {
					Debug.Log ("TARGET HIT : " + other.gameObject.GetInstanceID ());

					//damage the enemy
					other.gameObject.GetComponent<HealthManager> ().ReceiveDamage (10);

					//add enemy to list of enemies already damaged during this attack
					targetsHit.Add (other.gameObject.GetInstanceID ());
				}

				//if enemy has already been damaged, then do nothing

			}//if enemy hit

		} //if enabled

	}


	/*
	// @other: could be an enemy, object, or switch
	void OnCollisionEnter2D (Collision2D other) {
		Debug.Log ("COLLIDER ENABLED");
		if (enabled) {
			if (other.gameObject.tag == "Enemy") {
				//Line below calls the function ApplyDamage(10)
				//other.gameObject.SendMessage("ApplyDamage", 10);
				//other.gameObject.SetActive (false);
				other.gameObject.GetComponent<HealthManager>().ReceiveDamage(10);
			}
		}
	}
	*/

	public void AttackHasEnded() {
		attackHasEnded = true;
		if (targetsHit != null)
		    targetsHit.Clear ();
	}

	public void Disable() {
		enabled = false;
	}

	public void Enable() {
		enabled = true;
		attackHasEnded = false;
	}
}
