using UnityEngine;

public class AbilityInteract : MonoBehaviour {

    private enum InteractState {
        Check,
        InteractChest,
        Done
    }

    public Sprite hpPotionSprite;
    public Sprite spPotionSprite;
    public Sprite keySprite;

    private InteractState state;
    private Animator animator;
    private AbilityBasicMovement playerMovementSystem;
    private FourDirectionSystem DirectionSystem;
    public SpriteRenderer holdItem;
    public LayerMask whatToHit;

	// Use this for initialization
	void Start () {
        this.state = InteractState.Check;
        this.animator = GetComponent<Animator>();
        this.playerMovementSystem = GetComponent<AbilityBasicMovement>();
        this.DirectionSystem = new FourDirectionSystem();
        this.holdItem.enabled = false;
	}

    //Animation event
    private void ShowHoldItem()
    {
        this.holdItem.enabled = true;
    }

    private void HideHoldItem() {
        this.holdItem.enabled = false;
    }

    private void HandleChestInteractable(InteractableObject interactable) {
        Debug.Log("Interacted With Chest");
        interactable.DeactivateInteractable();

        //Play animation
        this.animator.Play("gotItem");

        //Increase key count, hp, or sp 
        switch (interactable.chestReward) {
            case InteractableObject.ChestReward.HPPotion:
                this.holdItem.sprite = hpPotionSprite;
                break;
            case InteractableObject.ChestReward.SPPotion:
                this.holdItem.sprite = spPotionSprite;
                break;
            case InteractableObject.ChestReward.Key:
                this.holdItem.sprite = keySprite;
                break;
        }

        //Go into dialogue state
        this.state = InteractState.InteractChest;
    }

    public void Interact(ref PlayerState playerState) {
        switch(state) {
            case InteractState.Check:
                //Determine direction to raycast
                FourDirections dir = this.playerMovementSystem.GetFaceDirectionIn4DirSystem();

                //Raycast again to get information about interactable object
                RaycastHit2D hit = Physics2D.Raycast(transform.position, 
                    this.DirectionSystem.GetVectorFromDirection(dir), 1, this.whatToHit);

                //If mistake, go back to default
                if (hit.collider == null || hit.collider.tag != "InteractableStaticObject" 
                    || hit.collider.gameObject.GetComponent<InteractableObject>().IsActive() == false) {
                    Debug.Log("No interactable object in range");
                    playerState = PlayerState.Default;
                    return;
                }

                //Determine which type of interactable we hit
                InteractableObject interactable = hit.collider.gameObject.GetComponent<InteractableObject>();

                if (interactable == null) {
                    playerState = PlayerState.Default;
                    return;
                }

                switch (interactable.objectType)
                {
                    case InteractableObject.InteractableType.Chest:
                        HandleChestInteractable(interactable);
                        break;

                    case InteractableObject.InteractableType.Door:
                        break;

                    case InteractableObject.InteractableType.Lever:
                        break;
                }

                break;

            case InteractState.InteractChest:
                //Play interact animation

                //Go into dialogue mode -> The whole scene is paused when this happens
                //this.state = InteractState.Done;

                break;

            case InteractState.Done:
                HideHoldItem();
                this.state = InteractState.Check;
                playerState = PlayerState.Default;
                break;
        }
    }
}
