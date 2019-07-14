
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour {

    public enum DoorType {
        EnemyLocked,
        KeyLocked
    }

    //List of enemies inside room
    public List<GameObject> enemies;
    public DoorType doorType;

    private DoorOpener doorOpener;
    private RoomsDeactivator roomsDeactivator;
    private SpriteMask roomMask;
    private bool insideRoom;
    [SerializeField]
    private int numEnemies;

	void Start () {
        this.doorOpener = GetComponent<DoorOpener>();
        this.roomsDeactivator = GetComponentInParent<RoomsDeactivator>();
        this.roomMask = GetComponent<SpriteMask>();
        this.roomMask.enabled = false;
        this.insideRoom = false;
        this.numEnemies = 0;

        //Activate enemies
        foreach (GameObject i in this.enemies)
        {
            i.gameObject.SetActive(false);
            this.numEnemies++;
        }
    }

    public void DecrementNumEnemies() {
        this.numEnemies--;

        if (this.numEnemies == 0) {
            //Open locked bar door if there is one
            if (this.doorOpener != null && this.doorType == DoorType.EnemyLocked) {
                this.doorOpener.RemoveDoorTile();
            }

            Debug.Log("Killed all enemies in room");
        }
    }

    public void ActivateRoom() {

        if (insideRoom == true) {
            return;
        }

        if (this.roomsDeactivator != null) {
            this.roomsDeactivator.DeactivateAllRooms();
        }

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
}
