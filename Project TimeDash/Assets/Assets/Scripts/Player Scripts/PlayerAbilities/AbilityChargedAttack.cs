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
	public float baseAttackForce;
	public int chargeMultiplier;

	//References and variables needed
	private float timer;
	private float finalAttackForce;
	private AttackInfoContainer attackInfo;
	private ChargeAttackState chargeAttackState;
	private PlayerOrientation playerOrientation;
	private PlayerDirections playerDirection; //enum
	private AbilityBasicMovement movementInfo;
	private Rigidbody2D playerBody;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private OrientationSystem orientationSystem;
	private EightDirections playerFaceDirection;
	private Vector2 lastAttackDirection; 
	private bool playerChargeAttacking;

	private PlayerInfoContainer playerInfo;

	//Collider scripts
	private SwordCollider colliderUp;
	private SwordCollider colliderDown;
	private SwordCollider colliderLeft;
	private SwordCollider colliderRight;

	// Use this for initialization
	void Start () {
		attackInfo = new AttackInfoContainer (AttackID.ChargedAttack, baseAttackForce);
		playerInfo = GetComponent<PlayerInfoContainer> ();
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

		colliderUp.Disable ();
		colliderDown.Disable ();
		colliderLeft.Disable ();
		colliderRight.Disable ();
	}

	private void ActivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			//Debug.Log ("North");
			//Activate RightUp collider and animation
			colliderUp.Enable();
			break;
		case EightDirections.NorthEast:
			//Debug.Log ("North East");
			//swordColliderUp.Enable ();
			break;
		case EightDirections.East:
			//Debug.Log ("East");
			colliderRight.Enable ();
			break;
		case EightDirections.SouthEast:
			//Debug.Log ("South East");
			//swordColliderLeft.Enable ();
			break;
		case EightDirections.South:
			//Debug.Log ("South");
			colliderDown.Enable ();
			break;
		case EightDirections.SouthWest:
			//Debug.Log ("South West");
			//swordCollider.Enable ();
			break;
		case EightDirections.West:
			//Debug.Log ("West");
			colliderLeft.Enable ();
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
				playerInfo.UpdateAttackPerformed (attackInfo.GetAttackID(), finalAttackForce);
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
				colliderUp.AttackHasEnded ();
				colliderDown.AttackHasEnded ();
				colliderLeft.AttackHasEnded ();
				colliderRight.AttackHasEnded ();
				colliderUp.Disable ();
				colliderDown.Disable ();
				colliderLeft.Disable ();
				colliderRight.Disable ();

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
