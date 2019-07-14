using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FOR TESTING HURT STATE FOR PLAYER
public class HurtPlayer : MonoBehaviour {

	private PlayerController playerController;
	private AttackInfoContainer attackInfo;

	// Use this for initialization
	void Start () {
		playerController = GameObject.Find ("Player").GetComponent<PlayerController> ();
		attackInfo = GetComponent<AttackInfoContainer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("CANCEL CALLED");

            /*
			attackInfo.attackType = AttackType.MeleeWeakAttack;
			playerController.HurtPlayer (attackInfo);
            */

            playerController.MakePlayerFall();
		}
	}
}
