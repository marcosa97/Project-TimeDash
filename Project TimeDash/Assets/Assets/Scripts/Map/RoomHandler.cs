using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour {

    //List of enemies inside room
    public List<GameObject> enemies;

    private RoomsDeactivator roomsDeactivator;
    private SpriteMask roomMask;
    private bool insideRoom;

	void Start () {
        this.roomsDeactivator = GetComponentInParent<RoomsDeactivator>();
        this.roomMask = GetComponent<SpriteMask>();
        this.roomMask.enabled = false;
        this.insideRoom = false;

        //Activate enemies
        foreach (GameObject i in this.enemies)
        {
            i.gameObject.SetActive(false);
        }
    }

    public void ActivateRoom() {

        if (insideRoom == true) {
            return;
        }

        this.roomsDeactivator.DeactivateAllRooms();

        this.roomMask.enabled = true;
        this.insideRoom = true;

        //Activate enemies
        foreach(GameObject i in this.enemies) {
            if (i != null) {
                i.gameObject.SetActive(true);
            }
        }
    }

    public void DeactivateRoom() {
        this.roomMask.enabled = false;
        this.insideRoom = false;

        //Deactivate enemies
        foreach(GameObject i in this.enemies) {
            if (i != null) {
                i.gameObject.SetActive(false);
            }
        }
    }

    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            Debug.Log("Entered Room");

            ActivateRoom();

            //Play cutscene
            //this.timelineController.Play();
        }
    }
    */
}
