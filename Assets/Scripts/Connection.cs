using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class Connection : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // Connection
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to the server");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Server connection done");

        PhotonNetwork.JoinLobby();

        // Syncronitation of the scenes
        // PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        // This works for a non-multiplayer scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Joined to the lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Some problems in the connection to server");
    }
}
