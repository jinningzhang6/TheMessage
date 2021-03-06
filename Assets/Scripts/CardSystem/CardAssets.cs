using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAssets
{
    private Sprite[] spellCards;
    private Sprite[] backgroundCards;

    public CardAssets()
    {
        spellCards = new Sprite[24];
        backgroundCards = new Sprite[2];
        backgroundCards[0] = Resources.Load<Sprite>("normal_msg");
        backgroundCards[1] = Resources.Load<Sprite>("direct_msg");
        int i = 0;
        spellCards[i++]= Resources.Load<Sprite>("lock_blue");//0
        spellCards[i++] = Resources.Load<Sprite>("lock_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("lock_red");
        spellCards[i++] = Resources.Load<Sprite>("lock_red_black");
        spellCards[i++] = Resources.Load<Sprite>("away_red");//4
        spellCards[i++] = Resources.Load<Sprite>("away_red_black");
        spellCards[i++] = Resources.Load<Sprite>("away_black");
        spellCards[i++] = Resources.Load<Sprite>("away_blue");
        spellCards[i++] = Resources.Load<Sprite>("away_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("help_red");//9
        spellCards[i++] = Resources.Load<Sprite>("help_black");
        spellCards[i++] = Resources.Load<Sprite>("help_blue");
        spellCards[i++] = Resources.Load<Sprite>("redirect_red");//12
        spellCards[i++] = Resources.Load<Sprite>("redirect_black");
        spellCards[i++] = Resources.Load<Sprite>("redirect_blue");
        spellCards[i++] = Resources.Load<Sprite>("gamble_black");//15
        spellCards[i++] = Resources.Load<Sprite>("intercept_red");//16
        spellCards[i++] = Resources.Load<Sprite>("intercept_red_black");
        spellCards[i++] = Resources.Load<Sprite>("intercept_black");
        spellCards[i++] = Resources.Load<Sprite>("intercept_blue");
        spellCards[i++] = Resources.Load<Sprite>("intercept_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("gamble_all_red");//21
        spellCards[i++] = Resources.Load<Sprite>("gamble_all_blue");
        spellCards[i++] = Resources.Load<Sprite>("redraw_black");//23
    }

    public Sprite[] getCardBackground()
    {
        return backgroundCards;
    }
    public Sprite[] getCardAssets()
    {
        return this.spellCards;
    }
}
