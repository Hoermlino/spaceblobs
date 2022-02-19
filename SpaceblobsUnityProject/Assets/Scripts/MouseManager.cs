using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using NodeClass;
using MLAPI;
using MLAPI.Connection;
public class MouseManager : MonoBehaviour
{
    UnitInfo UnitInfo;
    

    public TextMeshProUGUI UnitNameUI;
    public TextMeshProUGUI UnitDamageUI;
    public Slider UnitHealthUI;
    public TextMeshProUGUI UnitRangeUI;
    public TextMeshProUGUI UnitMovementUI;
    public TextMeshProUGUI UnitAbilityDescrUI;
    public TextMeshProUGUI UnitResourceProdUI;
    public TextMeshProUGUI UnitResourceCostUI;
    [HideInInspector] public Vector3 FigurePositionWhenPickedUp;
    [HideInInspector] public Vector3 NodePositionWhenPickedUp;

    Vector2 MousePosScreen;

    private MeshRenderer prevRenderer;
    public GameObject RayHitObjectParent;
    public Vector3 MousePosXZPlane;
    private Plane plane;
    Vector3 MousePosSnapToRaster;

    Vector3 NodePosition;




    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsOwner) { return; }
        // Is the mouse over a unity UI Element? We can do the same check for the main menu
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // it is, so lets not do custum stuff and return
            return;
        }


        fctGetNextObjectHit();
    }

    void fctMouseOverHex(GameObject HexTheMouseIsOver)
    {
        // do we have a unit selected, because that would change what we do on click


        // check if we click the left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {
            if (prevRenderer != null)
            {
                prevRenderer.material.color = Color.white;
            }


            MeshRenderer MR = HexTheMouseIsOver.GetComponentInChildren<MeshRenderer>();
            prevRenderer = MR;
            //change the material or color so that one knows which tile is selected
            MR.material.color = Color.gray;



            for (int i = 0; i < 99; i++)
            {
                //Debug.Log(RayHitObjectParent.GetComponent<Hex>().GetNeighbours(1)[i]);
                //Hex[] Neighbours = RayHitObjectParent.GetComponent<Hex>().GetNeighbours();
            }

        }
    }

    void fctMouseOverFigure(NetworkObject FigureTheMouseIsOver)
    {
        // check if bolonMousePrefab is false. Only if nothing sticks to the mouse we select a new Figure
        bool bolMousePrefab = GameObject.FindWithTag("FigurePlacementTag").GetComponent<DragDrop>().onMousePrefab;
        

        // we only want to be able to move figures in the ActionState
        if (Input.GetMouseButtonDown(0) && (GameObject.Find("RoundLogic").GetComponent<RoundLogic>().state == RoundState.ACTIONTURN))
        {



            // now we need to set the adjacent nodes of the node ( where the figure was standing at the beginning of the ActionTurn) to IsMoveable = false again
            //


            // Deactivate the flag that the Object is on the Grid -> it is on the tip of the mouse
            FigureTheMouseIsOver.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = false;
            // Activate the opposite flag (maybe can be reduced to one flag but also maybe ther are more possibilities than only beeing on the mouse or grid)
            GameObject.FindWithTag("FigurePlacementTag").GetComponentInChildren<DragDrop>().bolonMousePrefab = true;
            // Activate the flag on the Node so that on this node figures can be placed again

            // We have to get the node the figure is sitting on and set the nodes variable 'isplaceable' to true again
            //FigureTheMouseIsOver.GetComponentInChildren<DragDrop_FollowMouse>().IsOnThisNode
            GameObject HexMap = GameObject.FindWithTag("Map_HexStructureTag");
            Node[] nodes = HexMap.GetComponent<Mapping_all>().nodes;
            foreach (var node in nodes)
            {
                
                // This code checks all nodes for the gameObject the mouse is currently over and if it finds the node which holds the figure, the node is placeable again
                if (FigureTheMouseIsOver == node.FigureOnThisNode)
                {
                    
                    node.isPlaceable = true;
                    node.FigureOnThisNode = null;
                    NodePositionWhenPickedUp = node.cellPosition;
                    Debug.Log(node.cellPosition);
                    
                }
            }
            // here we set the color of all hexes to grey and the color of all hexes which can be moved to to white
            Node.fctSetAllNodes(false, false, true, Color.gray);
            // we save the position of the Figure the moment we pick it up
            FigurePositionWhenPickedUp = FigureTheMouseIsOver.transform.position;

            // we need to set adjacent nodes to IsMoveable = true to be able to move there 
            int intUnitMovementRange = FigureTheMouseIsOver.GetComponent<UnitInfo>().UnitMovementRange;
            Vector3 vecNodePositionAtStartOfState = FigureTheMouseIsOver.GetComponent<UnitInfo>().vecHexpositionAtStartOfState;
            // change the y value to 0.5 because in this plane are the nodes
            vecNodePositionAtStartOfState.y = 0.5f;
            Node.fctSetAdjacentNodes(false, false, true, true, intUnitMovementRange, true, Color.white, vecNodePositionAtStartOfState);
            Debug.Log(intUnitMovementRange);


        }
        if (Input.GetMouseButtonUp(0))
        {
            // we need to distinguish between the spawning of a Figure and the movement after spawning (diffenet States ECO and Action)
            if((GameObject.Find("RoundLogic").GetComponent<RoundLogic>().state == RoundState.ECOTURN))
            {
                GameObject.FindWithTag("FigurePlacementTag").GetComponentInChildren<DragDrop>().fctSpawnOnGrid(FigureTheMouseIsOver);
            }
            if((GameObject.Find("RoundLogic").GetComponent<RoundLogic>().state == RoundState.ACTIONTURN))
            {
                GameObject.FindWithTag("FigurePlacementTag").GetComponentInChildren<DragDrop>().fctMoveOnGrid(FigureTheMouseIsOver, FigurePositionWhenPickedUp);
            }

        }
    }




    public NetworkObject fctGetNextObjectHit()
    {
        MousePosScreen = Input.mousePosition;
        
        Ray rayFromCamToMouse = Camera.main.ScreenPointToRay(MousePosScreen);
        RaycastHit hitinfo;
        //if (IsClient)
        //{

        //cast a ray from the camera over the Mouse to infinity and check if it hits something
        if (Physics.Raycast(rayFromCamToMouse, out hitinfo))
        {
            GameObject RayHitObject = hitinfo.collider.transform.gameObject;
            RayHitObjectParent = hitinfo.collider.transform.parent.gameObject;

                

            //Debug.Log("Raycast hit something "+ hitinfo.collider.transform.parent.name);

            // What kind of Gameobject are we over?
            if (RayHitObjectParent.GetComponent<Hex>() != null)
            {
                //We are over a Object of class Hex
                fctMouseOverHex(RayHitObjectParent);
            }
            if (RayHitObject.GetComponent<DragDrop_FollowMouse>() != null)
            {

                fctMouseOverFigure(RayHitObjectParent.GetComponent<NetworkObject>());

                    


                // here we want to display the clicked object´s information in the Infowindow
                UnitInfo = RayHitObjectParent.GetComponent<NetworkObject>().GetComponent<UnitInfo>();

                UnitNameUI.text = UnitInfo.UnitName;
                UnitDamageUI.text = UnitInfo.UnitDamage.ToString();
                UnitHealthUI.maxValue = UnitInfo.UnitHealth;
                UnitHealthUI.value = UnitInfo.UnitCurrentHealth;
                UnitRangeUI.text = UnitInfo.UnitRange.ToString();
                UnitMovementUI.text = UnitInfo.UnitMovementRange.ToString();
                UnitAbilityDescrUI.text = UnitInfo.UnitAbilityDescription;
                UnitResourceProdUI.text = UnitInfo.UnitResourceProduction;
                UnitResourceCostUI.text = "Cost: " + UnitInfo.UnitResourceCost;

            }


            //}

            
        }

        return RayHitObjectParent.GetComponent<NetworkObject>();
    }
    public Vector3 fctGetMousePositionOnXZPlane()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane = new Plane(inNormal: Vector3.up, inPoint: new Vector3(0, 2f, 0));
        if (plane.Raycast(ray, out var enter))
        {
            MousePosXZPlane = ray.GetPoint(enter);
            MousePosSnapToRaster = MousePosXZPlane;
            MousePosSnapToRaster.y = 0;
            //Die nächste Zeile muss noch auf das Hex grid angepasst werden
            MousePosSnapToRaster = Vector3Int.RoundToInt(Input.mousePosition);
        }
        return MousePosXZPlane;
    }


}

