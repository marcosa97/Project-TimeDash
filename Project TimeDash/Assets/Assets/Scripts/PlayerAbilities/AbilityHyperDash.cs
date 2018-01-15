using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHyperDash : MonoBehaviour {
	HyperDashState hyperDashState;

	//Public settings (set in inspector)
	public float hyperDashTime; //How long a hyper dash should last
	public float coolDownTime; //How long the cool down time for an attack should last
    public float dashDistance; //How far an attack will make a player move (% of the original walking distance)
	public float comboWindowTime; //How long the player has between attacks to chain together a combo
	public int maxDashesAllowed;
	public Queue<Vector3> positionsToDashTo;
	private bool donePickingPositions;

	//References needed
	public TimeManager timeManager; //For slow motion
	public Rigidbody2D playerBody; //To move the player
	public PlayerController playerController; 

	Vector3 hyperDashDirection;

	private float timer;

	// Use this for initialization
	void Start () {
		positionsToDashTo = new Queue<Vector3> ();
		playerBody = GetComponent<Rigidbody2D> ();
		playerController = GetComponent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Right mouse click
		if (Input.GetMouseButtonDown(1)) {
			//timeManager.DoSlowMotion ();
		}
	}

	void PickHyperDashPositions(Vector3 attackDirection, ref PlayerState playerState) {
		//If player adds a new position to Hyper Dash to (Right click)
		if (Input.GetMouseButtonDown (1)) {
			if (!positionsToDashTo.Contains (attackDirection)) {
				Debug.Log ("ENQUEUED!");
				positionsToDashTo.Enqueue (attackDirection);
			}
		}

		//If player has picked all his dashes (MaxDashes reached or Left click)
		if ((positionsToDashTo.Count == maxDashesAllowed) || Input.GetMouseButtonDown(0)) {
			//Debug.Log ("ATTACK READY");
			donePickingPositions = true;
			return;
		}

		//If player decides to cancel Hyper Dash
		if (Input.GetKeyDown (KeyCode.Q)) {
			positionsToDashTo.Clear ();
			donePickingPositions = false; //Reset
			hyperDashState = HyperDashState.Ready; //Reset
			playerState = PlayerState.Default;
		}
	}

	public void HyperDash(ref PlayerState playerState, ref float coolDownTimer, ref bool playerHyperDashing, Vector3 attackDirection,
		                  float moveHorizontal, float moveVertical ) {

		switch (hyperDashState) {
		case HyperDashState.Ready:
			//Activate slow motion if we initiated Hyper Dash
			if ( !(timeManager.SlowMotionActive ()) ) {
				timeManager.ActivateSlowMotion ();
			}

			//If player has picked all positions to Hyper Dash to
			if (donePickingPositions) {
				//Set up hyper dash information
				hyperDashState = HyperDashState.HyperDashing;
				timer = hyperDashTime;
				//DetermineAttackDirection() -> may need a modified version of this function
				//hyperDashDirection = playerController.GetMousePositionVector ();

				//timeManager.DoSlowMotion ();
				donePickingPositions = false; //reset
				//timeManager.DoSlowMotion();
			} else {
				//Let player move while in slow motion
				playerController.MovePlayer ();
				//Let player pick attack positions
				PickHyperDashPositions (attackDirection, ref playerState);
			}
			break;

		case HyperDashState.HyperDashing:
			timer -= Time.unscaledDeltaTime;

			//This bool may not be needed 
			playerHyperDashing = true;

			//move towards click position
			playerBody.velocity = Vector2.zero;

			if (positionsToDashTo.Count > 0) {
				transform.position = Vector3.MoveTowards (transform.position, positionsToDashTo.Peek (), 
					                                      dashDistance * Time.unscaledDeltaTime);
			}


			if (timer <= 0f) {
				if (positionsToDashTo.Count > 0) {
					positionsToDashTo.Dequeue ();
					timer = hyperDashTime;
					break;
				}
				hyperDashState = HyperDashState.Cooldown;

				//Window of time that player can chain another dash
				timer = comboWindowTime;
			}
			break;

		case HyperDashState.Cooldown:
			timer -= Time.unscaledDeltaTime;

			timeManager.DeactivateSlowMotion ();
			playerController.MovePlayer ();

			//If Hyper Dashes aren't done, move on to next attack
			if (positionsToDashTo.Count != 0) {
				timer = hyperDashTime;
				hyperDashState = HyperDashState.Ready;
				//playerHyperDashing = false;
				break;
			}

			if (timer <= 0f) {
				timer = 0;
				hyperDashState = HyperDashState.Ready;
				playerState = PlayerState.Default;
			}

			break;

		}//switch

	}//HyperDash()

	//May have to use Raycasting to deal damage for this move
	void OnCollisionEnter2D(Collision2D other) {
		//If player hyper dashes through an enemy
		if ((hyperDashState == HyperDashState.HyperDashing) && (other.gameObject.tag == "Enemy")) {
			other.gameObject.GetComponent<HealthManager> ().ReceiveDamage (10);
		}
	}
}

public enum HyperDashState {
	Ready,
	HyperDashing,
	Cooldown
}
