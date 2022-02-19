using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class ServerVariableManager : NetworkBehaviour
{


    public NetworkVariableULong ulngFirstClientId = new NetworkVariableULong(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, 666);



    public void fctChangeFirstClientId(ulong ClientID)
    {
        ulngFirstClientId.Value = ClientID;
    }

}
