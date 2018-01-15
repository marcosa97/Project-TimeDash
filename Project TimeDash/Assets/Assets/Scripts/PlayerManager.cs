using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We use this so we can keep a reference to the player at all times
public class PlayerManager : MonoBehaviour {

	#region Singleton

	public static PlayerManager instance;

	void Awake() {
		instance = this;
	}

	#endregion

	public GameObject player;
}
