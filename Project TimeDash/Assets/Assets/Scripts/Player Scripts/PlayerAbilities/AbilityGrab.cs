using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the grabbing state
public class AbilityGrab : MonoBehaviour {
	//enums
	private enum GrabState {
		Setup,
		Grab,
		HoldingEnemy,
		ThrowingEnemy,
		Done
		//HoldingItem
	}

	//Public Settings
	public float grabTime;
	public float throwTime;

	//References and variables needed
	private float timer; //how long a grab animation takes
	private bool enemyGrabbed; 
	private Rigidbody2D playerBody; //To stop player movement
	private GrabState grabState;
	private Animator playerAnimator;
	private CircleCollider2D grabCollider;
	private Collider2D enemyCollider; //To ignore collisions during grab
	private Rigidbody2D enemyRigidBody;
	private OrientationSystem orientationSystem;
	private AbilityBasicMovement moveInfo; 

	private AttackInfoContainer playerAttackInfo;

	// Use this for initialization
	void Start () {
		playerBody = GetComponent<Rigidbody2D> (); 
		playerAnimator = GetComponent<Animator> ();
		grabCollider = GameObject.Find ("Grab Collider Up").GetComponent<CircleCollider2D> ();
		grabCollider.enabled = false;
		grabState = GrabState.Setup;
		enemyGrabbed = false;
		playerAttackInfo = GetComponent<AttackInfoContainer> ();
		orientationSystem = GetComponent<OrientationSystem> ();
		moveInfo = GetComponent<AbilityBasicMovement> (); 
	}

	public void Grab(ref PlayerState playerState) {
		switch (grabState) {
		case GrabState.Setup:
			timer = grabTime;
			grabState = GrabState.Grab;
			playerBody.velocity = Vector2.zero;


			//Activate collider
			grabCollider.enabled = true;
			playerAnimator.Play("Grab State");

			//Move Hold point to the direction the player is facing

			break;


		case GrabState.Grab:
			timer -= Time.deltaTime;
			//Play grab animation

			//if hitbox grabs something
			if (enemyGrabbed) {
				//transition to grabbing state
				grabState = GrabState.HoldingEnemy;

				//Reset stuff
				grabCollider.enabled = false;
				enemyGrabbed = false;

				//Play Holding animation
				playerAnimator.Play("Idle Direction");

				//Update Player's Attack Info Container 
				Vector2 dir = orientationSystem.GetOrientationVector( moveInfo.GetFaceDirection() );
				playerAttackInfo.UpdateAttackInfo (AttackID.Grab, 10, dir);

				//Ignore collisions between player body and enemy body
				Physics2D.IgnoreCollision(enemyCollider, GetComponent<BoxCollider2D>(), true );

				break;
			}

			//Grab move is done, and something was not grabbed
			if (timer <= 0f) {
				timer = 0f;
				grabState = GrabState.Done;
				grabCollider.enabled = false;

				//Play "let go" animation
			}

			break;


		case GrabState.HoldingEnemy:
			timer -= Time.deltaTime;

			//If hit, play hit animation and do damage

			//If throw button is pressed, throw enemy
			if (Input.GetButtonDown ("GrabPS4")) {
				//Play throw animation

				//Switch to throw state
				grabState = GrabState.ThrowingEnemy;

				enemyRigidBody.AddRelativeForce (playerAttackInfo.direction * 10, ForceMode2D.Impulse);
				timer = throwTime;
				grabCollider.enabled = false;
				Physics2D.IgnoreCollision (enemyCollider, GetComponent<BoxCollider2D> (), false);
				break;
			}

			if (timer <= 0f) {
				timer = 0f;
				grabState = GrabState.Done;
				grabCollider.enabled = false;

				//Undo ignored collisions
				Physics2D.IgnoreCollision (enemyCollider, GetComponent<BoxCollider2D> (), false);
			}

			break;

		
		case GrabState.ThrowingEnemy:
			timer -= Time.deltaTime;

			if (timer <= 0f) {
				timer = 0f;
				grabState = GrabState.Done;
			}

			break;
		
		case GrabState.Done:
			//Reset
			grabState = GrabState.Setup;
			playerState = PlayerState.Default;

			break;
		}
	}

	/// <summary>
	///   To be called by the grab collider's script.
	///   This function makes the bool "enemyGrabbed" true
	///   to let the state machine know that a grabbable enemy
	///   has been detected by the grab collider.
	///   To Implement: A float will also be passed in,
	///   which represents the time for how long the enemy
	///   can be held for before they break free
	/// </summary>
	public void EnemyDetected(float holdTime) {
		Debug.Log ("Grabbed Enemy");
		enemyGrabbed = true;
		timer = holdTime;
	}

	/// <summary>
	///    Retreives the enemy body's 2D collider and stores it
	///    in the class' variable "enemyCollider." This function
	///    is to be called by the grab collider script, which has
	///    access to the enemy's collider
	/// </summary>
	/// <param name="enemyBody">Enemy body.</param>
	public void RetreiveEnemyBodyCollider(Collider2D enemyBody) {
		enemyCollider = enemyBody;
		enemyRigidBody = enemyBody.gameObject.GetComponent<Rigidbody2D> ();
	}

	//FOR NOW UNUSED
	/// <summary>
	///    Makes it so that the physics system ignores collisions 
	///    between player body and enemy body. To be called by the
	///    grab collider script (collisions must be set to be ignored
	///    before we can move the enemy's body to the hold position)
	/// </summary>
	public void IgnoreBodyCollisions() {
		Physics2D.IgnoreCollision(enemyCollider, GetComponent<BoxCollider2D>(), true );
	}

	public void ResetState(ref PlayerState playerState) {
		timer = 0f;
		grabState = GrabState.Setup;
		enemyGrabbed = false;
		grabCollider.enabled = false;
		if (enemyCollider != null) {
			Physics2D.IgnoreCollision (enemyCollider, GetComponent<BoxCollider2D> (), false);
		}
		playerState = PlayerState.Default;
	}
}
