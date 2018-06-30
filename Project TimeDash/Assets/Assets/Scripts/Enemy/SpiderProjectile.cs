using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderProjectile : MonoBehaviour {
	public float speed;
	public LayerMask whatToHit;
	public AttackInfoContainer attackInfo; 
	private GameObject player;

	private PlayerController playerController;
	private Transform playerTrans; 
	private Vector2 target;

	// Use this for initialization
	void Start () {
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
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards (transform.position, target, speed * Time.deltaTime);

		if (transform.position.x == target.x && transform.position.y == target.y) {
			//Destroy projectile
			DestroyProjectile();
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player" || other.tag == "Player Shield") {
			Debug.Log (other.tag);
			DestroyProjectile ();

			if (other.tag == "Player") {
				//Hurt player
				playerController.HurtPlayer (attackInfo);
			}
		}
	}

	void DestroyProjectile() {
		Destroy (gameObject);
	}
}
