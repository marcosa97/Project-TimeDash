using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProjectile : MonoBehaviour {
	public float speed;
	public float lifeTime;
	public LayerMask whatToHit;
	public AttackInfoContainer attackInfo; 
	private GameObject player;

	private PlayerController playerController;
	private Transform playerTrans; 
	private Vector2 target;
	private float timer; //For how long a bullet lasts before it's destroyed

	// Use this for initialization
	void Start () {
		timer = lifeTime;
		player = GameObject.FindGameObjectWithTag ("Player");
		playerController = player.GetComponent<PlayerController> ();
		playerTrans = player.transform;
		target = new Vector2 (playerTrans.position.x, playerTrans.position.y);

		//Create vector from transform position to attack direction
		Vector2 dir = (Vector2)playerTrans.position - (Vector2)transform.position;
		dir.Normalize();

		//Set attack stats
		attackInfo.direction = dir;
		Debug.Log (attackInfo.direction);
		target = dir * 10;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if (timer <= 0f) {
			DestroyProjectile ();
		} else {
			transform.position = Vector2.MoveTowards (transform.position, target, speed * Time.deltaTime);
		}

		//if (transform.position.x == target.x && transform.position.y == target.y) {
			//Destroy projectile
		//	DestroyProjectile();
		//}
	}

	void OnTriggerEnter2D(Collider2D other) {
		//Ignore collisions with itself
		if (other.tag == "Enemy Unit Medium") {
			return;
		}

		//Reflect
		if (other.tag == "Sword") {
			target = -target;
			return;
		}

		DestroyProjectile ();

		if (other.tag == "Player" || other.tag == "Player Shield") {
			Debug.Log (other.tag);

			if (other.tag == "Player") {
				//Hurt player
				playerController.HurtPlayer (attackInfo);
			} else {
				//Hit shield

			}
		}
	}

	void DestroyProjectile() {
		Destroy (gameObject);
	}
}
