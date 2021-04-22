using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    [SerializeField]
    private Text id;
    [SerializeField]
    private Image background;

    public void SetCardInfo(int id, Color color)
    {
        this.id.text = $"{id}";
        background.color = color;
    }
}
