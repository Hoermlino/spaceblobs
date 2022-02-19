using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeClass;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;


public enum RoundState { STARTECOTURN, ECOTURN, STARTACTIONTURN, ACTIONTURN, WON, LOST }
public class RoundLogic : NetworkBehaviour
{
    public GameObject Player1Castle;
    public GameObject Player2Castle;

    public GameObject BuildingOption1;
    public GameObject BuildingOption2;

    public Transform SpawnHexP1;
    public Transform SpawnHexP2;

    public GameObject TimeBar;
    public GameObject ResourceInfo;

    public Transform CanvasTra;
    public RoundTimer SliderTimer;


    public TextMeshProUGUI UnitNameUI;
    public TextMeshProUGUI UnitDamageUI;
    public Slider UnitHealthUI;
    public TextMeshProUGUI UnitRangeUI;
    public TextMeshProUGUI UnitMovementUI;
    public TextMeshProUGUI UnitAbilityDescrUI;
    public TextMeshProUGUI UnitResourceProdUI;
    public TextMeshProUGUI UnitResourceCostUI;

    


    UnitInfo Player1CastleInfo;
    UnitInfo Player2CastleInfo;

    public RoundState state;

    private void Start()
    {
        

        // initially we dont want to display the building options so we have to find them and dont show them (the building options are much more than 8, they are all available figures, probably times 4 because you can play more of the same in one Deck)
        BuildingOption1 = GameObject.Find("BuildingOption1");
        BuildingOption2 = GameObject.Find("BuildingOption2");
        BuildingOption1.SetActive(false);
        BuildingOption2.SetActive(false);
    }



    // Start is called before the first frame update
    public void fctStart()
    {
        Debug.Log("client?");

        state = RoundState.STARTECOTURN;
        StartCoroutine(SetupCastles());
        // we want to set the camera position according to the Player Nr
        GameObject.FindWithTag("MainCamera").GetComponent<CameraPosition>().FctSetCameraPosition();
    }

    IEnumerator SetupCastles()
    {
        // create the castles itself and the info
        GameObject Player1CastleGO = Instantiate(Player1Castle, SpawnHexP1);
        Player1CastleInfo = Player1CastleGO.GetComponent<UnitInfo>();

        GameObject Player2CastleGO = Instantiate(Player2Castle, SpawnHexP2);
        Player2CastleInfo = Player2CastleGO.GetComponent<UnitInfo>();

        UnitNameUI.text = Player2CastleInfo.UnitName;
        UnitDamageUI.text = Player2CastleInfo.UnitDamage.ToString();
        UnitHealthUI.maxValue = Player2CastleInfo.UnitHealth;
        UnitHealthUI.value = Player2CastleInfo.UnitCurrentHealth;
        UnitRangeUI.text = Player2CastleInfo.UnitRange.ToString();
        UnitMovementUI.text = Player2CastleInfo.UnitMovementRange.ToString();
        UnitAbilityDescrUI.text = Player2CastleInfo.UnitAbilityDescription;
        UnitResourceProdUI.text = Player2CastleInfo.UnitResourceProduction;
        UnitResourceCostUI.text = "Cost: "+ Player2CastleInfo.UnitResourceCost;


        //show the Resource Info
        GameObject ResourceInfoGO = Instantiate(ResourceInfo, CanvasTra);




        yield return new WaitForSeconds(2f);
        state = RoundState.ECOTURN;
        fctEcoturnP1();
    }   

    void fctEcoturnP1()
    {
        // check if we are in the correct state of the game
        if (state != RoundState.ECOTURN)
        {
            return;
        }
        // crate a slider which shows the remaining time in this phase
        SliderTimer.TotalTime = 30f;
        GameObject TimeBarGO = Instantiate(TimeBar, CanvasTra);
        
       

        StartCoroutine(cooBuyUnits(SliderTimer.TotalTime));
    }


    IEnumerator cooBuyUnits(float floTime)
    {
        // create the randomly generated Buy Options and display them as long as the EcoturnP1 lasts


        // we take the diffent splash arts of the units and put them on one of the 8 possible positions for them, then we set them active (visible)

        GameObject PositionBuildingOption3 = GameObject.Find("/PositionDummys/PositionBuildingOption3");
        //Debug.Log(PositionBuildingOption3.GetComponent<RectTransform>().anchoredPosition);
        BuildingOption1.GetComponent<RectTransform>().anchoredPosition = PositionBuildingOption3.GetComponent<RectTransform>().anchoredPosition;
        BuildingOption1.SetActive(true);
        BuildingOption2.SetActive(true);

        // ***


        // everything after yield return will be executed after the corotine has finished
        yield return new WaitForSeconds(floTime);
        // at the end of the ecoTurn we want to send the position Info of our figures to the server


        state = RoundState.STARTACTIONTURN;
        StartCoroutine(cooStartActionTurn());
    }

    IEnumerator cooStartActionTurn()
    {
        yield return new WaitForSeconds(1f);
        state = RoundState.ACTIONTURN;
        fctActionturnP1();
    }




    void fctActionturnP1()
    {
        if (state != RoundState.ACTIONTURN)
        {
            return;
        }
        BuildingOption1.SetActive(false);
        BuildingOption2.SetActive(false);
    }

}


