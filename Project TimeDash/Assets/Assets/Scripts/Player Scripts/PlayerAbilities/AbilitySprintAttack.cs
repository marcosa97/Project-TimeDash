using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySprintAttack : MonoBehaviour {

	private enum AttackState {
		Setup,
		SprintAttacking,
		SprintAttackCooldown,
		Done
	}

	private AttackState attackState; //Make private when done debugging

	//Public settings
	public float sprintAttackTime;
	public float sprintAttackDistance;
	public float sprintAttackCooldownTime;
	public float baseAttackForce;

	//References and variables needed
	private float currentLerpTime;
	private Vector2 startPosition; //For sprint attack lerp
	private float attackTimer;
	private Rigidbody2D playerBody; //We need reference so we can move the body
	private OrientationSystem orientationSystem;
	private EightDirections playerFaceDirection;
	private Animator playerAnimator;
	private AbilityBasicMovement movementInfo;
	private AttackInfoContainer playerAttackInfo;


	//Colliders -> change these once I create new colliders for dashing
	private SwordCollider swordCollider;
	private SwordCollider swordColliderUp;
	private SwordCollider swordColliderLeft;
	private SwordCollider swordColliderRight;

	// Use this for initialization
	void Start () {
		//playerInfo = GetComponent<PlayerInfoContainer> (); 
		movementInfo = GetComponent<AbilityBasicMovement> ();
		playerAttackInfo = GetComponent<AttackInfoContainer> ();
		swordCollider = GameObject.Find ("Sword Collider Down").GetComponent<SwordCollider> (); //Down
		swordColliderUp = GameObject.Find ("Sword Collider Up").GetComponent<SwordCollider> ();
		swordColliderLeft = GameObject.Find ("Sword Collider Left").GetComponent<SwordCollider> ();
		swordColliderRight = GameObject.Find ("Sword Collider Right").GetComponent<SwordCollider> ();
		playerBody = GetComponent<Rigidbody2D> ();
		orientationSystem = GetComponent<OrientationSystem> ();
		playerAnimator = GetComponent<Animator> ();
	}

	//Activates the appropriate collider and animation ID/Integer
	private void ActivateCorrespondingCollider(EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			Debug.Log ("RIGHT UP");
			//Activate RightUp collider and animation
			swordColliderUp.EnableCollider();
			break;
		case EightDirections.NorthEast:
			Debug.Log ("UP RIGHT");
			//swordColliderUp.EnableCollider ();
			break;
		case EightDirections.East:
			Debug.Log ("UP LEFT");
			swordColliderRight.EnableCollider ();
			break;
		case EightDirections.SouthEast:
			Debug.Log ("LEFT UP");
			//swordColliderLeft.EnableCollider ();
			break;
		case EightDirections.South:
			Debug.Log ("LEFT DOWN");
			swordCollider.EnableCollider ();
			break;
		case EightDirections.SouthWest:
			Debug.Log ("DOWN LEFT");
			//swordCollider.EnableCollider ();
			break;
		case EightDirections.West:
			Debug.Log ("DOWN RIGHT");
			swordColliderLeft.EnableCollider ();
			break;
		case EightDirections.NorthWest:
			Debug.Log ("RIGHT DOWN");
			//swordColliderRight.EnableCollider ();
			break;
		}
	}

	private void DeactivateCorrespondingCollider (EightDirections dir) {
		switch (dir) {
		case EightDirections.North:
			Debug.Log ("RIGHT UP");
			//Activate RightUp collider and animation
			swordColliderUp.DisableCollider();
			break;
		case EightDirections.NorthEast:
			Debug.Log ("UP RIGHT");
			//swordColliderUp.EnableCollider ();
			break;
		case EightDirections.East:
			Debug.Log ("UP LEFT");
			swordColliderRight.DisableCollider ();
			break;
		case EightDirections.SouthEast:
			Debug.Log ("LEFT UP");
			//swordColliderLeft.EnableCollider ();
			break;
		case EightDirections.South:
			Debug.Log ("LEFT DOWN");
			swordCollider.DisableCollider ();
			break;
		case EightDirections.SouthWest:
			Debug.Log ("DOWN LEFT");
			//swordCollider.EnableCollider ();
			break;
		case EightDirections.West:
			Debug.Log ("DOWN RIGHT");
			swordColliderLeft.DisableCollider ();
			break;
		case EightDirections.NorthWest:
			Debug.Log ("RIGHT DOWN");
			//swordColliderRight.EnableCollider ();
			break;
		}
	}

	public void SprintAttack(ref PlayerState playerState, Vector3 attackDirection) {
		switch (attackState) {
		case AttackState.Setup:
			//modify sprint bool so that we can change the animation
			//playerSprinting = false;
			//playerSprAttacking = true;
			attackState = AttackState.SprintAttacking; 
			attackTimer = sprintAttackTime;
			startPosition = transform.position;
			currentLerpTime = 0f;
			Debug.Log ("SPRINT ATTACK!"); 

			playerFaceDirection = orientationSystem.DetermineDirectionFromVector (movementInfo.GetLastMove());
			ActivateCorrespondingCollider (playerFaceDirection );  
			playerAttackInfo.UpdateAttackInfo (AttackID.SprintAttack, baseAttackForce,
				movementInfo.GetLastMove().normalized);
			playerAnimator.Play ("Sprint Attack");
			break;

		case AttackState.SprintAttacking:
			//This state is when the player was sprinting and attacked
			attackTimer -= Time.deltaTime;

			//move towards attack direction
			playerBody.velocity = Vector2.zero; //Stop player from moving while attacking
			//transform.position = Vector3.MoveTowards (transform.position, attackDirection, sprintAttackDistance * Time.deltaTime);
			//float t = 

			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > sprintAttackTime)
				currentLerpTime = sprintAttackTime;

			float t = currentLerpTime / sprintAttackTime;
			t = Mathf.Sin (t * Mathf.PI * 0.5f);
			transform.position = Vector2.Lerp (startPosition, attackDirection, t);

			if (attackTimer <= 0f) {
				attackState = AttackState.SprintAttackCooldown; 

				//Disable colliders
				DeactivateCorrespondingCollider(playerFaceDirection);

				//Cooldown time for dash attack
				attackTimer = sprintAttackCooldownTime;
			}
			 
			break;

		case AttackState.SprintAttackCooldown:
			attackTimer -= Time.deltaTime; 

			if (attackTimer <= 0f) { 
				//Reset stuff
				attackTimer = 0f; 
				attackState = AttackState.Setup;
				playerState = PlayerState.Default; 
				//playerSprAttacking = false; 
			} 
			break;

		case AttackState.Done:
			attackState = AttackState.Setup;   
			playerState = PlayerState.Default; 
			break;
		}
	}
		
	public void ResetState(ref PlayerState playerState) {
		playerState = PlayerState.Default;
		attackState = AttackState.Setup;
		attackTimer = 0f;
		currentLerpTime = 0f;
		startPosition = Vector2.zero;
		playerBody.velocity = Vector2.zero;

		//Disable colliders
		DeactivateCorrespondingCollider(playerFaceDirection);
	}
}
