using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;

public class CameraPosition : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        
    }

    public void FctSetCameraPosition()
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        // Try to get the local client object
        // Return if unsuccessful

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient))
        {
            return;
        }
        // Try to get the TeamPlayer component from the player object
        // Return if unsuccessful
        if (!networkClient.PlayerObject.TryGetComponent<PlayerManager>(out var player))
        {
            return;
        }
        if (player.bolFirstPlayer.Value)
        {
            GameObject.FindWithTag("MainCamera").GetComponent<Transform>().position = new Vector3(6f, 111f, 48f);
            GameObject.FindWithTag("MainCamera").GetComponent<Transform>().eulerAngles = new Vector3(110f, 0f, 180f);

        }
        else
        {
            GameObject.FindWithTag("MainCamera").GetComponent<Transform>().position = new Vector3(6f, 111f, -35f);
            GameObject.FindWithTag("MainCamera").GetComponent<Transform>().eulerAngles = new Vector3(70f, 0f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
