using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiControl : MonoBehaviour
{
    public static UiControl uiControl;

    [SerializeField] private Image UiSeasonImage;
    [SerializeField] private Sprite[] SeasonSprites;

    private GameObject UiImage;

    void Awake()
    {
        if (uiControl == null)
        {
            DontDestroyOnLoad(gameObject);
            uiControl = this;
        }
        else if (uiControl != this)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateSeasonSprite()
    {
        Debug.Log("setting image");
        switch (GameControl.Control.CurrentSeason)
        {
            case "spring":
                UiSeasonImage.sprite = SeasonSprites[0];
                break;

            case "summer":
                UiSeasonImage.sprite = SeasonSprites[1];
                break;

            case "fall":
                UiSeasonImage.sprite = SeasonSprites[2];
                break;

            case "winter":
                UiSeasonImage.sprite = SeasonSprites[3];
                break;
        }
    }
}
