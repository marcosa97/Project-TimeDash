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
	private bool attackHasEnded; 
	private AttackAbility attackScript;
	private AbilityBasicMovement moveInfo;
	private GameObject obj; //set in inspector
	private TimeFunctions timeManager;
	private GameObject hitParticles;
	private AttackInfoContainer playerAttackInfo;

	// Use this for initialization
	void Start () {
		//swordCollider = GetComponent<Collider2D> ();
		targetsHit = new List<int> ();
		attackHasEnded = true;
		moveInfo = GetComponentInParent<AbilityBasicMovement> ();
		playerAttackInfo = GetComponentInParent<AttackInfoContainer> ();
		obj = GameObject.Find ("Time Manager");
		timeManager = obj.GetComponent<TimeFunctions> ();
		hitParticles = GameObject.Find ("HitParticles");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// @other: could be an enemy, object, or switch
	void OnTriggerStay2D (Collider2D other) {
		if (enabled) {
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

		} //if enabled

	}

	//================FOR CONTROLLING COLLIDER==============

	/// <summary>
	///   This function clears the "targetsHit" list.
	///   This will be called at the end of an attack
	/// </summary>
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
