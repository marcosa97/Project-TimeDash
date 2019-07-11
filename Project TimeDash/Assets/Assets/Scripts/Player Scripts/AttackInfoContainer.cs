using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

/// <summary>
///   These containers store the information of an attack performed
///   by either the player or an enemy. Copies of these objects will be passed
///   to whoever is on the receiving end of the attack, and the game object
///   attached to that unit will handle accordingly how it gets hurt
/// </summary>
public class AttackInfoContainer : MonoBehaviour {
	
	public AttackID AttackID;
	public AttackType attackType;

	//NOTES: 
	//@ The attack direction * force gets passed into AddForce
	//@ Direction vector relative to the entity 
	//    performing the attack (magnitude = 1)
	//@ Direction is in the format (1,0), (0,1), (-1, 0), (0,-1), etc.
	public Vector2 direction; 
	public float force; //To determine knockback, unmodified by power ups
	public int damage;

	//Default constructor
	public AttackInfoContainer() {
		AttackID = AttackID.Null;
		attackType = AttackType.Null;
		direction = Vector2.zero;
		force = 0f;
		damage = 0;
	}

	//Updates variables of this container
	public void UpdateAttackInfo(AttackID ID, float f, Vector2 dir, int dam) { 
		AttackID = ID;
		force = f; 
		direction = dir;
		damage = dam;
	}
	
}

public enum AttackType {
	Null,
	MeleeWeakAttack,
	MeleeStrongAttack
	//Stun
	//etc
}

public enum AttackID {
	Null,
	NormalAttack,
	ChargedAttack,
    WarpStrike,
	SprintAttack,
	Grab
}
