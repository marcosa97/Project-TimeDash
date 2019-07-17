
using UnityEngine;
using UnityEngine.UI;

public class PlayerKeyInventory : MonoBehaviour {

    private int keyCount;
    public Text count;
    
	void Start () {
        this.keyCount = 0;
        this.count.text = keyCount.ToString();
	}

    public void IncrementKeyCount() {
        this.keyCount++;
        this.count.text = keyCount.ToString();
    }

    public void DecrementKeyCount() {
        this.keyCount--;

        if (this.keyCount < 0) {
            this.keyCount = 0;
        }

        this.count.text = keyCount.ToString();
    }
	
}
