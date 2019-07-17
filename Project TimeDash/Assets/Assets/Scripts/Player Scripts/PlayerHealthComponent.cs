using UnityEngine;
using UnityEngine.UI;

//Taken from Unity forums user novashot
public class PlayerHealthComponent : MonoBehaviour
{

    public int maxHealth;
    [SerializeField]
    private int currentHealth;

    //public Image healthBar;
    public RectTransform healthTransform;
    public Image healthFill;
    private float rectY;
    private float minXValue;
    private float maxXValue;

    /*
	public bool canRegen; //can turn regen on or off
	public int regenAmount; //amount to heal per regen tick
	public float regenTime; //time between regen ticks in seconds
	public float regenHitDelay; //how long after we are hit can we regen
	*/

    // Use this for initialization
    void Start()
    {
        currentHealth = maxHealth;
        //healthBar.fillAmount = 1; //full health
        /*
        if (canRegen) {
            InvokeRepeating ("Regen", regenTime, regenTime);
        } */
        rectY = healthTransform.position.y;
        maxXValue = healthTransform.position.x;
        minXValue = healthTransform.position.x - healthTransform.rect.width;
    }

    public void Heal(int amount) {
        currentHealth += amount;
        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }

        UpdateHealth();
    }

    public void TakeDamage(int amount)
    {
        //Cancel regeneration if hit
        /*
		if (IsInvoking ("Regen")) {
			CancelInvoke ("Regen");
		} */

        currentHealth -= amount;
        UpdateHealth();
        //healthBar.fillAmount = (float) currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        //if we can regen, resume after hit delay
        /*
		if (canRegen) {
			InvokeRepeating ("Regen", regenHitDelay, regenTime);
		} */
    }

    public void Die()
    {
        //Destroy or pool game object
        Debug.Log("Dead");

        //Add screen shake

        Destroy(this.gameObject);
    }

    private void UpdateHealth()
    {
        float currentXValue = MapValues(this.currentHealth, 0,
            this.maxHealth, this.minXValue, this.maxXValue);

        this.healthTransform.position = new Vector3(currentXValue, this.rectY);

        //Change health bar color
        if (currentHealth > maxHealth / 2)
        {
            healthFill.color = new Color32((byte)MapValues(currentHealth, maxHealth / 2, maxHealth, 255, 0), 255, 0, 255);
        }
        else
        {
            healthFill.color = new Color32(255, (byte)MapValues(currentHealth, 0, maxHealth / 2, 0, 255), 0, 255);
        }
    }

    //Maps current health to corresponding health bar's x position in the UI canvas
    //@x : current health
    //@inMin : minimum health (0)
    //@inMax: max health 
    //@outMin: min x position of health bar rect
    //@outMax: max x position of health bar rect
    private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    /*
	void Regen() {
		currentHealth += regenAmount;

		if (currentHealth <= maxHealth) {
			currentHealth = maxHealth;

			if (IsInvoking ("Regen")) {
				CancelInvoke("Regen");
			}
		}
	}
	*/
}
