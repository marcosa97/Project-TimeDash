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
	public float attackForce;

	//References and variables needed
	private float currentLerpTime;
	private Vector2 startPosition; //For sprint attack lerp
	private float attackTimer;
	private Rigidbody2D playerBody; //We need reference so we can move the body
	private PlayerOrientation playerOrientation;
	private Animator playerAnimator;
	private AttackInfoContainer attackInfo;
	private PlayerInfoContainer playerInfo;

	//Colliders -> change these once I create new colliders for dashing
	private SwordCollider swordCollider;
	private SwordCollider swordColliderUp;
	private SwordCollider swordColliderLeft;
	private SwordCollider swordColliderRight;

	// Use this for initialization
	void Start () {
		attackInfo = new AttackInfoContainer (AttackID.SprintAttack, attackForce);
		playerInfo = GetComponent<PlayerInfoContainer> (); 
		swordCollider = GameObject.Find ("Sword Collider Down").GetComponent<SwordCollider> (); //Down
		swordColliderUp = GameObject.Find ("Sword Collider Up").GetComponent<SwordCollider> ();
		swordColliderLeft = GameObject.Find ("Sword Collider Left").GetComponent<SwordCollider> ();
		swordColliderRight = GameObject.Find ("Sword Collider Right").GetComponent<SwordCollider> ();
		playerBody = GetComponent<Rigidbody2D> ();
		playerOrientation = GetComponent<PlayerOrientation> ();
		playerAnimator = GetComponent<Animator> ();
	}

	//Activates the appropriate collider and animation ID/Integer
	private void SetColliderAndAnimation(PlayerDirections playerDirection) {
		switch (playerDirection) {
		case PlayerDirections.RightUp:
			Debug.Log ("RIGHT UP");
			//Activate RightUp collider and animation
			swordColliderRight.Enable ();
			break;
		case PlayerDirections.UpRight:
			Debug.Log ("UP RIGHT");
			swordColliderUp.Enable ();
			break;
		case PlayerDirections.UpLeft:
			Debug.Log ("UP LEFT");
			swordColliderUp.Enable ();
			break;
		case PlayerDirections.LeftUp:
			Debug.Log ("LEFT UP");
			swordColliderLeft.Enable ();
			break;
		case PlayerDirections.LeftDown:
			Debug.Log ("LEFT DOWN");
			swordColliderLeft.Enable ();
			break;
		case PlayerDirections.DownLeft:
			Debug.Log ("DOWN LEFT");
			swordCollider.Enable ();
			break;
		case PlayerDirections.DownRight:
			Debug.Log ("DOWN RIGHT");
			swordCollider.Enable ();
			break;
		case PlayerDirections.RightDown:
			Debug.Log ("RIGHT DOWN");
			swordColliderRight.Enable ();
			break;
		}

		//Let animator which direction to face
		playerAnimator.SetInteger ("PlayerAttackDirection", (int)playerDirection );
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

			SetColliderAndAnimation (playerOrientation.GetDirection (attackDirection)); 
			playerInfo.UpdateAttackPerformed (attackInfo.GetAttackID(), attackInfo.GetForce() );
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

				//Clear list of targets that were hit
				swordCollider.AttackHasEnded (); 
				swordColliderUp.AttackHasEnded (); 
				swordColliderLeft.AttackHasEnded (); 
				swordColliderRight.AttackHasEnded (); 

				//Disable colliders
				swordCollider.Disable();
				swordColliderUp.Disable ();
				swordColliderLeft.Disable ();
				swordColliderRight.Disable ();

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

		//Clear list of targets that were hit
		swordCollider.AttackHasEnded (); 
		swordColliderUp.AttackHasEnded (); 
		swordColliderLeft.AttackHasEnded (); 
		swordColliderRight.AttackHasEnded (); 

		//Disable colliders
		swordCollider.Disable();
		swordColliderUp.Disable ();
		swordColliderLeft.Disable ();
		swordColliderRight.Disable (); 
	}
}
