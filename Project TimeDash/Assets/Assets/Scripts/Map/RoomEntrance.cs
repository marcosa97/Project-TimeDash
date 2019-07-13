
using UnityEngine;
using UnityEngine.Playables;

public class RoomEntrance : MonoBehaviour {

    private RoomHandler roomHandler;
    private PlayableDirector playableDirector;

	// Use this for initialization
	void Start () {
        this.roomHandler = GetComponentInParent<RoomHandler>();
        this.playableDirector = GetComponent<PlayableDirector>();
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            //Entered new room
            this.roomHandler.ActivateRoom();

            //Play walk in cutscene
            
        }
    }
}
