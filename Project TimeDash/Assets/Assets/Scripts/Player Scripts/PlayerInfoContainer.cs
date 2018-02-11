using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be used to store all info on the player, like movement direction and attack used
public class PlayerInfoContainer : MonoBehaviour {
//Private
	private AttackID attackPerformed;
	private float attackForce; //attack force modified by buff/debuff 
	private Vector2 directionFacing;

//Public
	// Use this for initialization
	void Start () {
		attackPerformed = AttackID.Null;
		attackForce = 0f;
		directionFacing = new Vector2 (0f, 0f);
	}

	public void UpdateAttackPerformed(AttackID ID, float force) {
		attackPerformed = ID;
		attackForce = force;
	}

	public void UpdateDirectionFacing(Vector2 dir) { 
		directionFacing = dir;
	}
		
	//=================== Getter Functions =======================
	public float GetAttackForce() {
		return attackForce;
	}

}
