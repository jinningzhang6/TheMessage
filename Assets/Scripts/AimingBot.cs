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
    public Button charIcon;
    public GameObject directMsgDropZone;

    private string playerName;
    private float rotZ;
    private float RotationSpeed = 200;
    // Start is called before the first frame update
    void Start()
    {
        charIcon.onClick.AddListener(() => clickOnCharIcon());
        inGame = game_object.GetComponent<InGame>();
        if(aimingIndicator!=null) aimingIndicator.SetActive(false);
        playerName = GetComponentsInChildren<Text>()[5].text;

        EventTrigger trigger = directMsgDropZone.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drop;
        entry.callback.AddListener((data) => { OnDropMy((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    private void OnDropMy(PointerEventData eventData)
    {
        Debug.Log($"being droped on player: {playerName}");
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
