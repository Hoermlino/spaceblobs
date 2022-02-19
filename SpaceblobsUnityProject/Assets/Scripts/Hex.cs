using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public int HexGridCoordinateX;
    public int HexGridCoordinateZ;
    public int intDistance;
    //Die Formel 6* intDistance muss noch überprüft werden. Wie viele Nachbarn hat man in Abhängigkeit von der Anzahl der Felder die man gehen darf?
    
    public GameObject CurrentNeighbor;

    private int HexGridCoordinateXNeighbor;
    private int HexGridCoordinateZNeighbor;
    private int intPositionInArray = 1;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Hex[] GetNeighbours()
    {
        Hex[] arrNeighbours = new Hex[100];

        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (!(x + z == 0))
                {
                    HexGridCoordinateZNeighbor = HexGridCoordinateZ + z;
                    HexGridCoordinateXNeighbor = HexGridCoordinateX + x;
                    CurrentNeighbor = GameObject.Find("HexTile_" + HexGridCoordinateZNeighbor + "_" + HexGridCoordinateXNeighbor);
                    Debug.Log("##############################" + CurrentNeighbor);

                    if (CurrentNeighbor != null)
                    {
                        arrNeighbours[intPositionInArray] = CurrentNeighbor.GetComponent<Hex>();
                    } 
                    


                    intPositionInArray++;

                }

            }
        }


        return arrNeighbours;
    }
}
