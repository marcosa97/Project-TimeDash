using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the flinch (hurt) state for the player
public class PlayerStateFlinch : MonoBehaviour {

	private enum FlinchState {
		Setup,
		Flinching,
		Done
	}

	//Public Settings
	public float flinchTime;

	//References and variables needed
	private float timer;
	//private Vector2 knockBackDirection;
	private FlinchState flinchState;
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private Rigidbody2D playerBody;

	// Use this for initialization
	void Start () {
		timer = 0f;
		//knockBackDirection = Vector2.zero;
		flinchState = FlinchState.Setup;
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		playerBody = GetComponent<Rigidbody2D> ();
	}

    //Used by the player controller to set the direction
	// in which the player will be knocked back. 
	// Note: This function has to be called before entering
	//       this state if there will be knockback
	//public void SetKnockbackDirection(Vector2 dir) {

	//}

	//Makes the player flinch (when player is hurt)
	public void Flinch(ref PlayerState playerState) {
		switch (flinchState) {
		case FlinchState.Setup:
			timer = flinchTime;
			playerBody.velocity = Vector2.zero;
			//Play Flinch Animation according to face direction

			//Set sprite color to red to indicate hurt
			spriteRenderer.material.SetFloat ("_FlashAmount", 0.60f);

			//Add force to player in a direction

			flinchState = FlinchState.Flinching;

			break;

		case FlinchState.Flinching:
			timer -= Time.deltaTime;

			//Gradually change red color to normal

			if (timer <= 0f) {
				timer = 0f;
				//Set sprite color to normal
				spriteRenderer.material.SetFloat("_FlashAmount", 0f);

				flinchState = FlinchState.Done;

			}
			break;

		case FlinchState.Done:
			flinchState = FlinchState.Setup;
			playerState = PlayerState.Default;
			//knockBackDirection = Vector2.zero;
			break;
		}
	}

	public void ResetState(ref PlayerState playerState) {
		playerState = PlayerState.Default;
		flinchState = FlinchState.Setup;
		timer = 0f;
		spriteRenderer.material.SetFloat ("_FlashAmount", 0f);
	}
}
