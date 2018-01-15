using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour {

	public float slowDownFactor; //default value = 0.05f
	public float slowDownLength;

	private float originalFixedDeltaTime;
	private static bool slowMotionActivated;
	private static bool slowMotionDeactivating; //Will only be used by DeactivateSlowMotion
	private static bool timeManagerExists;

	void Start() {
		slowMotionActivated = false;

		//If Player doesn't exist yet
		if (!timeManagerExists) {
			timeManagerExists = true;
			DontDestroyOnLoad (transform.gameObject);
		} 
		else {
			Destroy (gameObject);
		}
	}

	void Update() {

		/*
		//if slow motion is activated
		if (Time.timeScale < 1f) {
			//Return to normal speed -> we use unscaledDeltaTime because it's not affected by timeScale
			Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime;
			Time.fixedDeltaTime = originalFixedDeltaTime;
			Time.timeScale = Mathf.Clamp (Time.timeScale, 0f, 1f);
		}
		*/
		//if slow motion is activated
		if (slowMotionDeactivating) {
			SpeedUpToNormalSpeed ();
		}

	}
		

	//Slows down the game by the slow down factor
	public void ActivateSlowMotion() {
		//Time scale = 1 is normal real time
		//Slow down factor will be 1/slowDownFactor, ex) 1/0.05 = 20 times slower!
		Time.timeScale = slowDownFactor;

		//We need to adjust how many times per second the physics get updated
		//  because slowing down time will cause slower fixed updates
		//  Down here, we want physics to update like about 50 times per sec, so we multiply by 1/50 = 0.2
		originalFixedDeltaTime = Time.fixedDeltaTime;
		Time.fixedDeltaTime = Time.timeScale * .02f;
		slowMotionActivated = true;
	}

	public void DeactivateSlowMotion() {
		//Code for deactivating slow motion is in Update()
		slowMotionDeactivating = true;
		slowMotionActivated = false;
		StartCoroutine ("SpeedUpToNormalSpeed");
	}

	//Coroutine : A coroutine is like a function that has the ability to pause execution and return control to Unity but then
	//            to continue where it left off on the following frame
	IEnumerator SpeedUpToNormalSpeed() {
		//if slow motion is activated
		//if (Time.timeScale < 1f) {

		//Return to normal speed -> we use unscaledDeltaTime because it's not affected by timeScale
		while (Time.timeScale < 1f) {
		//for (Time.timeScale; Time.timeScale < 1f; Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime) {
			Time.timeScale += (1f / slowDownLength) * Time.unscaledDeltaTime;
			Time.fixedDeltaTime = originalFixedDeltaTime;
			Time.timeScale = Mathf.Clamp (Time.timeScale, 0f, 1f);

			if (Time.timeScale == 1f) {
				slowMotionDeactivating = false;
				Debug.Log ("SLOW MOTION SUCCESSFULLY DEACTIVATED!");
			}

			yield return null;
		}
			
	}

	public bool SlowMotionActive() {
		return slowMotionActivated;
	}

}
