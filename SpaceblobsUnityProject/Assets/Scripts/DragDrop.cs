using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeClass;
using MLAPI;
using MLAPI.Connection;

public class DragDrop : MonoBehaviour
{
    public Transform onMousePrefab;
    public bool bolonMousePrefab;
    public GameObject BuildingOptionUI;


    public bool isOnGrid = false;
    public Vector3 smoothMousePosition;
    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //fctPositionOnGrid(onMousePrefab);

    }


    public void fctMoveOnGrid( NetworkObject OnMouseNOB, Vector3 vecPosWhenPickedUP)
    {


        if (Input.GetMouseButtonUp(0) && bolonMousePrefab) // && onMousePrefab != null
        {

            //onMousePrefab = 

            GameObject HexMap = GameObject.FindWithTag("Map_HexStructureTag");

            GameObject MouseManager = GameObject.FindWithTag("MouseManagerTag");

            Node[] nodes = HexMap.GetComponent<Mapping_all>().nodes;

            float tempLowestDistance = 100;

            Vector3 currentMousePosXZ = MouseManager.GetComponent<MouseManager>().fctGetMousePositionOnXZPlane();


            foreach (var node in nodes)
            {
                //floatDistance = Distance of MousepositionXZ to node.cellPosition 
                // sort and take the node.cellPosition with the smallest Distance

                float distance = Vector3.Distance(node.cellPosition, currentMousePosXZ);

                if (distance < tempLowestDistance)
                {
                    tempLowestDistance = distance;
                }
                //Debug.Log(distance);
            }
            

            
            // Die zweite foreach schleife ist nicht ganz sauber programmiert aber ich weiß aktuell nicht wie man auf den node auserhalb der schleife zugrift.
            foreach (var node in nodes)
            {
                float distance = Vector3.Distance(node.cellPosition, currentMousePosXZ);

                //float floFigureDistanceMoved = Vector3.Distance(OnMouseGO.GetComponent<UnitInfo>().vecHexpositionAtStartOfState, currentMousePosXZ);
                

                // The maximal allowed distance from the center is tricky. This solution is not very clean. We just take a circle but then we have to cut some areas of the hexes to make sure to not hit the outer hexes.
                // the radius of the circle is half the height plus half the width of the hex

                
                if (distance == tempLowestDistance && node.isPlaceable && node.isMoveable)
                {
                    
                    
                    node.isPlaceable = false;
                    OnMouseNOB.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = true;

                    //We have to tell the Figure on which node it is sitting
                    node.FigureOnThisNode = OnMouseNOB;
                    //OnMouseObject.GetComponentInChildren<DragDrop_FollowMouse>().IsOnThisNode = node;

                    OnMouseNOB.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
                    OnMouseNOB.transform.position = node.cellPosition + new Vector3(0, 1.5f, 0);

                    bolonMousePrefab = false;
                    //OnMouseObject = null;

                    // Calculate the CurrentMovementRange
                    //OnMouseGO.GetComponent<UnitInfo>().fctCalculateMovementRangeCurrent(OnMouseGO.transform.position);

                    
                }

                else if(distance == tempLowestDistance)
                {

                    // Activate the flag that the Object is on the Grid 
                    OnMouseNOB.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = true;
                    // Dectivate the opposite flag (maybe can be reduced to one flag but also maybe ther are more possibilities than only beeing on the mouse or grid)
                    GameObject.FindWithTag("FigurePlacementTag").GetComponentInChildren<DragDrop>().bolonMousePrefab = false;

                    //change the position of the Figure back to the root in this turn
                    OnMouseNOB.transform.position = vecPosWhenPickedUP;
                    OnMouseNOB.transform.GetChild(0).localPosition = new Vector3(0f, 0.5f, 0f);
                }
            }
            Node.fctSetAllNodes(true, false, true, Color.white);

        }


    }

    public void fctSpawnOnGrid(NetworkObject OnMouseObject)
    {

        
        if (Input.GetMouseButtonUp(0) && bolonMousePrefab) // && onMousePrefab != null
        {

            

            GameObject HexMap = GameObject.FindWithTag("Map_HexStructureTag");

            GameObject MouseManager = GameObject.FindWithTag("MouseManagerTag");

            Node[] nodes = HexMap.GetComponent<Mapping_all>().nodes;

            float tempLowestDistance = 100;

            Vector3 currentMousePosXZ = MouseManager.GetComponent<MouseManager>().fctGetMousePositionOnXZPlane();


            foreach (var node in nodes)
            {
                //floatDistance = Distance of MousepositionXZ to node.cellPosition 
                // sort and take the node.cellPosition with the smallest Distance
                
                float distance = Vector3.Distance(node.cellPosition, currentMousePosXZ);

                if (distance < tempLowestDistance)
                {
                    tempLowestDistance = distance;
                }
                //Debug.Log(distance);
            }
            // Die zweite foreach schleife ist nicht ganz sauber programmiert aber ich weiß aktuell nicht wie man auf den node auserhalb der schleife zugrift.
            foreach (var node in nodes)
            {
                float distance = Vector3.Distance(node.cellPosition, currentMousePosXZ);
                
                if (distance == tempLowestDistance && node.isPlaceable && node.isSpawnable)
                {
                    
                    node.isPlaceable = false;
                    OnMouseObject.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = true;

                    //We have to tell the Figure on which node it is sitting
                    node.FigureOnThisNode = OnMouseObject;
                    //OnMouseObject.GetComponentInChildren<DragDrop_FollowMouse>().IsOnThisNode = node;
                    

                    OnMouseObject.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
                    OnMouseObject.transform.position = node.cellPosition + new Vector3(0, 1.5f, 0);

                    // here we want to save the Object and the Position so that we can later send this Info to the server

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
                    player.fctWriteTempUnitInfo(OnMouseObject, node.cellPosition + new Vector3(0, 1.5f, 0), node.cellPosition + new Vector3(0, 1.5f, 0), true);




                    bolonMousePrefab = false;
                    //OnMouseObject = null;
                }
                // if we are not able to spawn the figure it shall be destroyed and the UI Splashart shall be recreated
                else if(distance == tempLowestDistance)
                {
                    // Dectivate the flag that the figure is sticking to the mouse
                    bolonMousePrefab = false;
                    // Despawn the Figure
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
                    // then send a ServerRpc from that player to create this figure
                    //player.fctDespawnFigureServerRpc(OnMouseObject);

                    // Activate the UI Spashart again
                    BuildingOptionUI.SetActive(true);
                }

            }
            // after the figure has been placed we change the color of all Hexes back to white
            Node.fctSetAllNodes(false, false, true, Color.white);
        }

        
    }
    
    public void OnMouseClickOnUI(Transform traBuildingOption)
    {
        // if there is not anything on the mouse already
        if (bolonMousePrefab == false)
        {
            // then we create a new Figure 
            //old --------------------
            //onMousePrefab = Instantiate(traBuildingOption, Input.mousePosition, Quaternion.identity);
            //onMousePrefab.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = false;
            // old -------------------
            // new ------------------- MLAPI
            // first find my own player
            // Get the local client's id
            
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
            // new ------------------- MLAPI
            bolonMousePrefab = true;
            BuildingOptionUI = traBuildingOption.gameObject;

            // then send a ServerRpc from that player to create this figure
            player.fctCreateFigureServerRpc(1, Input.mousePosition, true);
            


            

            //Set all Hexes color to gray (and later the Hexes which can be build on white) 
            Node.fctSetAllNodes(false, false, true, Color.gray);
            // set the Hex nodes around the castles to swanable and change the color of spawnable Hexes to white
            Node.fctSetAdjacentNodes(true, true, false, false, 1, true, Color.white, GameObject.Find("RoundLogic").GetComponent<RoundLogic>().SpawnHexP1.position);
        }
    }
    public void OnMouseClickOnUIdestroy(GameObject BuildingOptionUIGO)
    {
        // and we do not show the UI spashart of this Figure anymore
        BuildingOptionUIGO.SetActive(false);
        BuildingOptionUI = BuildingOptionUIGO;
    }

}
