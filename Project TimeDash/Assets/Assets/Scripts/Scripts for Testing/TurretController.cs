using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {

	public float fireRate = 0f;
	public float baseAttackForce = 0f;
	public LayerMask whatToHit;

	float timeToFire = 0f;
	Transform firePoint;

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
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > timeToFire) {
			timeToFire = Time.time + (1 / fireRate);
			Shoot ();
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
}
