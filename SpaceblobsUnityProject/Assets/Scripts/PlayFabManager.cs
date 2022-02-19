using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
//using PlayFab.ServerModels;
using TMPro;
using MLAPI;


public class PlayFabManager : MonoBehaviour
{
    // log in and create Account
    [SerializeField] private GameObject nextMenuAfterLoginGO = default;
    [SerializeField] private GameObject signInDisplayGO = default;
    [SerializeField] private GameObject CreateAccountDisplayGO = default;
    [SerializeField] private TMP_InputField usernameInputField = default;
    [SerializeField] private TMP_InputField passwordInputField = default;
    [SerializeField] private TMP_InputField usernameInputFieldCreate = default;
    [SerializeField] private TMP_InputField emailInputFieldCreate = default;
    [SerializeField] private TMP_InputField passwordInputFieldCreate = default;
    [SerializeField] private TMP_Text SignInStatusText = default;

    // get and store Data on Playfab

    [SerializeField] private TMP_Text ReceivedDataText = default;


    public static string SessionTicket;
    public static string strEntityId;

    public string FigureID;

    public void fctCreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = usernameInputFieldCreate.text,
            Email = emailInputFieldCreate.text,
            Password = passwordInputFieldCreate.text
        }, result =>
        {
            SessionTicket = result.SessionTicket;
            strEntityId = result.EntityToken.Entity.Id;
            CreateAccountDisplayGO.SetActive(false);
            nextMenuAfterLoginGO.SetActive(true);
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
        });
    }

  

    public void fctSignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = usernameInputField.text,
            Password = passwordInputField.text
        }, result =>
        {
            SessionTicket = result.SessionTicket;
            strEntityId = result.EntityToken.Entity.Id;
            
            signInDisplayGO.SetActive(false);
            nextMenuAfterLoginGO.SetActive(true);
        }, error =>
        {
            Debug.LogError(error.GenerateErrorReport());
            SignInStatusText.text = "Wrong username and password combination";
        });
    }
    /*
    public void testfct(string PlayFabID)
    {
        var request = new UpdateUserInternalDataRequest
        {
            PlayFabId = PlayFabID,
            Data = new Dictionary<string, string>
            {
                { "AlreadyGotFreeFigures", "true" }
            }
        };
        //PlayFabServerAPI.UpdateUserInternalData(request, OnValueChanged, OnError);
    }

    
    public void OnValueChanged(UpdateUserInternalDataRequest result)
    {
        Debug.Log("AlreadyGotFreeFigures set to true");
    }
    */

    public void fctSendDataToPlayfab()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"FigureNr", "1" },
                {"Health", "10" }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }
    public void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successful user data send !");
    }
    public void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void fctGetDataFromPlayfab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    public void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log("Received user data!");
        if (result.Data != null && result.Data.ContainsKey("FigureNr") && result.Data.ContainsKey("Health"))
        {
            ReceivedDataText.text = result.Data["FigureNr"].Value + result.Data["Health"].Value;
        }
        else
        {
            Debug.Log("Player data not complete");
        }
    }

    public void fctSendJsonDataToPlayfab(string json)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Figure Collection", json }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void fctRequestInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnInventroyReceived, OnError);
    }

    public void OnInventroyReceived(GetUserInventoryResult result)
    {
        fctRequestCatalog();
        Debug.Log(result.Inventory);
        FigureID = result.Inventory[1].ItemId;
    }

    public void fctRequestCatalog()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnCatalogReceived, OnError);
    }

    public void OnCatalogReceived(GetCatalogItemsResult result)
    {
        foreach ( var Item in result.Catalog)
        {
            if ( Item.ItemId == FigureID)
            {
                Debug.Log(Item.CustomData);
            }
            
        }
    }
}

