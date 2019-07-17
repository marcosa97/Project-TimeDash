using UnityEngine;

public class AbilityDialogue : MonoBehaviour {

    private enum DialogueState {
        Setup,
        Dialoguing,
        Done
    }

    //State to returnto after dialogue is over
    [SerializeField]
    private DialogueState dialogueState;
    private PlayerState prevState;
    private Dialogue dialogue;
    private DialogueManager dialogueManager;

	void Start () {
        this.dialogueState = DialogueState.Setup;
        this.dialogueManager = FindObjectOfType<DialogueManager>();
	}

    //Setup before going into this state. Called by another state
    public void Setup(PlayerState state, InteractableObject interactable) {
        this.dialogue = interactable.GetCorrespondingDialogue();
        Debug.Log("Setup: " + this.dialogue.sentences[0]);
        this.prevState = state;
    }
	
	public void Dialogue(ref PlayerState playerState) {
        switch(this.dialogueState) {
            case DialogueState.Setup:
                this.dialogueManager.StartDialogue(dialogue);
                this.dialogueState = DialogueState.Dialoguing;
                break;
            case DialogueState.Dialoguing:
                bool notDone = true;
                if (Input.GetButtonDown("Interact")) {
                    notDone = this.dialogueManager.DisplayNextSentence();
                }

                if (notDone == false) {
                    this.dialogueState = DialogueState.Done;
                }
                break;
            case DialogueState.Done:
                playerState = this.prevState;
                this.dialogueState = DialogueState.Setup;
                break;
        }
    }
}
