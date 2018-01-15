using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbility : MonoBehaviour {
	public AttackState attackState;

	//Public settings (set in inspector)
	public float attackTime; //How long an attack should last
	public float attackDistance; //How far an attack will make a player move (% of the original walking distance)
	public float sprintAttackTime; //How long a sprint attack should last
	public float sprintAttackDistance; 
	public float sprintAttackCooldownTime;
	 
	//References we need
	private float currentLerpTime;
	private Vector2 startPosition; //For sprint attack
	private float attackTimer;
	private Rigidbody2D playerBody; //We need reference so we can move the body
	private PlayerController playerController;
	private AbilitiesHandler abilitiesHandler;
	private PlayerOrientation playerOrientation;
	private Animator playerAnimator;

	private SwordCollider swordCollider;
	private SwordCollider swordColliderUp;
	private SwordCollider swordColliderLeft;
	private SwordCollider swordColliderRight;
	//NOTE: Will need to add additional collider references for sprint attacks
	// and for the chain attacks if they end up being different

	void Start() {
		swordCollider = GameObject.Find ("Sword Collider Down").GetComponent<SwordCollider> (); //Down
		swordColliderUp = GameObject.Find ("Sword Collider Up").GetComponent<SwordCollider> ();
		swordColliderLeft = GameObject.Find ("Sword Collider Left").GetComponent<SwordCollider> ();
		swordColliderRight = GameObject.Find ("Sword Collider Right").GetComponent<SwordCollider> ();
		playerBody = GetComponent<Rigidbody2D> ();
		playerController = GetComponent<PlayerController> ();
		playerOrientation = GetComponent<PlayerOrientation> ();
		playerAnimator = GetComponent<Animator> ();
		abilitiesHandler = GetComponent<AbilitiesHandler> ();
	}

	//Performs the attack ability
	//  @playerState: the state of the player is needed so we can change it to Attacking state
	//  @playerAttacking: the bool is needed so that we can let the animator know when an attack is happening
	//  @attackDirection: the vector that contains which direction the attack is happening at so we can set the 
	//                    appropriate animation and collider
	public void Attack(ref PlayerState playerState, ref bool playerAttacking, ref bool playerSprAttacking, 
		                                            ref bool playerSprinting, Vector3 attackDirection) { 
		switch (attackState) {
		case AttackState.Ready:
			//Determine if it's a regular attack or sprint attack
			if (playerSprinting) {
				//modify sprint bool so that we can change the animation
				playerSprinting = false;
				playerSprAttacking = true;
				attackState = AttackState.SprintAttacking;
				attackTimer = sprintAttackTime;
				startPosition = transform.position;
				currentLerpTime = 0f;
				Debug.Log ("SPRINT ATTACK!");
			} else {
				attackState = AttackState.Attacking;
				attackTimer = attackTime;
			}

			//NOTE: will have to make a different function for dash attack colliders
			//Determines which animation to play and which collider to enable
			DetermineAttackDirection (playerOrientation.GetDirection (attackDirection));
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


		case AttackState.Attacking:
			//This state is when the sword is swinging in the animation
			attackTimer -= Time.deltaTime;

			//move towards click position
			playerBody.velocity = Vector2.zero; //Stop player from moving while attacking
			transform.position = Vector3.MoveTowards (transform.position, attackDirection, attackDistance * Time.deltaTime);

			//Set attacking animation
			playerAttacking = true;

			if (attackTimer <= 0f) {
				attackState = AttackState.Done;

				//Deactivate sword collider
				//NEED UPDATE: Find a way to only make one call and disable the activated collider
				//Let collider know that attack has ended so it can clear its list of attacked enemies
				swordCollider.AttackHasEnded ();
				swordColliderUp.AttackHasEnded ();
				swordColliderLeft.AttackHasEnded ();
				swordColliderRight.AttackHasEnded ();

				//Disable colliders
				swordCollider.Disable();
				swordColliderUp.Disable ();
				swordColliderLeft.Disable ();
				swordColliderRight.Disable ();

				//Window of time that player can chain another attack for a combo
				attackTimer = 0f; //Reset
				abilitiesHandler.activateComboWindowTimer();
			}
			break;

		case AttackState.SprintAttackCooldown:
			attackTimer -= Time.deltaTime;

			if (attackTimer <= 0f) {
				//Reset stuff
				attackTimer = 0f;
				attackState = AttackState.Ready;
				playerState = PlayerState.Default;
				playerSprAttacking = false;
			}

			break;


		case AttackState.Done:
			//NOTE: Will need to have the animation continue to play during this state. During this state, the sword is 
			//      no longer swinging, but this state can be interrupted by another attack -> combo

			if (abilitiesHandler.comboMaxReached () && !abilitiesHandler.isCooldownTimerUp ()) {
				//stay in this state, let it cool down
			} else {
				//Reset States
				attackState = AttackState.Ready; 
				playerState = PlayerState.Default;
			}

			//need some sort of bool/message that will let the animator know to keep playing cooldown animation/sprite

			break;
		}//switch
	}
		
	//Determines what direction to face when attacking and enables the corresponding collider
	void DetermineAttackDirection(PlayerDirections playerDirections) {
		switch (playerDirections) {
		case PlayerDirections.RightUp:
			Debug.Log ("RIGHT UP");
			//Activate RightUp collider and animation
			swordColliderRight.Enable();
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
		playerAnimator.SetInteger ("PlayerAttackDirection", (int)playerDirections);
	}

	Vector2 CreateAttackVector(Vector2 attackDirection) {
		
		return attackDirection;
	}

}

public enum AttackState {
	Ready,
	SprintAttacking,
	Attacking,
	SprintAttackCooldown,
	Done
}
