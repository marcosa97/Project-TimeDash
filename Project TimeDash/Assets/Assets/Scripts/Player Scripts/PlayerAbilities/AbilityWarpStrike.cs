
using UnityEngine;

public class AbilityWarpStrike : MonoBehaviour {
    private enum WarpState {
        Setup,
        InitialStrike,
        Warping,
        EndStrike,
        Cooldown,
        Done
    }

    [SerializeField]
    private WarpState warpState;

    [Header("Warp Strike Settings")]
    public float warpDuration;
    public float warpSpeed;
    public float cooldownDuration;
    public int baseAttackForce;
    public int damageAmount;

    private Rigidbody2D playerBody; //We need reference so we can move the body
    private BoxCollider2D playerCollider;
    private AbilityChargedAttack chargedAttack; // For colliders
    private FourDirectionSystem DirectionHandler;
    private AttackInfoContainer playerAttackInfo;
    private SpriteRenderer spriteRenderer;
    private Animator playerAnim;

    private float timer;
    private FourDirections warpDirection;
    private Vector2 warpDirVector;
    private const int EnemiesLayer = 10;
    private const int PlayerLayer = 8;

    // Use this for initialization
    void Start () {
        this.timer = 0f;
        this.playerBody = GetComponent<Rigidbody2D>();
        this.playerCollider = GetComponent<BoxCollider2D>();
        this.chargedAttack = GetComponent<AbilityChargedAttack>();
        this.DirectionHandler = new FourDirectionSystem();
        this.playerAttackInfo = GetComponent<AttackInfoContainer>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.playerAnim = GetComponent<Animator>();
        this.warpState = WarpState.Setup;
	}
	
    //@dir: normalized vector in direction of attack
    public void WarpStrike(ref PlayerState playerState, Vector2 dir) {
        switch(this.warpState) {
            case WarpState.Setup:
                this.warpState = WarpState.InitialStrike;
                this.warpDirVector = dir;
                this.warpDirection = this.DirectionHandler.GetDirectionFromVector(dir);
                this.chargedAttack.ActivateCorrespondingCollider(warpDirection);

                //Let the attack info container know what just happened
                this.playerAttackInfo.UpdateAttackInfo(AttackID.WarpStrike,
                    baseAttackForce, dir, damageAmount);

                this.timer = this.warpDuration;

                //Play Animation
                this.playerAnim.Play("Initial Warp Strike");

                break;

            case WarpState.InitialStrike:

                //Animation Event transitions out of this state
                break;

            case WarpState.Warping:
                this.timer -= Time.deltaTime;

                if (this.timer <= 0f) {
                    EndWarp();
                }
                break;

            case WarpState.EndStrike:

                //Animation Event transitions out of this state
                break;

            case WarpState.Cooldown:
                this.timer -= Time.deltaTime;

                if (this.timer <= 0f) {
                    this.timer = this.warpDuration;
                    this.warpState = WarpState.Done;
                }
                break;

            case WarpState.Done:
                this.warpState = WarpState.Setup;
                playerState = PlayerState.Default;
                break;
        }
    }

    private void StartWarp() {
        this.warpState = WarpState.Warping;

        this.chargedAttack.DeactivateCorrespondingCollider(this.warpDirection);

        //Move player faster
        this.playerBody.velocity = new Vector2(this.warpDirVector.x * this.warpSpeed, 
            this.warpDirVector.y * this.warpSpeed);

        Debug.Log("player velocity during warp: " + this.playerBody.velocity);

        //Ignore collisions with enemies
        Physics2D.IgnoreLayerCollision(EnemiesLayer, PlayerLayer, true);

        //Hide sprite
        this.spriteRenderer.enabled = false;

        //Instantiate sword sprite and move it with player's position

        //Instantiate trail effects

    }

    private void EndWarp() {
        this.warpState = WarpState.EndStrike;

        this.chargedAttack.ActivateCorrespondingCollider(this.warpDirection);

        //Stop player's movement
        this.playerBody.velocity = Vector2.zero;

        //Undo collision ignore
        Physics2D.IgnoreLayerCollision(EnemiesLayer, PlayerLayer, false);

        this.spriteRenderer.enabled = true;

        //Play Animation
        this.playerAnim.Play("End Warp Strike");

        //Get rid of sword sprite
        
        //Get rid of trail effects
    }

    private void FinishWarpAttack() {
        this.warpState = WarpState.Cooldown;

        this.timer = this.cooldownDuration;

        this.chargedAttack.DeactivateCorrespondingCollider(this.warpDirection);

    }

    //TODO: Do I make this state interruptable by enemy attacks?

}
