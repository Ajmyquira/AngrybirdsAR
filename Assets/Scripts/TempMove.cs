using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TempMove : MonoBehaviour
{
    float speed = 0.1f;
    PhotonView view; // Identifies an object across the network

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        // Sure that you can only move your player, and not the others
        if (view.IsMine)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
            }
        }
    }
}
