
using UnityEngine;

public class AbilityFall : MonoBehaviour {

    private enum FallState {
        Setup,
        Falling,
        Done
    }

    public int FallDamageAmount;
    private PlayerHealthComponent healthComponent;
    private Animator playerAnimator;
    private FallState fallState;
    private Vector2 activeRespawnPoint;

	void Start () {
        this.healthComponent = GetComponent<PlayerHealthComponent>();
        this.playerAnimator = GetComponent<Animator>();
        this.fallState = FallState.Setup;
	}

    public void SetActiveRespawnPoint(Vector2 newPoint) {
        this.activeRespawnPoint = newPoint;
    }
	
	public void Fall(ref PlayerState playerState) {
        switch(this.fallState) {
            case FallState.Setup:
                this.fallState = FallState.Falling;
                this.playerAnimator.Play("Falling Down");
                break;

            case FallState.Falling:
                break;

            case FallState.Done:
                playerState = PlayerState.Default;
                this.fallState = FallState.Setup;
                this.healthComponent.TakeDamage(this.FallDamageAmount);

                //Move player position to respawn point
                this.gameObject.transform.position = this.activeRespawnPoint;

                //Make player sprite blink
                break;
        }
    }

    public void StopFalling() {
        this.fallState = FallState.Done;
    }
}
