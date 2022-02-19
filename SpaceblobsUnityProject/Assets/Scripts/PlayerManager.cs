using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TempUnitPositionInfoClass;
using ThisPlayersFigureCollection;
using System;
using JsonHelperClass;

public class PlayerManager : NetworkBehaviour
{
    // here we want to find out if we are the first player connected
    public NetworkVariableBool bolFirstPlayer = new NetworkVariableBool(false);
    public NetworkVariableBool bolPlayerReady = new NetworkVariableBool(false);

    private NetworkObject SpawnObjectNOB;

    // here we need to have a whole list of all spawnable figures (prefabs)
    
    public NetworkObject SpawnFigureNr1NOB;
    public NetworkObject SpawnFigureNr2NOB;
    private NetworkObject SpawnFigureNr3NOB;
    private NetworkObject SpawnFigureNr4NOB;
    private NetworkObject SpawnFigureNr5NOB;
    private NetworkObject SpawnFigureNr6NOB;
    private NetworkObject SpawnFigureNr7NOB;
    private NetworkObject SpawnFigureNr8NOB;
    private NetworkObject SpawnFigureNr9NOB;
    private NetworkObject SpawnFigureNr10NOB;
    private NetworkObject SpawnFigureNr11NOB;

    public TempUnitPositionInfo[] TempUnitPositionArray;


    public FigureCollection[] ThisFigureCollection;

    public static string FigureCollectionJson;


