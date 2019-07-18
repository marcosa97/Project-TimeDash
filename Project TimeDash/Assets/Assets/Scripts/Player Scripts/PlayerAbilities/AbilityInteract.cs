using UnityEngine;

public class AbilityInteract : MonoBehaviour {

    private enum InteractState {
        Check,
        Done
    }

    public Sprite hpPotionSprite;
    public Sprite spPotionSprite;
    public Sprite keySprite;

    private InteractState state;
    private Animator animator;
    private AbilityBasicMovement playerMovementSystem;
    private FourDirectionSystem DirectionSystem;
    private AbilityDialogue playerDialogueState;
    public SpriteRenderer holdItem;
    public LayerMask whatToHit;

    private PlayerHealthComponent HPHandler;
    private PlayerSPComponent SPHandler;
    private PlayerKeyInventory KeyInventory;

	// Use this for initialization
	void Start () {
        this.state = InteractState.Check;
        this.animator = GetComponent<Animator>();
        this.playerMovementSystem = GetComponent<AbilityBasicMovement>();
        this.playerDialogueState = GameObject.Find("Player").GetComponent<AbilityDialogue>();
        this.DirectionSystem = new FourDirectionSystem();
        this.holdItem.enabled = false;

        this.HPHandler = GetComponent<PlayerHealthComponent>();
        this.SPHandler = GetComponent<PlayerSPComponent>();
        this.KeyInventory = GetComponent<PlayerKeyInventory>();
	}

    //Animation event
    private void ShowHoldItem()
    {
        this.holdItem.enabled = true;
    }

    private void HideHoldItem() {
        this.holdItem.enabled = false;
    }

    private void HandleChestInteractable(InteractableObject interactable, ref PlayerState playerState) {
        Debug.Log("Interacted With Chest");
        interactable.DeactivateInteractable();

        //Play animation
        this.animator.Play("gotItem");

        var PotionValues = ScriptableObject.CreateInstance<PlayerSPValues>();

        //Increase key count, hp, or sp 
        switch (interactable.chestReward) {
            case InteractableObject.ChestReward.HPPotion:
                this.holdItem.sprite = hpPotionSprite;
                this.HPHandler.Heal(PotionValues.HPPotionGain);  
                break;
            case InteractableObject.ChestReward.SPPotion:
                this.holdItem.sprite = spPotionSprite;
                this.SPHandler.IncrementSP(PotionValues.SPPotionGain);
                break;
            case InteractableObject.ChestReward.Key:
                this.holdItem.sprite = keySprite;
                this.KeyInventory.IncrementKeyCount();
                break;
        }

        //If first time getting an HP Potion, activate dialogue
        this.playerDialogueState.Setup(playerState, interactable);

        //Go into dialogue state
        playerState = PlayerState.Dialogue;
        this.state = InteractState.Done;
    }

    private void HandleDoorInteractable(InteractableObject interactable, ref PlayerState playerState) {
        

        if (this.KeyInventory.GetKeyCount() > 0) {
            interactable.DeactivateInteractable();
            this.KeyInventory.DecrementKeyCount();
        } else {
            this.playerDialogueState.Setup(playerState, interactable);
            playerState = PlayerState.Dialogue;
        }

        
        this.state = InteractState.Done;
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
                        HandleChestInteractable(interactable, ref playerState);
                        break;

                    case InteractableObject.InteractableType.Door:
                        HandleDoorInteractable(interactable, ref playerState);
                        break;

                    case InteractableObject.InteractableType.Lever:
                        break;
                }

                break;

            case InteractState.Done:
                HideHoldItem();
                this.state = InteractState.Check;
                playerState = PlayerState.Default;
                break;
        }
    }
}
