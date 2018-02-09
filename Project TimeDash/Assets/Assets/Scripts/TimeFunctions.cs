using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFunctions : MonoBehaviour {
	//Public settings
	public float slowDownFactor; //How much to slow down time by
	public float slowDownLength; //How long to slow down for

	//Refencences and variables needed
	private static bool timeManagerExists;
	private float originalFixedDeltaTime;

	// Use this for initialization
	void Start () {
		//If Player doesn't exist yet
		if (!timeManagerExists) {
			timeManagerExists = true;
			DontDestroyOnLoad (transform.gameObject);
		} 
		else {
			Destroy (gameObject);
		}
	}

	//Coroutine : A coroutine is like a function that has the ability to pause execution and return control to Unity but then
	//            to continue where it left off on the following frame
	//HitStop() : slows down time for a tiny fraction of a second and gradually speeds it up
	//  CONSIDER: Maybe make it take in a parameter to activate longer hit stops for stronger attacks
	IEnumerator HitStop() {
		//Setup slow motion
		//Time scale = 1 is normal real time
		//Slow down factor will be 1/slowDownFactor, ex) 1/0.05 = 20 times slower!
		Time.timeScale = slowDownFactor;

		//We need to adjust how many times per second the physics get updated
		//  because slowing down time will cause slower fixed updates
		//  Down here, we want physics to update like about 50 times per sec, so we multiply by 1/50 = 0.2
		originalFixedDeltaTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime = Time.timeScale * .02f;

		//Return to normal speed -> we use unscaledDeltaTime because it's not affected by timeScale
		while (Time.timeScale < 1f) {
			Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime;
			Time.fixedDeltaTime = originalFixedDeltaTime;
			Time.timeScale = Mathf.Clamp (Time.timeScale, 0f, 1f);

			if (Time.timeScale == 1f) {
				//slowMotionDeactivating = false;
				Debug.Log ("SLOW MOTION SUCCESSFULLY DEACTIVATED!");
			}

			//Yield control to the main program
			yield return null;
		}

	}
}
