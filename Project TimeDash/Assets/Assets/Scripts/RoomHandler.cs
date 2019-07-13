using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour {

    //List of enemies inside room
    public List<GameObject> enemies;

    private RoomsDeactivator roomsDeactivator;
    private BoxCollider2D roomChecker;
    private SpriteMask roomMask;

	void Start () {
        this.roomsDeactivator = GetComponentInParent<RoomsDeactivator>();
        this.roomChecker = GetComponent<BoxCollider2D>();
        this.roomMask = GetComponent<SpriteMask>();
        this.roomMask.enabled = false;

        //Activate enemies
        foreach (GameObject i in this.enemies)
        {
            i.gameObject.SetActive(false);
        }
    }

    public void ActivateRoom() {
        this.roomMask.enabled = true;

        //Activate enemies
        foreach(GameObject i in this.enemies) {
            if (i != null) {
                i.gameObject.SetActive(true);
            }
        }
    }

    public void DeactivateRoom() {
        this.roomMask.enabled = false;

        //Deactivate enemies
        foreach(GameObject i in this.enemies) {
            if (i != null) {
                i.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            Debug.Log("Entered Room");

            //Deactivate other rooms
            this.roomsDeactivator.DeactivateAllRooms();

            ActivateRoom();   
        }
    }
}
