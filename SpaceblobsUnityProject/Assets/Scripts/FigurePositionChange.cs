using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class FigurePositionChange : NetworkBehaviour
{
    private float FigureXposition;
    private float FigureYposition;
    private float FigureZposition;

    public void fctChangeFigurePositionGlobally()
    {
        // only if you are a client and the owner of this figure you should call this function
        if (!IsOwner) { return; }
        if (!IsClient) { return; }

        // write all position info from the local Figure to the global variables
        FigureXposition = gameObject.transform.position.x;
        FigureYposition = gameObject.transform.position.y;
        FigureZposition = gameObject.transform.position.z;

        // call the server to change the Position of this figure on the other clients
        fctChangeFigurePositionGloballyServerRpc(FigureXposition, FigureYposition, FigureZposition);
    }



    [ServerRpc]
    public void fctChangeFigurePositionGloballyServerRpc(float FigurePosX, float FigurePosY, float FigurePosZ)
    {
        // the server calls all clients to change the position of the figure
        fctChangeFigurePositionGloballyClientRpc(FigurePosX, FigurePosY, FigurePosZ);
    }

    [ClientRpc]
    public void fctChangeFigurePositionGloballyClientRpc(float FigurePosX, float FigurePosY, float FigurePosZ)
    {
        //dont change the postition on the client which is the owner ( he already knows that the position has changed)
        if(IsOwner) { return; }
        //but change the position of the figure on all other clients
        gameObject.transform.position = new Vector3(FigurePosX, FigurePosY, FigurePosZ);
    }
}
