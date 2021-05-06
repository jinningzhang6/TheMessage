using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameAnimation : MonoBehaviour
{
    public GameObject InGameObjects;
    public GameObject passingCardLive;
    private InGame inGame;
    private Sprite[] passingCardBck;

    // Start is called before the first frame update
    void Start()
    {
        inGame = InGameObjects.GetComponent<InGame>();
        passingCardBck = new CardAssets().getCardBackground();
        passingCardLive.SetActive(false);
        passingCardLive.GetComponent<Image>().sprite = passingCardBck[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame.shouldAnimatePassingCard)
        {
            passingCardLive.SetActive(true);
            passingCardLive.transform.position = Vector3.Lerp(passingCardLive.transform.position, inGame.passingCardPosition, 5 * Time.deltaTime);
        }
        else
        {
            passingCardLive.SetActive(false);
        }
    }
}
