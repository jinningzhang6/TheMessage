using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public Color color { get; private set; }
    public int id { get; private set; }

    public Card(Color color, int id)
    {
        this.color = color;
        this.id = id;
    }
}
