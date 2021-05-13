using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AimingBot : MonoBehaviour
{
    private InGame inGame;
    public GameObject game_object;
    public GameObject aimingIndicator;
    private Button charIcon;
    private Button directMsgDropZone;

    private string playerName;
    private float rotZ;
    private float RotationSpeed = 200;
    // Start is called before the first frame update
    void Start()
    {
        inGame = game_object.GetComponent<InGame>();
        if(aimingIndicator!=null) aimingIndicator.SetActive(false);
        playerName = GetComponentsInChildren<Text>()[5].text;
        charIcon = GetComponentsInChildren<Button>()[0];
        directMsgDropZone = GetComponentsInChildren<Button>()[8];
        charIcon.onClick.AddListener(() => clickOnCharIcon());
        if (charIcon == null) Debug.Log($" not good in: {playerName}");
        if (directMsgDropZone == null) Debug.Log($"error at {playerName}");
        OnPointerOut(null);
        directMsgDropZone.gameObject.SetActive(false);
        assignEventTriggers();
    }

    public void OnDropMy(PointerEventData eventData)
    {
        InGame.hideAllReceivingCardSection();
        int cardId = -1;
        if (eventData.pointerDrag.TryGetComponent(out CardItem item)) cardId = eventData.selectedObject.GetComponent<CardItem>().cardId;
        else cardId = inGame.currentCardId;
        CardListing.selectedCard = null;
        if (cardId == -1) return;
        inGame.raiseCertainEvent(2, new object[] { (int)inGame.playerSequencesByName[playerName], cardId });// sendcardEvent
        inGame.cardListing.removeSelectedCardFromHand(cardId);
    }

    private void OnPointerIn(PointerEventData eventData)
    {
        CanvasGroup canvas = directMsgDropZone.GetComponentsInChildren<CanvasGroup>()[0];
        canvas.alpha = 1f;
    }

    private void OnPointerOut(PointerEventData eventData)
    {
        CanvasGroup canvas = directMsgDropZone.GetComponentsInChildren<CanvasGroup>()[0];
        canvas.alpha = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        if (aimingIndicator != null)
        {
            if (inGame.usingSpell)
            {
                aimingIndicator.SetActive(true);
                rotZ += Time.deltaTime * RotationSpeed;
                aimingIndicator.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }
            else
            {
                aimingIndicator.SetActive(false);
            }
        }
    }

    private void assignEventTriggers()
    {
        EventTrigger trigger = directMsgDropZone.GetComponent<EventTrigger>();
        EventTrigger.Entry drop = new EventTrigger.Entry();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerIn((PointerEventData)data); });
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { OnPointerOut((PointerEventData)data); });
        drop.eventID = EventTriggerType.Drop;
        drop.callback.AddListener((data) => { OnDropMy((PointerEventData)data); });
        trigger.triggers.Add(drop);
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exit);
    }

    void clickOnCharIcon()
    {
        if (!inGame.usingSpell || CardListing.selectedCard == null) return;
        inGame.useSpellCard(CardListing.selectedCard.cardId, CardListing.selectedCard.cardType,playerName);// playername-> target player
        inGame.cancelSpell();
    }

    public void clickOnMsgButton()
    {
        inGame.showBurnCardWindow(playerName);
    }
}
