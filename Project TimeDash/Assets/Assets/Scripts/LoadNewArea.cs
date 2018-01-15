using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewArea : MonoBehaviour {
	//Name of level to load
	public string levelToLoad;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		//If the player enters the collider trigger
		if (other.gameObject.name == "Player") {
			SceneManager.LoadScene (levelToLoad);
		}
	}
}