    // Start is called before the first frame update
    void Start()
    {

        // if we are not the owner of this Player we do nothing
        if (IsOwner)
        {
            // Test
            ThisFigureCollection = new FigureCollection[999];
            fctTestCreatePlayerCollection();

            string json = JsonHelper.ToJson(ThisFigureCollection);
            FigureCollectionJson = json;
            Debug.Log(json);

            

            FigureCollection[] CollectionFromJson = JsonHelper.FromJson<FigureCollection>(json);

            Debug.Log(CollectionFromJson[1].UnitName);


            // we create the array for temporary Unit position infos
            TempUnitPositionArray = new TempUnitPositionInfo[72];


            // we check what the ID of the First connected client is (if we are the first client the value is 666)
            ulong FirstClientID = GameObject.Find("ServerVariableManager").GetComponent<ServerVariableManager>().ulngFirstClientId.Value;
            // if we are the fist client connected we change the FirstClientId to our own ID

            if (FirstClientID == 666)
            {
                GameObject.Find("ServerVariableManager").GetComponent<ServerVariableManager>().fctChangeFirstClientId(OwnerClientId);
            }

            // are we the first Player connected?
            if (GameObject.Find("ServerVariableManager").GetComponent<ServerVariableManager>().ulngFirstClientId.Value == OwnerClientId)
            {
                fctChangeBolFirstClientServerRpc(true);
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void fctChangeBolFirstClientServerRpc(bool bolFirstClient)
    {
        bolFirstPlayer.Value = bolFirstClient;
    }



    [ServerRpc]
    public void fctCreateFigureServerRpc(int NrOfFigure, Vector3 vecSpawnPosition, bool bolFirstPlayer)
    {
        
        NetworkObject FigureToSpawn = fctTranslateFigureNrToNetworkOB(NrOfFigure);
        SpawnObjectNOB = Instantiate(FigureToSpawn.GetComponent<NetworkObject>(), vecSpawnPosition, Quaternion.identity);
        SpawnObjectNOB.SpawnWithOwnership(OwnerClientId);
        //GameObject.Find("NodeManager").GetComponent<NodeManager>().fctChangeNodeInfo(vecSpawnPosition, false);
        SpawnObjectNOB.GetComponentInChildren<DragDrop_FollowMouse>().isOnGrid = false;
    }

    [ServerRpc]
    public void fctDespawnFigureServerRpc()
    {
        //FigureToDespawn.Despawn();
    }


    [ServerRpc]
    public void fctSetPlayerReadyServerRpc()
    {
        bolPlayerReady.Value = true;
    }

    public NetworkObject fctTranslateFigureNrToNetworkOB(int FigureNr)
    {
        switch (FigureNr)
        {
            case 1: return SpawnFigureNr1NOB;
            case 2: return SpawnFigureNr2NOB;
        }
        return null;
    }
    // this funcition manages the array in which we save the data of changed Figures. If a figure is spawned the start and end vector are the same. If it was moved during action turn they are different.
    public void fctWriteTempUnitInfo(NetworkObject Unit, Vector3 StartOfTurnPosition, Vector3 EndOfTurnPosition, bool IsSpawned)
    {
        bool IsAlreadyInArray = false;
        int LengthOfArray = 0;
        
        // we check the TempUnitPositionArray if the unit is already inside the array then we overwrite the End-position
        foreach (var UnitandPosition in TempUnitPositionArray)
        {
            if(UnitandPosition != null)
            {
                if (UnitandPosition.UnitNOB == Unit)
                {
                    UnitandPosition.UnitPositionEnd = EndOfTurnPosition;
                    IsAlreadyInArray = true;
                }
                LengthOfArray += 1;
            }
        }
        // if it is empty or the moved unit is not already in the array -> then we write it in (append)
        if (!IsAlreadyInArray)
        {
            TempUnitPositionArray[LengthOfArray] = new TempUnitPositionInfo(Unit, StartOfTurnPosition, EndOfTurnPosition, IsSpawned);
        }
       
    }

    public void fctDeleteTempUnitInfo()
    {

    }

    public void fctCreateNewFigureForCollection(int ModelID, string UnitName, int UnitHealth)
    {
        int NrOfFiguresInCollection = 0;
        foreach ( var Figure in ThisFigureCollection)
        {
            if (Figure != null)
            {
                NrOfFiguresInCollection += 1;
            }
        }
        ThisFigureCollection[NrOfFiguresInCollection] = new FigureCollection(NrOfFiguresInCollection, ModelID, UnitName, UnitHealth);
        
    }

    public void fctTestCreatePlayerCollection()
    {
        fctCreateNewFigureForCollection(1, "The Boss", 11);
        fctCreateNewFigureForCollection(2, "Mausemann", 12);
        fctCreateNewFigureForCollection(3, "Katzenmann", 13);
    }

    public void fctSendJson()
    {
        Debug.Log(FigureCollectionJson);
        GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>().fctSendJsonDataToPlayfab(FigureCollectionJson);
    }

    public void fctGetInventory()
    {
        GameObject.Find("PlayFabManager").GetComponent<PlayFabManager>().fctRequestInventory();
    }

}


namespace TempUnitPositionInfoClass
{
    public class TempUnitPositionInfo
    {
        public NetworkObject UnitNOB;
        public Vector3 UnitPositionStart;
        public Vector3 UnitPositionEnd;
        public bool IsSpawnedThisRound;


        public TempUnitPositionInfo(NetworkObject UnitNOB, Vector3 UnitPosStart, Vector3 UnitPosEnd, bool bolIsSpawned)
        {
            this.UnitNOB = UnitNOB;
            this.UnitPositionStart = UnitPosStart;
            this.UnitPositionEnd = UnitPosEnd;
            this.IsSpawnedThisRound = bolIsSpawned;
        }
    }
}


namespace ThisPlayersFigureCollection
{
    [Serializable]
    public class FigureCollection
    {
        // The FigureNr is an ascending Nr of the players Figures
        public int FigureNr;
        // You can have multiple copys of the same Figure and so have multiple rows with the same ModelID
        public int ModelID;
        public string UnitName;
        public int UnitHealth;
        

        public FigureCollection(int FigureNr, int ModelID, string UnitName, int UnitHealth)
        {
            this.FigureNr = FigureNr;
            this.ModelID = ModelID;
            this.UnitName = UnitName;
            this.UnitHealth = UnitHealth;
        }
    }
}

namespace JsonHelperClass
{
    public class JsonHelper
    {

        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return UnityEngine.JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}


