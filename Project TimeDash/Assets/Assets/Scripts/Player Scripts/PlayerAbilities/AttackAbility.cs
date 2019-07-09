
using UnityEngine;

public class AttackAbility : MonoBehaviour {
    [SerializeField]
    public AttackState attackState;

    //Public settings (set in inspector)
    [Header("Normal Attack Settings")]
    public float inputBufferTime;
    public float attackDistance; //How far an attack will make a player move (% of the original walking distance)
    public float regularCooldownTime;
    public float comboCooldownTime;
    public int comboMax;
    public int baseAttackForce; //knockback
    public int damageAmount;

    //References and variables needed
    private int comboCount;
    private Vector2 lastAttackDirection;
    private float timer;
    private Rigidbody2D playerBody; //We need reference so we can move the body
    private Animator playerAnimator;
    private FourDirections playerFaceDirection;
    private FourDirectionSystem DirectionHandler;
    private AbilityBasicMovement movementInfo;
    private bool playerAttacking;
    private AttackInfoContainer playerAttackInfo;
    private bool bufferInput;
    private bool chargeAttack;
    private Vector2 targetPosition;

    private SwordCollider swordCollider;
    private SwordCollider swordColliderUp;
    private SwordCollider swordColliderLeft;
    private SwordCollider swordColliderRight;
    //NOTE: Will need to add additional collider references for sprint attacks
    // and for the chain attacks if they end up being different

    void Start() {
        playerAttackInfo = GetComponent<AttackInfoContainer>();
        swordCollider = GameObject.Find("Sword Collider Down").GetComponent<SwordCollider>(); //Down
        swordColliderUp = GameObject.Find("Sword Collider Up").GetComponent<SwordCollider>();
        if (swordColliderUp == null)
            Debug.Log("NULL UP ");
        swordColliderLeft = GameObject.Find("Sword Collider Left").GetComponent<SwordCollider>();
        swordColliderRight = GameObject.Find("Sword Collider Right").GetComponent<SwordCollider>();

        playerBody = GetComponent<Rigidbody2D>();
        //orientationSystem = GetComponent<OrientationSystem>();
        DirectionHandler = new FourDirectionSystem();
        playerAnimator = GetComponent<Animator>();
        movementInfo = GetComponent<AbilityBasicMovement>();
        comboCount = 0;
        bufferInput = false;
        chargeAttack = false;
    }

