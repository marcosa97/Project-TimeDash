using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityChargedAttack : MonoBehaviour {

	private enum ChargeAttackState {
		Setup,
		Charging,
		Attacking,
		Cooldown
	}

	//public settings
	public float maxChargeTime;
	public float attackTime;
	public float cooldownTime;

	//References and variables needed
	private float timer;
	private ChargeAttackState chargeAttackState;
	private PlayerOrientation playerOrientation;
	private PlayerDirections playerDirection; //enum
	private AbilityBasicMovement movementInfo;
	private Rigidbody2D playerBody;
	private bool playerChargeAttacking;

	//Colliders


	// Use this for initialization
	void Start () {
		playerOrientation = GetComponent<PlayerOrientation> ();
		movementInfo = GetComponent<AbilityBasicMovement> ();
		playerBody = GetComponent<Rigidbody2D> ();
		chargeAttackState = ChargeAttackState.Setup;
	}

	public void ChargeAttack(ref PlayerState playerState, Vector2 attackDirection) {
		switch (chargeAttackState) {
		case ChargeAttackState.Setup:
			timer = maxChargeTime;
			chargeAttackState = ChargeAttackState.Charging;
			playerBody.velocity = Vector2.zero;
			break;

		case ChargeAttackState.Charging:
			timer -= Time.deltaTime;

			//If timer is up or player releases button, then perform attack
			if (timer <= 0f || (Input.GetButtonUp ("AttackPS4")) ) {
				timer = attackTime;
				chargeAttackState = ChargeAttackState.Attacking;
			}

			if (Input.GetButton ("AttackPS4")) {
				//Increase damage that will be dealt
				break;
			}

			//If interrupted (enemy hits player), reset stuff
			
			break;

		case ChargeAttackState.Attacking:
			timer -= Time.deltaTime;

			//Activate colliders

			if (timer <= 0f) {
				timer = cooldownTime;
				chargeAttackState = ChargeAttackState.Cooldown;
			}
			break;

		case ChargeAttackState.Cooldown:
			timer -= Time.deltaTime;

			if (timer <= 0f) {
				//Reset stuff
				timer = 0f;
				chargeAttackState = ChargeAttackState.Setup;
				playerState = PlayerState.Default;
			}

			break;
		}
	}

}
