
using UnityEngine;

public class FallPit : MonoBehaviour {

    private PlayerController playerController;

	void Start () {
        this.playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && playerController.playerState != PlayerState.WarpStrike) {
            this.playerController.MakePlayerFall();
        }

        if (collision.tag == "Enemy") {
            //Kill Enemy
        }
    }
}
