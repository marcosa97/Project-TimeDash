using UnityEngine;

public class SlimeController : MonoBehaviour {

    public float moveSpeed;
    public float hurtTime;
    public float attackRange;

    private enum EnemyState {
        Pursuit,
        Attack,
        Hurt,
        Idle
    }

    //Reference for health stuff
    private HealthComponent healthComponent;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private Vector2 lastMove;
    [SerializeField]
    private EnemyState state;
    private FourDirectionSystem dirHandler;

    public AttackColliderController attackUp;
    public AttackColliderController attackRight;
    public AttackColliderController attackDown;
    public AttackColliderController attackLeft;
    
    //For making sprite glow
    [Header("Sprite Glow Properties")]
    public float smoothTime;
    private float smoothVelocity;

    private float timer;

	void Start () {
        this.playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.rb = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.healthComponent = GetComponent<HealthComponent>();
        this.state = EnemyState.Idle;
        this.dirHandler = new FourDirectionSystem();
        //this.gameObject.SetActive(false);
	}


    void FixedUpdate() {
            switch (this.state) {
            case EnemyState.Idle:
                break;

            case EnemyState.Pursuit:
                anim.Play("SlimeWalk");
                var moveDirection = transform.InverseTransformPoint(playerTransform.position);
                moveDirection.Normalize();
                this.rb.velocity = moveDirection * moveSpeed;
                lastMove = moveDirection;
                anim.SetFloat("MoveX", lastMove.x);
                anim.SetFloat("MoveY", lastMove.y);

                if ( PlayerInRange() ) {
                    this.state = EnemyState.Attack;
                }

                break;
            case EnemyState.Attack:
                //Play animation
                anim.Play("SlimeAttack");

                //Play sound effect


                break;

            case EnemyState.Hurt:
                this.timer -= Time.deltaTime;

                //Make glow fade out
                spriteRenderer.material.SetFloat("_FlashAmount",
                    Mathf.SmoothDamp(spriteRenderer.material.GetFloat("_FlashAmount"), 
                    0f, ref smoothVelocity, smoothTime));
                    //Mathf.PingPong(timer * 4.7f, 0.60f));

                if (this.timer <= 0f) {
                    this.timer = 0f;
                    this.state = EnemyState.Pursuit;
                    spriteRenderer.material.SetFloat("_FlashAmount", 0f);
                }
                break;

        }
	}

    private void StopAttack() {
        this.state = EnemyState.Pursuit;
        DisableAttackCollider();
    }

    private bool PlayerInRange() {
        //Check if player is within range
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        if (distance <= attackRange) {
            return true;
        } else {
            return false;
        }
    }

    private void EnableAttackCollider() {
        //See which way we're facing
        FourDirections dir = dirHandler.GetDirectionFromVector(lastMove);

        switch (dir) {
            case FourDirections.North:
                this.attackUp.EnableAttackCollider();
                break;
            case FourDirections.East:
                this.attackRight.EnableAttackCollider();
                break;
            case FourDirections.South:
                this.attackDown.EnableAttackCollider();
                break;
            case FourDirections.West:
                this.attackLeft.EnableAttackCollider();
                break;
        }
    }

    private void DisableAttackCollider() {
        //See which way we're facing
        FourDirections dir = dirHandler.GetDirectionFromVector(lastMove);

        switch (dir)
        {
            case FourDirections.North:
                this.attackUp.DisableAttackCollider();
                break;
            case FourDirections.East:
                this.attackRight.DisableAttackCollider();
                break;
            case FourDirections.South:
                this.attackDown.DisableAttackCollider();
                break;
            case FourDirections.West:
                this.attackLeft.DisableAttackCollider();
                break;
        }
    }

    //Function for when the enemy is hit
    private void ObjectHit(AttackInfoContainer obj)
    {
        Debug.Log("SLIME HIT");
        healthComponent.TakeDamage(obj.damage);

        this.state = EnemyState.Hurt;
        this.timer = hurtTime;

        spriteRenderer.material.SetFloat("_FlashAmount", 0.60f);

        //If already in attack state, don't interrupt it
        /*
        if (this.state != EnemyState.Attack)
        {

            //Change to hurt state
            //flinchTimer = flinchTime;

            //If already in hurt state, don't update previous state
            if (enemyState != EnemyBaseState.Hurt)
            {
                previousState = enemyState;
            }
            enemyState = EnemyBaseState.Hurt;

        }
        */
        //this.rb.velocity = Vector2.zero;
        this.rb.AddForce(obj.direction * obj.force);
    }

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    //For debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
