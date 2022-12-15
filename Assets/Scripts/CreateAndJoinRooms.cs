using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text, new RoomOptions() { MaxPlayers = 2 });
        Debug.Log("Room " + createInput.text + " created");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("The room was not created, trying again.");
        CreateRoom();
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        Debug.Log("Joining to the " + joinInput.text + " room");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("There a problem joining to the " + joinInput.text + " room");
    }
    public override void OnJoinedRoom()
    {
        // This is for multiplayer scene
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("Starting the game");
    }
}
