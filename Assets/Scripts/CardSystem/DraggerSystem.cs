using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggerSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    //private CardItem cardItem = null;
    private CanvasGroup canvas;
    public static bool onDraggingCard = false;

    void Start()
    {
        if (name == "PassingCardX") gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = transform.parent;
        if (name != "PassingCardX") transform.SetParent(transform.parent.parent);
        else gameObject.SetActive(true);
        canvas = eventData.pointerDrag.GetComponent<CanvasGroup>();
        canvas.alpha = 0.6f;
        canvas.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (name == "PassingCardX") onDraggingCard = true;
        transform.position = eventData.position;
        InGame.showAllReceivingCardSection();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;

        if (name == "PassingCardX") onDraggingCard = false;
        transform.SetParent(parentToReturnTo);
        CardListing.selectedCard = null;
        InGame.hideAllReceivingCardSection();
    }
}
