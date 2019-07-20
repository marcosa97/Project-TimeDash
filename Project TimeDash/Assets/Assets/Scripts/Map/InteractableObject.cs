
using UnityEngine;
using UnityEngine.UI;

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

    public DialogueTexts dialogueTexts;

    //Object's direction relative to the player
    public FourDirections objectDirection;
    public InteractableType objectType;
    public ChestReward chestReward;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private AbilityBasicMovement playerMovementSystem;
    private SpriteRenderer spriteRenderer;
    private PlayerKeyInventory keyInventory;
    public GameObject PromptUI;
    public Text promptText;
    [SerializeField]
    private bool isActive;

	// Use this for initialization
	void Start () {
        GameObject player = GameObject.Find("Player");
        this.playerMovementSystem = player.GetComponent<AbilityBasicMovement>();
        this.keyInventory = player.GetComponent<PlayerKeyInventory>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.PromptUI.SetActive(false);
        this.isActive = true;
	}

    public Dialogue GetCorrespondingDialogue()
    {
        Dialogue dialogue = null;

        switch(this.objectType) {
            case InteractableType.Chest:
                if (this.chestReward == ChestReward.HPPotion) {
                    dialogue = this.dialogueTexts.GotHPPotion;
                } else if (this.chestReward == ChestReward.SPPotion) {
                    dialogue = this.dialogueTexts.GotSPPotion;
                } else if (this.chestReward == ChestReward.Key) {
                    dialogue = this.dialogueTexts.GotKey;
                }
                break;
            case InteractableType.Door:
                //Will only enter this state if player doesn't have a key
                dialogue = this.dialogueTexts.NoDoorKey;
                break;
            case InteractableType.Lever:
                break;
        }

        return dialogue;
    }

    public bool IsActive() {
        return this.isActive;
    }

    public void DeactivateInteractable() {
        this.isActive = false;

        switch (this.objectType) {
            case InteractableType.Chest:
                if (this.spriteRenderer != null) {
                    this.spriteRenderer.sprite = this.inactiveSprite;
                }
                break;
            case InteractableType.Door:
                Destroy(this.gameObject);
                break;
        }

        this.PromptUI.SetActive(false);
    }

    private void SetUIText() {
        switch (this.objectType)
        {
            case InteractableType.Chest:
                this.promptText.text = "Open";
                break;
            case InteractableType.Door:
                if (this.keyInventory.GetKeyCount() == 0) {
                    this.promptText.text = "Inspect";
                } else {
                    this.promptText.text = "Open";
                }
                break;
            case InteractableType.Lever:
                break;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //Player has to be facing the object && interactable object has to be active
        if (collision.tag == "Player") { 
            if (this.objectDirection == this.playerMovementSystem.GetFaceDirectionIn4DirSystem()
                && this.isActive == true) {
                //Display interaction prompt
                SetUIText();
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
