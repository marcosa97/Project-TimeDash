using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public Slider healthBar;
	public HealthManager playerHealth;

	private static bool UIExists;

	// Use this for initialization
	void Start () {
		//If UI doesn't exist yet
		if (!UIExists) {
			UIExists = true;
			DontDestroyOnLoad (transform.gameObject);
		} 
		else {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		healthBar.maxValue = playerHealth.maxHealth;
		healthBar.value = playerHealth.currentHealth;
	}
}
