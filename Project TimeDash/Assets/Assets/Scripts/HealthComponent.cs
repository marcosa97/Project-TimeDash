
using UnityEngine;
using UnityEngine.UI;

//Taken from Unity forums user novashot
public class HealthComponent : MonoBehaviour
{

    public int maxHealth;
    [SerializeField]
    private int currentHealth;

    public Image healthBar;
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
        healthBar.fillAmount = 1; //full health
                                  /*
                                  if (canRegen) {
                                      InvokeRepeating ("Regen", regenTime, regenTime);
                                  } */
    }

    public void TakeDamage(int amount)
    {
        //Cancel regeneration if hit
        /*
		if (IsInvoking ("Regen")) {
			CancelInvoke ("Regen");
		} */

        currentHealth -= amount;
        healthBar.fillAmount = (float)currentHealth / maxHealth;

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