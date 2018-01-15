using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	public int maxHealth;
	public int currentHealth;

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		//PLACEHOLDER CODE FOR NOW
		if (currentHealth <= 0) {
			gameObject.SetActive (false);
		}
	}

	public void ReceiveDamage(int damage) {
		currentHealth -= damage;
	}

	public void RestoreHealth(int healthToRecover) {
		currentHealth += healthToRecover;
	}

	public void RestoreMaxHealth() {
		currentHealth = maxHealth;
	}
		
}
