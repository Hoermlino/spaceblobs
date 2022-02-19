using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{

    public string UnitName;
    public int UnitHealth;
    public int UnitCurrentHealth;
    public string UnitResourceCost;
    public string UnitAbilityDescription;
    public int UnitDamage;
    public int UnitRange;
    public int UnitMovementRange;
    public int UnitMovementRangeCurrent;
    public string UnitResourceProduction;
    //[HideInInspector] 
    public Vector3 vecHexpositionAtStartOfState;


    private void Update()
    {
        // Optimierungsbedarf. Die Funktion löst für zu viele Gameobjects aus und ich weiß nicht warum. Serious Computation Overhead !!!!!!!!!!!!!!!!!!!!!!!!!!!

        // here we check every frame if we are in the Start of Actionturn phase and then we save the position of the figure 
        if(GameObject.Find("RoundLogic").GetComponent<RoundLogic>().state == RoundState.STARTACTIONTURN)
        {
            vecHexpositionAtStartOfState = gameObject.transform.position;
            UnitMovementRangeCurrent = UnitMovementRange;

        }
    }

    public void fctCalculateMovementRangeCurrent(Vector3 vecFigurePostion) 
    {
        float Hexwidth;
        int RangeFromOriginalHex;
        Hexwidth = GameObject.Find("Map_HexStructure").GetComponent<Mapping_all>().ZoffsetForHeight;
        // Here we calculate the distance between the Figure and its original Hex position, then add half a Hex width and divide by the Hexwidth to get the number of hexes we went from the original hex
        RangeFromOriginalHex = (int) ((Vector3.Distance(vecHexpositionAtStartOfState, vecFigurePostion) + (Hexwidth / 3)) / Hexwidth);

        //Debug.Log("Range from Original Hex: " + RangeFromOriginalHex);

        UnitMovementRangeCurrent = UnitMovementRange - RangeFromOriginalHex;
        //Debug.Log("MovementRangeCurrent: " + UnitMovementRangeCurrent);
    }

}
