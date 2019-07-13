using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomsDeactivator : MonoBehaviour {

    public RoomHandler[] roomHandlers;

	// Use this for initialization
	void Start () {
        this.roomHandlers = GetComponentsInChildren<RoomHandler>();

        DeactivateAllRooms();
	}

    public void DeactivateAllRooms() {
        foreach (RoomHandler rh in this.roomHandlers) {
            rh.DeactivateRoom();
        }
    }
}
