using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MsgDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    //private CardItem cardItem = null;
    private CanvasGroup canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = transform.parent;
        transform.SetParent(transform.parent.parent);
        canvas = eventData.selectedObject.GetComponent<CanvasGroup>();
        canvas.alpha = 0.6f;
        canvas.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        InGame.showAllReceivingCardSection();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;
        transform.SetParent(parentToReturnTo);
        CardListing.selectedCard = null;
        InGame.hideAllReceivingCardSection();
    }
}
