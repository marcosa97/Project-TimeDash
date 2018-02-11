using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

//Each attack script will contain one of these to store info on attack
public class AttackInfoContainer {
	//Private
	private AttackID AttackID;
	//private int damage;
	private float baseAttackForce; //To determine knockback, unmodified by power ups

	//Public
	public AttackInfoContainer(AttackID ID, float f) { 
		AttackID = ID;
		baseAttackForce = f; 
	}
		
	public AttackID GetAttackID() {
		return AttackID;
	}

	public float GetForce() { 
		return baseAttackForce; 
	} 
	
}

public enum AttackID {
	Null,
	NormalAttack,
	ChargedAttack,
	SprintAttack
}
