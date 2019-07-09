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
	[Header ("Sprint Attack Settings")]
	public float sprintAttackTime;
	public float sprintAttackDistance;
	public float sprintAttackCooldownTime;
	public float baseAttackForce;
	public int damageAmount;

	//References and variables needed
	private float currentLerpTime;
	private Vector2 startPosition; //For sprint attack lerp
	private float attackTimer;
	private Rigidbody2D playerBody; //We need reference so we can move the body
    private FourDirectionSystem DirectionHandler;
    private FourDirections playerFaceDirection;
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
        DirectionHandler = new FourDirectionSystem();
		playerAnimator = GetComponent<Animator> ();
	}

	//Activates the appropriate collider and animation ID/Integer
	private void ActivateCorrespondingCollider(FourDirections dir) {
		switch (dir) {
		case FourDirections.North:
			Debug.Log ("RIGHT UP");
			swordColliderUp.EnableCollider();
			break;
		case FourDirections.East:
			Debug.Log ("UP LEFT");
			swordColliderRight.EnableCollider ();
			break;
		case FourDirections.South:
			Debug.Log ("LEFT DOWN");
			swordCollider.EnableCollider ();
			break;
		case FourDirections.West:
			Debug.Log ("DOWN RIGHT");
			swordColliderLeft.EnableCollider ();
			break;
		}
	}

	private void DeactivateCorrespondingCollider (FourDirections dir) {
		switch (dir) {
		case FourDirections.North:
			Debug.Log ("RIGHT UP");
			swordColliderUp.DisableCollider();
			break;
		case FourDirections.East:
			Debug.Log ("UP LEFT");
			swordColliderRight.DisableCollider ();
			break;
		case FourDirections.South:
			Debug.Log ("LEFT DOWN");
			swordCollider.DisableCollider ();
			break;
		case FourDirections.West:
			Debug.Log ("DOWN RIGHT");
			swordColliderLeft.DisableCollider ();
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

            playerFaceDirection = DirectionHandler.GetDirectionFromVector(movementInfo.GetLastMove());
			ActivateCorrespondingCollider (playerFaceDirection );  
			playerAttackInfo.UpdateAttackInfo (AttackID.SprintAttack, baseAttackForce,
				movementInfo.GetLastMove().normalized, damageAmount);
			playerAnimator.Play ("Sprint Attack");
			break;

		case AttackState.SprintAttacking:
			//This state is when the player was sprinting and attacked
			attackTimer -= Time.deltaTime;

			//move towards attack direction
			playerBody.velocity = Vector2.zero; //Stop player from moving while attacking

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
