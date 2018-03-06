using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Sword collider will be a Trigger collider because that way it can ignore the Player's
//        collision box and it won't push the player away either.

//NOTE: OnColliderEnter2D and OnTriggerEnter2D did not work as intended
//        because of how I decided to handle collider collisions (disabling,
//        and enabling colliders)
public class SwordCollider : MonoBehaviour {
	private List<int> targetsHit; //targets hit in one sword slash
	private Collider2D attachedCollider;
	private AttackAbility attackScript;
	private AbilityBasicMovement moveInfo;
	private TimeFunctions timeManager;
	private GameObject hitParticles;
	private AttackInfoContainer playerAttackInfo;

	// Use this for initialization
	void Start () {
		//swordCollider = GetComponent<Collider2D> ();
		attachedCollider = GetComponent<PolygonCollider2D> ();
		if (attachedCollider == null)
			Debug.Log ("MISSING COLLIDER REF");
		Debug.Log ("GOT COLLIDER REF");
		targetsHit = new List<int> ();
		moveInfo = GetComponentInParent<AbilityBasicMovement> ();
		playerAttackInfo = GetComponentInParent<AttackInfoContainer> ();
		timeManager = GameObject.Find("Time Manager").GetComponent<TimeFunctions> ();
		hitParticles = GameObject.Find ("HitParticles");
		DisableCollider ();
	}

	// @other: could be an enemy, object, or switch
	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Interactable Object") {
			//if the enemy that was attacked hasn't already been damaged during this attack
			// AKA, Hit Registered
			if (!targetsHit.Contains (other.gameObject.GetInstanceID ())) {
				//Debug.Log ("TARGET HIT : " + other.gameObject.GetInstanceID ());

				//damage the enemy
				//other.gameObject.GetComponent<HealthManager> ().ReceiveDamage (10);

				//Get attack info from the attack info object attached to the player,
				//  and send it to the other object to process	
				other.SendMessage("ObjectHit", playerAttackInfo );

				//Call Time.Timescale using SendMessage or a reference to time manager
				timeManager.StartCoroutine("HitStop");

				//Instantiate particle effect
				var effect = Instantiate(hitParticles, other.transform.position, other.transform.rotation);
				Destroy (effect, .5f);

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

	}

	//================FOR CONTROLLING COLLIDER==============

	/// <summary>
	/// Disables the collider attached to 
	/// to the same object as this script.
	/// Also clears the list of targets that
	/// were hit
	/// </summary>
	public void DisableCollider() {
		if (attachedCollider != null)
			attachedCollider.enabled = false;

		//Clear list of targets hit
		if (targetsHit != null)
			targetsHit.Clear ();
	}

	/// <summary>
	/// Enables the collider attached
	/// to the same object as this script.
	/// </summary>
	public void EnableCollider() {
		attachedCollider.enabled = true;
	}
}
