using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//UI Card
public class CardItem : MonoBehaviour
{
    [SerializeField]
    private Text id;
    [SerializeField]
    private Image background;

    public int cardId { get; private set; }
    public int cardType { get; private set; }

    public void SetCardInfo(int id, Sprite image, int type)
    {
        this.id.text = $"{id}";
        background.sprite = image;
        cardId = id;
        cardType = type;
    }

    public void OnSelected()
    {
        if(CardListing.selectedCard == null) MoveUpPosition();
        else
        {
            Vector2 position = CardListing.selectedCard.transform.position;
            position.y -= 100;
            CardListing.selectedCard.transform.position = position;
            if (CardListing.selectedCard.cardId == this.cardId) CardListing.selectedCard = null;
            else MoveUpPosition();
        }
    }

    private void MoveUpPosition()
    {
        Vector2 position = transform.position;
        position.y += 100;
        transform.position = position;
        CardListing.selectedCard = this;
    }

}
