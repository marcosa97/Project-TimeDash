using UnityEngine;
using UnityEngine.UI;

public class PlayerSPComponent : MonoBehaviour {

    public int maxSP;
    [SerializeField]
    private int currentSP;

    public int maxBubbles;
    [SerializeField]
    private int currentBubbles;

    public RectTransform SPTransform;
    public Image SPFill;
    private float rectY;
    private float minXValue;
    private float maxXValue;
    public Image[] SPBubbleFills;

	void Start () {
        this.currentSP = 0;
        this.currentBubbles = 0;
        this.rectY = this.SPTransform.position.y;
        this.maxXValue = this.SPTransform.position.x;
        this.minXValue = this.SPTransform.position.x - this.SPTransform.rect.width;

        UpdateSPBar();
        UpdateSPBubblesUI();
	}
	
	public void IncrementSP(int amount) {
        int totalSP = this.currentSP + (this.currentBubbles * this.maxSP);

        totalSP += amount;

        //Cap SP to total max
        int maxSPPossible = (this.maxBubbles + 1) * this.maxSP;
        if (totalSP > maxSPPossible) {
            totalSP = maxSPPossible;
        }

        
        this.currentBubbles = totalSP / this.maxSP;
        if (this.currentBubbles > this.maxBubbles) {
            this.currentBubbles = this.maxBubbles;
        }

        //Special case: all bubbles are filled
        if (this.currentBubbles != this.maxBubbles) {
            this.currentSP = totalSP % this.maxSP;
        } else {
            this.currentSP = totalSP - (this.maxSP * this.maxBubbles);
        }

        //Update UI
        UpdateSPBar();
        UpdateSPBubblesUI();
    }

    public void DecrementSP(int amount) {
        int totalSP = this.currentSP + (this.currentBubbles * this.maxSP);

        totalSP -= amount;

        this.currentBubbles = totalSP / this.maxSP;
        this.currentSP = totalSP % this.maxSP;

        //Update UI
        UpdateSPBar();
        UpdateSPBubblesUI();
    }

    //Checks if player has enough SP to perform move that requires @amount of SP
    public bool HasEnoughSP(int requiredAmount) {
        int totalSP = this.currentSP + (this.currentBubbles * this.maxSP);
        if ( totalSP >= requiredAmount ) {
            return true;
        } else {
            return false;
        }
    }

    private void UpdateSPBubblesUI() {
        for (int i = 0; i < this.maxBubbles; i++) {
            if (i < this.currentBubbles) {
                this.SPBubbleFills[i].enabled = true;
            } else {
                this.SPBubbleFills[i].enabled = false;
            }
        }
    }

    private void UpdateSPBar() {
        float currentXValue = MapValues(this.currentSP, 0,
            this.maxSP, this.minXValue, this.maxXValue);

        this.SPTransform.position = new Vector3(currentXValue, this.rectY);
    }

    private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
