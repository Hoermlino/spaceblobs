using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.MultiplayerModels;

public class MatchMaker : MonoBehaviour
{
    private string strTicketID;

    [SerializeField] private GameObject StartQueueGO;
    [SerializeField] private GameObject StopQueueGO;
    [SerializeField] private TMP_Text QueueStatusText;

    private const string strQueueName = "RankedQueue";
    private Coroutine cooPollTicketCoroutine;

    public void fctStartMatchMaking()
    {
        StartQueueGO.SetActive(false);
        QueueStatusText.text = "Queue Ticket created";
        QueueStatusText.gameObject.SetActive(true);
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
            new CreateMatchmakingTicketRequest
            {
                Creator = new MatchmakingPlayer
                {
                    Entity = new EntityKey
                    {
                        Id = PlayFabManager.strEntityId,
                        Type = "title_player_account"
                    },
                    Attributes = new MatchmakingPlayerAttributes
                    {
                        DataObject = new { }
                    }
                },
                GiveUpAfterSeconds = 600,

                QueueName = strQueueName
            },
            fctOnMatchmakingTicketCreated,
            fctOnMatchmakingError
        );
    }


    public void fctLeaveQueue()
    {
        StopQueueGO.SetActive(false);
        QueueStatusText.gameObject.SetActive(false);

        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
            new CancelMatchmakingTicketRequest
            {
                QueueName = strQueueName,
                TicketId = strTicketID
            },
            OnTicketCanceled,
            fctOnMatchmakingError
        );
    }
    private void OnTicketCanceled(CancelMatchmakingTicketResult result)
    {
        StartQueueGO.SetActive(true);
    }

    private void fctOnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
    {
        strTicketID = result.TicketId;
        cooPollTicketCoroutine = StartCoroutine(cooPollTicket());

        StopQueueGO.SetActive(true);
        QueueStatusText.text = "In Queue since: ";
    }

    private void fctOnMatchmakingError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private IEnumerator cooPollTicket()
    {
        while (true)
        {
            PlayFabMultiplayerAPI.GetMatchmakingTicket(
                new GetMatchmakingTicketRequest
                {
                    TicketId = strTicketID,
                    QueueName = strQueueName
                },
                fctOnGetMatchmakingTicket,
                fctOnMatchmakingError
            );
            yield return new WaitForSeconds(6f);
        }
    }

    private void fctOnGetMatchmakingTicket(GetMatchmakingTicketResult result)
    {
        QueueStatusText.text = $"Status: {result.Status}";

        switch (result.Status)
        {
            case "Matched":
                StopCoroutine(cooPollTicketCoroutine);
                fctStartMatch(result.MatchId);
                // here we request the ip adress
                //Debug.Log(result.)
                break;
            case "Canceled":
                StopCoroutine(cooPollTicketCoroutine);
                StopQueueGO.SetActive(false);
                QueueStatusText.gameObject.SetActive(false);
                StartQueueGO.SetActive(true);
                break;
        }
    }

    private void fctStartMatch(string matchId)
    {
        QueueStatusText.text = $"Starting Match";

        PlayFabMultiplayerAPI.GetMatch(
            new GetMatchRequest
            {
                MatchId = matchId,
                QueueName = strQueueName
            },
            fctOnGetMatch,
            fctOnMatchmakingError
        );
    }

    private void fctOnGetMatch(GetMatchResult result)
    {
        QueueStatusText.text = $"{result.Members[0].Entity.Id} vs {result.Members[1].Entity.Id}";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
