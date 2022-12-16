using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    // Start is called before the first frame update
    void Start()
    {
        // Instances the players
        // The master client is who enter to the room first
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 initPosition = new Vector3(-30, 15, 50);
            PhotonNetwork.Instantiate(player1Prefab.name, initPosition, Quaternion.Euler(90, -90, -270));
            //PhotonNetwork.Instantiate(player2Prefab.name, initPosition, Quaternion.Euler(90, -90, -270));
            Debug.Log("Player 1 instantiated: I'm the master client");
        }
        else
        {
            Vector3 initPosition = new Vector3(30, 10, 20);
            //PhotonNetwork.Instantiate(player1Prefab.name, initPosition, Quaternion.Euler(90, -90, -270));
            PhotonNetwork.Instantiate(player2Prefab.name, initPosition, Quaternion.Euler(90, -90, -270));
            Debug.Log("Player 2 instantiated: I'm not the master client");
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("A player left the room");
        // Return to the lobby
        SceneManager.LoadScene("Lobby");
        Debug.Log("Joined to the lobby");
    }
}
