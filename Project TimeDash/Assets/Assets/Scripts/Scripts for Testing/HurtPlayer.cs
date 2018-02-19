using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour {

	private PlayerController playerController;

	// Use this for initialization
	void Start () {
		playerController = GameObject.Find ("Player").GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("CANCEL CALLED");
			playerController.HurtPlayer (AttackType.MeleeWeakAttack);
			//playerController.gameObject.GetComponent<Rigidbody2D> ().
		}
	}
}
