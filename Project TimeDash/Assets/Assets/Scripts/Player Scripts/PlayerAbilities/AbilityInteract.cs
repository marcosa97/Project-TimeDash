using UnityEngine;

public class AbilityInteract : MonoBehaviour {

    private enum InteractState {
        Check,
        InteractChest,
        Done
    }

    private InteractState state;
    private Animator animator;
    public LayerMask whatToHit;

	// Use this for initialization
	void Start () {
        this.state = InteractState.Check;
        this.animator = GetComponent<Animator>();
	}

    public void ExitState() {
        this.state = InteractState.Done;
        return;
    }


    public void Interact(ref PlayerState playerState) {
        switch(state) {
            case InteractState.Check:
                //Raycast again to get information about interactable object
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1, this.whatToHit);

                //If mistake, go back to default
                if (hit.collider == null || hit.collider.tag != "InteractableStaticObject" 
                    || hit.collider.gameObject.GetComponent<InteractableObject>().IsActive() == false) {
                    ExitState();
                }

                //Determine which type of interactable we hit
                InteractableObject interactable = hit.collider.gameObject.GetComponent<InteractableObject>();

                if (interactable == null) {
                    ExitState();
                }

                switch (interactable.objectType)
                {
                    case InteractableObject.InteractableType.Chest:
                        Debug.Log("Interacted With Chest");
                        interactable.DeactivateInteractable();

                        //Go into got item state
                        this.state = InteractState.InteractChest;
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
                ExitState();

                break;

            case InteractState.Done:
                this.state = InteractState.Check;
                playerState = PlayerState.Default;
                break;
        }
    }
}
