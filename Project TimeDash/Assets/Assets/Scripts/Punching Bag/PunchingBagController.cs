using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagController : MonoBehaviour {
	//Public settings

	//References and variables needed
	private bool isHit;
	private Animator anim;
	private Rigidbody2D body;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		body = GetComponent<Rigidbody2D> ();
	}

	//Pass in an object AttackInfo that contains attack direction and attack force
	void ObjectHit(AttackInfoContainer obj) {
		Debug.Log ("PUNCHING bag HIT");
		isHit = true;
		anim.SetBool ("Hit", isHit);

		body.AddForce (obj.direction * obj.force );
	}

}
