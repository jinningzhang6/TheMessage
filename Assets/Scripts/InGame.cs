using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGame : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject[] playerUIs;
    private GameObject[] newPlayerUIS;
    public Text playerTurnUI;

    private int playersCount;
    private int turnCount;
    private Hashtable playerPositions;
    private Hashtable playerSequences;
    private Deck Deck;
    private List<Card> serverDeck;

    private bool roundZero=true;

    public const int TurnStartEventCode = 1;
    public const int DrawCardEventCode = 2;
    public const int SendCardEventCode = 3;
    //public const int ShipoEventCode = 4;

    private void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;//3;// 
        turnCount = 0;
        populateCurrentPlayerUIs(playersCount);
        playerPositions = new Hashtable();
        playerSequences = new Hashtable();
        PhotonNetwork.AddCallbackTarget(this);//+ listener -> notified
    }

    // Start is called before the first frame update
    void Start()
    {
        setUI();
        if(PhotonNetwork.IsMasterClient) startGame();
    }

    private void populateCurrentPlayerUIs(int count)
    {
        newPlayerUIS = new GameObject[count];
        HashSet<int> set = new HashSet<int>();
        if (count < 7)
        {
            Destroy(playerUIs[2].gameObject);
            Destroy(playerUIs[6].gameObject);
            set.Add(2); set.Add(6);
        }
        if (count < 5)
        {
            Destroy(playerUIs[3].gameObject); Destroy(playerUIs[5].gameObject);
            set.Add(3); set.Add(5);
        }
        if (count == 5 || count == 7 || count == 3)
        {
            set.Add(4);
            Destroy(playerUIs[4].gameObject);
        }
        for(int i=0,j=0; i<playerUIs.Length;i++)
        {
            if (!set.Contains(i)) newPlayerUIS[j++] = playerUIs[i];
        }
    }

    private void setUI()
    {
        int sequence = (int)PhotonNetwork.CurrentRoom.CustomProperties["sequence"];
        int pos = -1;
        for(int i = 0; i < playersCount; i++)
        {
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{sequence + i}"];
            if (player == null)
            {
                Debug.Log($"This player is null.");
                continue;
            }
            Debug.Log($"This player not null: name:{player.NickName}, isLocalPlayer?{player.IsLocal}, isMasterClient?{player.IsMasterClient}.");//to be deleted
            if (player.IsLocal) pos = i;
            playerSequences.Add($"{i}", player);// nick 0, bill 1, eugene 2
        }
        if (pos == -1) return;
        Debug.Log($"Position found: {pos}");
        int originalPos = pos;
        int positionIndex = 0;
        bool passed = false;

        while(positionIndex < playersCount)
        {
            int posIndex = pos % playersCount;
            if (passed && posIndex == originalPos) break;
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{(posIndex + sequence)}"];
            newPlayerUIS[positionIndex].GetComponentsInChildren<Text>()[5].text = player.NickName;
            playerPositions.Add(player, positionIndex);
            positionIndex++;
            pos++;
            if(!passed) passed = true;
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log($"[OnRoomPropertiesUpdate]: UPDATING");

        if (roundZero && PhotonNetwork.IsMasterClient)
        {
            Deck = new Deck();
            serverDeck = Deck.shuffleCards();//Ï´ÅÆ
            distributeCards();
            roundZero = false;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)//2
    {
        if (changedProps.ContainsKey("playerStartDeck"))
        {
            object[] cards = (object[])changedProps["playerStartDeck"];
            newPlayerUIS[(int)playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[0].text = $"{cards.Length}";//3 + 2 = 5;
        }
        
    }

    private void distributeCards() 
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Player player in playerSequences.Values)
        {
            DrawCardsForPlayer(player, 3);
        }
    }

    private void startGame()
    {//room.customproperties -> hashtable(object,object)//string,int
        // int 1 - 113. Deck->feature/description. Server -> player (int)id. (int)id -> other players, yes? id->card(red?black?) -> +black, +red
        // player custom properties.Add("black",1) -> server -> my screen -> player.blackUI +1;
        object[] content = new object[] { turnCount };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(TurnStartEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        Debug.Log($"Startting the GGGGAAAMMMME");
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        Debug.Log($"[OnEvent]: RECEIVING DATA {eventCode}");

        if (eventCode == TurnStartEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;// turn?

            Player playerToStart = (Player)playerSequences[$"{data[0]}"];// 0 -> nick, 1 -> bill
            playerTurnUI.text = playerToStart.NickName;
            DrawCardsForPlayer(playerToStart, 2);
        }
    }

    public void DrawCardsForPlayer(Player player, int cardsNumToDraw)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        ExitGames.Client.Photon.Hashtable table = player.CustomProperties;
        if (table == null) table = new ExitGames.Client.Photon.Hashtable();

        object[] existingCards = new object[0];
        object[] startDeck;
        if (table.ContainsKey("playerStartDeck"))
        {
            existingCards = (object[])table["playerStartDeck"];
        }

        startDeck = new object[cardsNumToDraw + existingCards.Length];
        int i = 0;
        for (; i < existingCards.Length; i++)
        {
            startDeck[i] = existingCards[i];
        }
        for (; i < startDeck.Length; i++)
        {
            startDeck[i] = serverDeck.LastOrDefault().id;
            if (serverDeck.LastOrDefault() == null) break;
            serverDeck.Remove(serverDeck.LastOrDefault());
        }
        
        
        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = startDeck;
        else table.Add("playerStartDeck", startDeck);
        player.SetCustomProperties(table);
    }

}