    //Performs the attack ability
    //  @playerState: the state of the player is needed so we can change it to Attacking state
    //  @playerAttacking: the bool is needed so that we can let the animator know when an attack is happening
    //  @attackDirection: the vector that contains which direction the attack is happening at so we can set the 
    //                    appropriate animation and collider
    public void Attack(ref PlayerState playerState, Vector3 attackDirection) {
        switch (attackState) {
            case AttackState.Ready:
                //Setup attack
                attackState = AttackState.Attacking;
                timer = inputBufferTime;
                //Increase combo counter
                comboCount++; //Use combo count to let animator know which attack is being performed
                playerAnimator.SetInteger("ComboCount", comboCount);

                if (comboCount == 1) {
                    lastAttackDirection = attackDirection;
                }

                //Decide which animation to play
                switch (comboCount) {
                    case 1:
                        playerAnimator.Play("Attack 1");
                        break;
                    case 2:
                        playerAnimator.Play("Attack 2");
                        break;
                    case 3:
                        playerAnimator.Play("Attack 1");
                        break;
                    case 4:
                        playerAnimator.Play("Attack 2");
                        break;
                }

                //Determine which which collider to enable
                //playerFaceDirection = orientationSystem.GetDirection(CreateAttackVector());
                Debug.Log(CreateAttackVector());
                playerFaceDirection = DirectionHandler.GetDirectionFromVector(CreateAttackVector());
                ActivateCorrespondingCollider(playerFaceDirection);
                playerAttacking = true;

                //Set up to check if charged attack will be performed
                //Charge attack happens if attack button is held down while performing attack
                if (Input.GetButtonUp("AttackPS4")) {
                    this.chargeAttack = false;
                } else {
                    this.chargeAttack = true;
                }

                //Let the attack info container know what just happened
                playerAttackInfo.UpdateAttackInfo(AttackID.NormalAttack,
                    baseAttackForce, movementInfo.GetLastMove().normalized, damageAmount);

                //Calculate where the attack will move the player
                targetPosition = new Vector3(transform.position.x + lastAttackDirection.x * 10f,
					transform.position.y + lastAttackDirection.y * 10f);

                break;


            case AttackState.Attacking:
                //This state is when the sword is swinging in the animation
                timer -= Time.deltaTime;

                if ( Input.GetButtonUp("AttackPS4") ) {
                    this.chargeAttack = false;
                }

                //Check if user inputted attack while an attack is still happening (in buffer time window)
                if ( !(timer <= 0f) && Input.GetButtonDown("AttackPS4") && (comboCount != comboMax)) {
                    bufferInput = true;
                }

                //move towards click position
                playerBody.velocity = Vector2.zero; //Stop player from moving while attacking
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, attackDistance * Time.deltaTime);

                //Animation Event will trigger exit out of this state
                break;
         

            case AttackState.Done:
                //NOTE: Will need to have the animation continue to play during this state. During this state, the sword is 
                //      no longer swinging, but this state can be interrupted by another attack -> combo
                timer -= Time.deltaTime;

                //Check for charged attack
                if ( Input.GetButtonUp("AttackPS4") || comboCount == comboMax) {
                    this.chargeAttack = false;
                }

                //Inputs that can interrupt attack:
                //  Moving
                //  Attack -> Combo
                //If input is received within cooldown period (except when combo max reached), change states. 
                if ( (Input.GetButtonDown("AttackPS4") && (comboCount != comboMax)) || bufferInput) {
                    //Go to next attack -> Maybe create switch case here to assign different animations or attack times
                    Debug.Log("Buffered Input");
                    attackState = AttackState.Ready;
                    bufferInput = false;
                }

                //If input not received, finish playing the cooldown animation and then transition into default state
                if (timer <= 0f) {
                    //Reset stuff
                    comboCount = 0;
                    timer = 0f;
                    playerAttacking = false;
                    attackState = AttackState.Ready;

                    if (this.chargeAttack) {
                        playerState = PlayerState.ChargedAttacking;
                    } else {
                        playerState = PlayerState.Default;
                    }

                    this.bufferInput = false;
                    this.chargeAttack = true;
                }

                //need some sort of bool/message that will let the animator know to keep playing cooldown animation/sprite

                break;
        }//switch
    }

    public void StopAttack() {
        this.attackState = AttackState.Done;

        //Deactivate sword collider
        DeactivateCorrespondingCollider(playerFaceDirection);

        //Window of time that player can chain another attack for a combo
        if (this.comboCount != comboMax)
        {
            this.timer = regularCooldownTime;
        }
        else
        {
            this.timer = comboCooldownTime;
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
			//v = new Vector2 (transform.position.x + v.x, transform.position.y + v.y);
		} else {
			v = lastAttackDirection;
		}

		lastAttackDirection = v;
			
		return v;
	}

	//Determines what direction to face when attacking and enables the corresponding collider
	private void ActivateCorrespondingCollider(FourDirections direction) {
		switch (direction) {
		case FourDirections.North:
			Debug.Log ("North");
			swordColliderUp.EnableCollider ();
			break;
		case FourDirections.East:
			Debug.Log ("East");
			swordColliderRight.EnableCollider();
			break;
		case FourDirections.South:
			Debug.Log ("South");
			swordCollider.EnableCollider();
			break;
		case FourDirections.West:
			Debug.Log ("West");
			swordColliderLeft.EnableCollider();
			break;
		}
	}

	private void DeactivateCorrespondingCollider(FourDirections direction) {
		switch (direction) {
		case FourDirections.North:
			//Debug.Log ("North");
			swordColliderUp.DisableCollider ();
			break;
		case FourDirections.East:
			//Debug.Log ("East");
			swordColliderRight.DisableCollider();
			break;
		case FourDirections.South:
			//Debug.Log ("South");
			swordCollider.DisableCollider();
			break;
		case FourDirections.West:
			//Debug.Log ("West");
			swordColliderLeft.DisableCollider();
			break;
		}
	}

	//=================GETTER FUNCTIONS===================
	public bool GetPlayerAttacking() {
		return playerAttacking;
	}

	public int GetAttackDirection() {
		return (int) playerFaceDirection;
	}

	public Vector2 GetAttackVector() {
		Vector2 v = lastAttackDirection;
		lastAttackDirection.x -= transform.position.x;
		lastAttackDirection.y -= transform.position.y;
		return v;
	}

	//Cancels the state by stopping the attack and returning to default state
	public void ResetState(ref PlayerState playerState) {
		playerState = PlayerState.Default;
		attackState = AttackState.Ready;
		timer = 0f;
		comboCount = 0;
		playerAttacking = false;
		playerBody.velocity = Vector2.zero;
        bufferInput = false;
        this.chargeAttack = true;

		//Deactivate sword collider
		DeactivateCorrespondingCollider (playerFaceDirection);
	}

}

public enum AttackState {
	Ready,
	Attacking,
	Done
}
