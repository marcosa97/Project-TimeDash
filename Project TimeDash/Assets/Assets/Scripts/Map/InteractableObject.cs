
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    public enum InteractableType {
        Chest,
        Door,
        Lever
    }

    public enum ChestReward {
        HPPotion,
        SPPotion,
        Key,
        NotApplicable
    }

    //Object's direction relative to the player
    public FourDirections objectDirection;
    public InteractableType objectType;
    public ChestReward chestReward;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private AbilityBasicMovement playerMovementSystem;
    private SpriteRenderer spriteRenderer;
    public GameObject PromptUI;
    private bool isActive;

	// Use this for initialization
	void Start () {
        this.playerMovementSystem = GameObject.Find("Player").GetComponent<AbilityBasicMovement>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.PromptUI.SetActive(false);
        this.isActive = true;
	}

    public bool IsActive() {
        return this.isActive;
    }

    public void DeactivateInteractable() {
        this.isActive = false;

        this.spriteRenderer.sprite = this.inactiveSprite;

        this.PromptUI.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //Player has to be facing the object && interactable object has to be active
        if (collision.tag == "Player") { 
            if (this.objectDirection == this.playerMovementSystem.GetFaceDirectionIn4DirSystem()
                && this.isActive == true) {
                //Display interaction prompt
                this.PromptUI.SetActive(true);
            } else {
                this.PromptUI.SetActive(false);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            if (this.PromptUI != null) {
                this.PromptUI.SetActive(false);
            }
        }
    }

}
