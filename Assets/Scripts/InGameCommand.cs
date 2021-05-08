using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGameCommand : MonoBehaviourPunCallbacks, IOnEventCallback
{
    private int playersCount;
    private GameObject[] newPlayerUIS;
    private GameObject[] debuff_indicatorUI;
    private Hashtable playerPositions;
    private Hashtable playerSequences;
    private InGame inGame;
    
    public Text gameRealTimeInfo;
    public GameObject game_object;
    public GameObject acceptButton;
    public GameObject declineButton;
    private Player currentTurnPlayer;//subturn
    private SpellCardsListing spellCardsListing;

    private const int TurnStartEventCode = 1;
    private const int SendCardEventCode = 2;
    private const int SpellCardEventCode = 3;
    private const int ToEndTurnEventCode = 4;

    private void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;//3;// 
        PhotonNetwork.AddCallbackTarget(this);//+ listener -> notified
    }

    void Start()
    {
        inGame = game_object.GetComponent<InGame>();
        spellCardsListing = game_object.GetComponent<SpellCardsListing>();
        playerPositions = inGame.playerPositions;
        playerSequences = inGame.playerSequences;
        newPlayerUIS = inGame.newPlayerUIS;
        debuff_indicatorUI = new GameObject[newPlayerUIS.Length];
        for(int i = 0; i < newPlayerUIS.Length; i++)
        {
            debuff_indicatorUI[i] = newPlayerUIS[i].GetComponentsInChildren<Button>()[7].gameObject;
            debuff_indicatorUI[i].SetActive(false);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == TurnStartEventCode) resetUserDebuffUI();
        else if (eventCode == SpellCardEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            Debug.Log($"data length: {data.Length}");
            int spellType = (int)data[3];
            spellCardsListing.AddSpellCard((int)data[1]);
            if (spellType == 0) SpellLock((int)data[0], (int)data[2]);
            if (spellType == 1) SpellAway((int)data[0], (int)data[2]);
            if (spellType == 2) SpellHelp((int)data[2]);
            if (spellType == 3) SpellRedirect((int)data[0],(int)data[2]);
        }
        else if (eventCode == SendCardEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            currentTurnPlayer = (Player)playerSequences[$"{(int)data[0] % playersCount}"];
            if (currentTurnPlayer.IsLocal) checkIfUserDebuff(currentTurnPlayer);
        }
        else if (eventCode == ToEndTurnEventCode) spellCardsListing.ResetSpellCardListing();
    }

    private void SpellLock(int castPlayer, int toPlayer)
    {
        Player _player = (Player)playerSequences[$"{toPlayer}"];
        if (currentTurnPlayer == _player) declineButton.SetActive(false);
        gameRealTimeInfo.text = $"{((Player)playerSequences[$"{castPlayer}"]).NickName} 对 {_player.NickName} 使用了锁定";
        if (PhotonNetwork.IsMasterClient)  setPlayerDebuff(_player, "locked", true, "锁");
    }

    private void SpellAway(int castPlayer, int toPlayer)
    {
        Player _player = (Player)playerSequences[$"{toPlayer}"];
        if (currentTurnPlayer == _player) acceptButton.SetActive(false);
        gameRealTimeInfo.text = $"{((Player)playerSequences[$"{castPlayer}"]).NickName} 对 {_player.NickName} 使用了调虎离山";
        if (PhotonNetwork.IsMasterClient) setPlayerDebuff(_player, "awayed", true, "调");
    }

    private void SpellHelp(int castPlayer)
    {
        gameRealTimeInfo.text = $"{((Player)playerSequences[$"{castPlayer}"]).NickName} 使用了增援";
        if (PhotonNetwork.IsMasterClient)
        {
            int numBlack = inGame.getPlayerBlackMessage();
            inGame.DrawCardsForPlayer((Player)playerSequences[$"{castPlayer}"], numBlack + 1);
        }
    }

    private void SpellRedirect(int castPlayer, int toPlayer)
    {
        Debug.Log("doing in SpellRedirect");
        Player _player = (Player)playerSequences[$"{toPlayer}"];
        if (PhotonNetwork.IsMasterClient)
        {
            setPlayerDebuff((Player)playerSequences[$"{toPlayer}"], "redirected", true, "转");
            inGame.raiseCertainEvent(SendCardEventCode, new object[] { toPlayer, inGame.getCurrentCardId() });
            Debug.Log("cannot cast in SpellRedirect");
        }
        gameRealTimeInfo.text = $"{((Player)playerSequences[$"{castPlayer}"]).NickName} 对 {_player.NickName} 使用了转移";
    }

    private void setPlayerDebuff(Player player,string debuffName,bool debuff,string keyword)
    {
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        if (!table.ContainsKey(debuffName)) table.Add(debuffName, debuff);
        else table[debuffName] = debuff;

        debuff_indicatorUI[(int)playerPositions[player]].SetActive(debuff);//
        foreach(Text text in debuff_indicatorUI[(int)playerPositions[player]].GetComponentsInChildren<Text>()) text.text = keyword;
        player.SetCustomProperties(table);
    }

    private void checkIfUserDebuff(Player player)
    {
        acceptButton.SetActive(true);
        declineButton.SetActive(true);
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        if (table.ContainsKey("redirected") && (bool)table["redirected"]) declineButton.SetActive(false);//redirect > lock > away
        else if (table.ContainsKey("locked") && (bool)table["locked"]) declineButton.SetActive(false);
        else if (table.ContainsKey("awayed") && (bool)table["awayed"]) acceptButton.SetActive(false);
    }

    private void resetUserDebuffUI()
    {
        for(int i=0; i < debuff_indicatorUI.Length; i++)
        {
            debuff_indicatorUI[i].SetActive(false);
        }
        acceptButton.SetActive(true);
        declineButton.SetActive(true);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
