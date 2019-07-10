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
        this.currentSP += amount;

        //If current SP is at max
        if (this.currentSP >= this.maxSP) {
            //If bubbles are full, keep SP bar full, else empty bar and a fill bubble
            if (this.currentBubbles == this.maxBubbles) {
                this.currentSP = this.maxSP;
            } else {
                FillSPBubbles();

                this.currentSP = this.currentSP % this.maxSP;
            }
        }

        UpdateSPBar();
    }

    public void DecrementSP(int amount) {
        this.currentSP -= amount;

        //If current SP falls below 0
        if (this.currentSP <= 0) {

            //Remove SP bubble if there's filled bubbles
            if (this.currentBubbles > 0) {
                RemoveSPBubbles();

                //leftover SP in bar after removing bubbles
                this.currentSP = this.maxSP - (-this.currentSP % this.maxSP);
            } else {
                //No filled bubbles means player ran out of all SP
                this.currentSP = 0;
            }
        }

        UpdateSPBar();
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

    private void FillSPBubbles() {
        //Calculate how many bubbles to fill -> integer division floors
        int numBubbles = this.currentSP / this.maxSP;

        this.currentBubbles += numBubbles;
        UpdateSPBubblesUI();
    }

    private void RemoveSPBubbles() {
        //Calculate how many bubbles to remove
        //Note: +1 because the "if" case in caller function
        //Note: negate currentSP because it's already negative
        int numBubbles = 1 + (-this.currentSP / this.maxSP);

        this.currentBubbles -= numBubbles;
        UpdateSPBubblesUI();
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
