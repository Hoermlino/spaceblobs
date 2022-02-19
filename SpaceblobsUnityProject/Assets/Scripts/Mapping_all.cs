using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeClass;
using MLAPI;


public class Mapping_all : MonoBehaviour
{
    public Material[] materials;
    Renderer HexModelRenderer;
    public GameObject HexPreFab;
    public GameObject HexPreFab2;
    [HideInInspector]
    public GameObject GO_Hex;
    //public Node[,] nodes;
    public Node[] nodes;
    

    // Number of Tiles in each direction
    // width is self explainatory and heigth goes left up
    int width = 9;
    int height = 9;
    [HideInInspector]  public float XoffsetForWitdh = 1.8f;
    float XoffsetForHeight = -0.9f; //we need this because the hex grid axies are not 90° to each other
    [HideInInspector]  public float ZoffsetForHeight = 1.58f;
    int Rowslider = 0;
    int Rowstrecher = 0;

    // Start is called before the first frame update
    void Start()
    {

        //nodes = new Node[width + 1, height];
        nodes = new Node[76];
        int TileNr = 0;
        for (int h = 0; h < height; h+=1)
        {
            // We have to change the length of some rows and additionally shift them sideways because of the tilted z axis
            if (h % 2 != 0)//check if row is even
            {
                Rowstrecher++;
                Rowslider++;
            }
            else
            {
                Rowstrecher--;
            }


            // Here we construct the Hexes
            
            for (int w = 0; w < width + Rowstrecher; w += 1)
            {
                
                if (h == 4f)
                {

                    GO_Hex = (GameObject)Instantiate(HexPreFab2, new Vector3((w - Rowslider) * XoffsetForWitdh - h * XoffsetForHeight, 0f, h * ZoffsetForHeight), Quaternion.identity);
                    
                }
                else
                {
                    GO_Hex = (GameObject)Instantiate(HexPreFab, new Vector3((w - Rowslider) * XoffsetForWitdh - h * XoffsetForHeight, 0f, h * ZoffsetForHeight), Quaternion.identity);
                    
                }
                // I dont know why this w + Rowslider-Rowstrecher-1 is correct. I try and errored it
                GO_Hex.name = "HexTile_" + h + "_" + (w + Rowslider-Rowstrecher-1);
                
                
                Vector3 worldPosition = new Vector3(x: (w - Rowslider) * XoffsetForWitdh - h * XoffsetForHeight, y: 0.5f, z: h * ZoffsetForHeight);

                //nodes[w, h] = new Node(isPlaceable: true, worldPosition, GO_Hex.transform);
                nodes[TileNr] = new Node(isPlaceable: true, isSpawnable: false, isMoveable: false, isEnemy: false, worldPosition, GO_Hex); //
                TileNr++;

                //Saving of the HexGrid Coordinates in the other script's (called Hex) variables HexGridCoordinateX and HexGridCoordinateZ
                GO_Hex.GetComponent<Hex>().HexGridCoordinateX = (w + Rowslider - Rowstrecher - 1);
                GO_Hex.GetComponent<Hex>().HexGridCoordinateZ = h;


                GO_Hex.transform.SetParent(this.transform);

                GameObject GO_Child = GO_Hex.transform.GetChild(0).gameObject;

                HexModelRenderer = GO_Child.GetComponent<Renderer>();
                HexModelRenderer.enabled = true;

                if (h != 3f && h != 4f && h != 5f)
                {
                    //change the material to grass
                    HexModelRenderer.sharedMaterial = materials[0];
                }

                else
                {
                    //change the material to mountain
                    HexModelRenderer.sharedMaterial = materials[1];
                }
            }
            // we check in wich line we are and change the material


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


namespace NodeClass
{
public class Node
{
    public bool isPlaceable;
    public bool isSpawnable;
    public bool isMoveable;
    public bool isEnemy;
    public Vector3 cellPosition;
    public GameObject HexOfThisNodeGO;
    public NetworkObject FigureOnThisNode;



    public Node(bool isPlaceable, bool isSpawnable, bool isMoveable, bool isEnemy, Vector3 cellPosition, GameObject HexOfTheNodeGO) 
        {
        this.isPlaceable = isPlaceable;
        this.isSpawnable = isSpawnable;
        this.isMoveable = isMoveable;
        this.isEnemy = isEnemy;
        this.cellPosition = cellPosition;
        this.HexOfThisNodeGO = HexOfTheNodeGO;
    }

    public static void fctSetAdjacentNodes(bool bolChangeSpawnable, bool bolIsSpawnable, bool bolChangeMoveable, bool bolIsMoveable, int intNumberOfHexes, bool bolChangeColor, Color colHexColor, Vector3 vecNodepostion)
    {
        // In this loop we search for the node with the nearest position to the given Nodeposition
        GameObject HexMap = GameObject.FindWithTag("Map_HexStructureTag");
        Node[] nodes = HexMap.GetComponent<Mapping_all>().nodes;
        Node[] adjacentNodes;
        adjacentNodes = new Node[0];
        // Ich muss hier irgendwie die variable nodeSpawner vorbesetzen sonst gibt es einen Fehler. Ka warum aber Sie wird eh später überschrieben.
        Node nodeChange = nodes[0];
        float floHexDistance;
        floHexDistance = GameObject.Find("Map_HexStructure").GetComponent<Mapping_all>().XoffsetForWitdh;

        foreach (var node in nodes)
        {
            float distance = Vector3.Distance(node.cellPosition, vecNodepostion);


            if (distance < floHexDistance/2)
            {
                nodeChange = node;
                //Debug.Log(nodeSpawner);
            }

        }

        // In this Loop we search for all Nodes which are adjacent to the given Node by looking at the distance 
        // If the nodes (middlepoint of a Hex) are within a circle of litle bit less than 2 CNDs(Closest Node distance) then we get all nodes which have one Node distance to the given Node

        foreach (var node in nodes)
        {
            float distance = Vector3.Distance(node.cellPosition, nodeChange.cellPosition);
            int intNodeNr = 0;
            if ( distance < floHexDistance * (0.3f + intNumberOfHexes))
            {
                    //adjacentNodes[intNodeNr] = node;
                if (bolChangeSpawnable)
                {
                    node.isSpawnable = bolIsSpawnable;
                }
                if (bolChangeMoveable)
                {
                    node.isMoveable = bolIsMoveable; 
                }
                if (bolChangeColor)
                {
                    // here we also change the color of the hex the node is sitting on
                    //node.obj.gameObject.GetComponentInChildren<MeshRenderer>().material.color = colHexColor;
                    node.HexOfThisNodeGO.GetComponentInChildren<MeshRenderer>().material.color = colHexColor;
                }
                   
                        
                
                intNodeNr++;
            }
        }
    }

    public static void fctSetAllNodes(bool bolChangeIsMoveable, bool bolIsMoveable, bool bolChangeColorHex, Color colHexColor)
        {
            GameObject HexMap = GameObject.FindWithTag("Map_HexStructureTag");
            Node[] nodes = HexMap.GetComponent<Mapping_all>().nodes;
            foreach (var node in nodes)
            {
                if (bolChangeColorHex)
                {
                    // here we also change the color of the hex the node is sitting on
                    //node.obj.gameObject.GetComponentInChildren<MeshRenderer>().material.color = colHexColor;
                    node.HexOfThisNodeGO.GetComponentInChildren<MeshRenderer>().material.color = colHexColor;
                }
                if (bolChangeIsMoveable)
                {
                    node.isMoveable = bolIsMoveable;
                }

            }
        }

}
}



