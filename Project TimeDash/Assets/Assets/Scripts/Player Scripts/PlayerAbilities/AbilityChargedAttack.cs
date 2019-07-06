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
	[Header ("Charged Attack Settings")]
	public float maxChargeTime;
	public float attackTime;
	public float cooldownTime;
	public float baseAttackForce;
	public int chargeMultiplier;
	public int damageAmount;

	//References and variables needed
	private float timer;
	private float finalAttackForce; //after modifications applied
	private ChargeAttackState chargeAttackState;
	private PlayerOrientation playerOrientation;
	private PlayerDirections playerDirection; //enum
	private AbilityBasicMovement movementInfo;
	private Rigidbody2D playerBody;
	private SpriteRenderer spriteRenderer; //needed to make sprite flash
	private Animator animator;
	private OrientationSystem orientationSystem;
	private EightDirections playerFaceDirection; //enum
	private Vector2 lastAttackDirection; 
	private bool playerChargeAttacking;

	private AttackInfoContainer playerAttackInfo;

	//Collider scripts
	private SwordCollider colliderUp;
	private SwordCollider colliderDown;
	private SwordCollider colliderLeft;
	private SwordCollider colliderRight;

	// Use this for initialization
	void Start () {
		playerAttackInfo = GetComponent<AttackInfoContainer> ();
		playerOrientation = GetComponent<PlayerOrientation> ();
		movementInfo = GetComponent<AbilityBasicMovement> ();
		playerBody = GetComponent<Rigidbody2D> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		orientationSystem = GetComponent<OrientationSystem> ();
		chargeAttackState = ChargeAttackState.Setup;

		colliderUp = GameObject.Find ("cAttack Collider Up").GetComponent<SwordCollider> ();
		colliderDown = GameObject.Find ("cAttack Collider Down").GetComponent<SwordCollider> ();
		colliderLeft = GameObject.Find ("cAttack Collider Left").GetComponent<SwordCollider> ();
		colliderRight = GameObject.Find ("cAttack Collider Right").GetComponent<SwordCollider> ();

		colliderUp.DisableCollider();
		colliderDown.DisableCollider ();
		colliderRight.DisableCollider ();
		colliderLeft.DisableCollider ();
	}

	private void ActivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			//Debug.Log ("North");
			//Activate RightUp collider and animation
			colliderUp.EnableCollider();
			break;
		case EightDirections.NorthEast:
			//Debug.Log ("North East");
			//swordColliderUp.Enable ();
			break;
		case EightDirections.East:
			//Debug.Log ("East");
			colliderRight.EnableCollider ();
			break;
		case EightDirections.SouthEast:
			//Debug.Log ("South East");
			//swordColliderLeft.Enable ();
			break;
		case EightDirections.South:
			//Debug.Log ("South");
			colliderDown.EnableCollider ();
			break;
		case EightDirections.SouthWest:
			//Debug.Log ("South West");
			//swordCollider.Enable ();
			break;
		case EightDirections.West:
			//Debug.Log ("West");
			colliderLeft.EnableCollider ();
			break;
		case EightDirections.NorthWest:
			//Debug.Log ("North West");
			//swordColliderRight.Enable ();
			break;
		}
	}

	private void DeactivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			//Debug.Log ("North");
			//Activate RightUp collider and animation
			colliderUp.DisableCollider();
			break;
		case EightDirections.NorthEast:
			//Debug.Log ("North East");
			//swordColliderUp.Enable ();
			break;
		case EightDirections.East:
			//Debug.Log ("East");
			colliderRight.DisableCollider ();
			break;
		case EightDirections.SouthEast:
			//Debug.Log ("South East");
			//swordColliderLeft.Enable ();
			break;
		case EightDirections.South:
			//Debug.Log ("South");
			colliderDown.DisableCollider ();
			break;
		case EightDirections.SouthWest:
			//Debug.Log ("South West");
			//swordCollider.Enable ();
			break;
		case EightDirections.West:
			//Debug.Log ("West");
			colliderLeft.DisableCollider ();
			break;
		case EightDirections.NorthWest:
			//Debug.Log ("North West");
			//swordColliderRight.Enable ();
			break;
		}
	}

	//Create an attack vector from given components -> "attack vector" has a specific magnitude
	private Vector2 CreateAttackVector() { 
		float x = Input.GetAxis ("Horizontal");  
		float y = Input.GetAxis ("Vertical"); 
		Vector2 v = new Vector2 (x, y); 

		//If player moving joystick in any direction, get new atack direction vector. Else, use last direction player moved
		if ((x != 0f) || (y != 0f)) {  
			movementInfo.UpdateLastMove (v); //For animator to work correctly 
			v.Normalize ();
			v = new Vector2 (transform.position.x + v.x, transform.position.y + v.y); 
		} else {
			v = lastAttackDirection;
		}

		lastAttackDirection = v; 

		return v; 
	} 

	public void ChargeAttack(ref PlayerState playerState, Vector2 attackDirection) {
		switch (chargeAttackState) {
		case ChargeAttackState.Setup:
			timer = maxChargeTime;
			finalAttackForce = baseAttackForce; 
			chargeAttackState = ChargeAttackState.Charging;
			playerBody.velocity = Vector2.zero; 
			spriteRenderer.material.SetFloat ("_FlashAmount", 0.60f);

			//Activate corresponding collider
			playerFaceDirection = orientationSystem.GetDirection (CreateAttackVector ());
			break;

		case ChargeAttackState.Charging:
			timer -= Time.deltaTime;

			//Play Charging Animation

			//Make player flash while charging
			spriteRenderer.material.SetFloat("_FlashAmount", Mathf.PingPong(timer * 4.7f, 0.60f) );

			//If timer is up or player releases button, then perform attack
			if (timer <= 0f || (Input.GetButtonUp ("AttackPS4")) ) {
				timer = attackTime;
				spriteRenderer.material.SetFloat ("_FlashAmount", 0f);
				chargeAttackState = ChargeAttackState.Attacking;
				ActivateCorrespondingCollider (playerFaceDirection); 
	
				playerAttackInfo.UpdateAttackInfo (AttackID.ChargedAttack, finalAttackForce,
					movementInfo.GetLastMove ().normalized, damageAmount);
			}

			//If button is still being held down
			if (Input.GetButton ("AttackPS4")) {
				//Increase damage that will be dealt here
				finalAttackForce += (Time.deltaTime * chargeMultiplier);
				break;
			}

			//If interrupted (enemy hits player), reset stuff
			//If animation doesn't work, play it here
			break;

		case ChargeAttackState.Attacking:
			timer -= Time.deltaTime;

			//Play animation
			animator.Play("Charged Attack");

			//Activate colliders

			if (timer <= 0f) {
				//Deactivate collider 
				DeactivateCorrespondingCollider (playerFaceDirection);

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

	public void ResetState(ref PlayerState playerState) {
		playerState = PlayerState.Default;
		chargeAttackState = ChargeAttackState.Setup;
		timer = 0f;
		spriteRenderer.material.SetFloat ("_FlashAmount", 0f); 
		playerBody.velocity = Vector2.zero;
		finalAttackForce = baseAttackForce;

		//Deactivate collider 
		DeactivateCorrespondingCollider (playerFaceDirection);
	}

}
