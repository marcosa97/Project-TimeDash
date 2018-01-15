using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public GameObject followTarget;
	private Vector3 targetPos;
	public float moveSpeed; //how fast the camera follows the player

	private static bool cameraExists; 

	// Use this for initialization
	void Start () {
		//DontDestroyOnLoad(transform.gameObject);

		//If Camera doesn't exist yet
		if (!cameraExists) {
			cameraExists = true;
			DontDestroyOnLoad (transform.gameObject);
		} 
		else {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		targetPos = new Vector3 (followTarget.transform.position.x, followTarget.transform.position.y, transform.position.z);

		//Lerp trick used here: gives the effect of camera slowing down when reaching targetPos (the player)
		transform.position = Vector3.Lerp (transform.position, targetPos, moveSpeed * Time.deltaTime);
	}
}
