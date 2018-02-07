using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Sword collider will be a Trigger collider because that way it can ignore the Player's
//        collision box and it won't push the player away either.
public class SwordCollider : MonoBehaviour {
	private List<int> targetsHit; //targets hit in one sword slash
	private bool attackHasEnded; 
	private AttackAbility attackScript;
	private AbilityBasicMovement moveInfo;

	// Use this for initialization
	void Start () {
		//swordCollider = GetComponent<Collider2D> ();
		targetsHit = new List<int> ();
		attackHasEnded = true;
		moveInfo = GetComponentInParent<AbilityBasicMovement> ();
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
			if (other.tag == "Interactable Object") {
				//if the enemy that was attacked hasn't already been damaged during this attack
				if (!targetsHit.Contains (other.gameObject.GetInstanceID ())) {
					//Debug.Log ("TARGET HIT : " + other.gameObject.GetInstanceID ());

					//damage the enemy
					//other.gameObject.GetComponent<HealthManager> ().ReceiveDamage (10);
					float force = 150;
					AttackInfoObject obj = new AttackInfoObject (force, moveInfo.GetLastMove ().normalized);
					other.SendMessage("ObjectHit", obj );

					//Implement Hit lag here -> Create function that stops the player and the object(s) hit
					//                          from moving
					//What is needed for Hit Stop:
						//Stop animation
						//Stop player and object movement

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

//Object
//Contains the force and direction of an attack
public class AttackInfoObject {
//Private
	private float force; //Attack strength
	private Vector2 direction; //Direction created from GetAxis
//Public
	public AttackInfoObject(float f, Vector2 v) {
		force = f;
		direction = v.normalized;
	}

	public Vector2 GetDirection() {
		return direction;
	}

	public float GetForce() {
		return force;
	}
}
