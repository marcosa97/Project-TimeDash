using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Same as Attack Info Container, but this one is used to store an enemy attack, 
//  so that the unit on the receiving end of the attack can
//  process the attack info and react accordingly
public class HurtInfoReceiver : MonoBehaviour {

	public AttackID AttackID;
	public AttackType attackType;
	public Vector2 direction; //Direction vector relative to the entity performing the attack (magnitude = 1)
	public float force; //To determine knockback, unmodified by power ups
	//public int damage;

	//Default constructor
	public HurtInfoReceiver() { 
		AttackID = AttackID.Null;
		attackType = AttackType.Null;
		direction = Vector2.zero;
		force = 0f;
	}

	//Updates variables of this container
	public void UpdateHurtInfo(AttackInfoContainer enemyAttack) { 
		AttackID = enemyAttack.AttackID;
		attackType = enemyAttack.attackType;
		force = enemyAttack.force; 
		direction = enemyAttack.direction;
	}

}
