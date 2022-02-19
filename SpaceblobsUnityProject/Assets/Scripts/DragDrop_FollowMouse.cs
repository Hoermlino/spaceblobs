using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;

public class DragDrop_FollowMouse : NetworkBehaviour
{
    private DragDrop DragDrop;
    public bool isOnGrid = false;


    // Start is called before the first frame update
    void Start()
    {
        DragDrop = FindObjectOfType<DragDrop>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            GameObject MouseManager = GameObject.FindWithTag("MouseManagerTag");
            Vector3 MousePosXZPlane = MouseManager.GetComponent<MouseManager>().fctGetMousePositionOnXZPlane();


            if (!isOnGrid)
            {
                transform.position = MousePosXZPlane + new Vector3(0, 0.5f, 0);

            }
        }


    }

}