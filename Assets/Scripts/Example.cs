using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject clickmeButton;
    // Start is
    // called before the first frame update
    void Start()
    {
        
    }

    public void clickOnTheButton()
    {
        clickmeButton.SetActive(false);
    }

}
