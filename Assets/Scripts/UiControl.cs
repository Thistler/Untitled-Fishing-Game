using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiControl : MonoBehaviour
{
    public static UiControl uiControl;

    [SerializeField] private Image UiSeasonImage;
    [SerializeField] private Sprite[] SeasonSprites;

    [SerializeField] private Image UiWeatherImage;
    [SerializeField] private Sprite[] WeatherSprites;

    [SerializeField] private TextMeshProUGUI UiClock;

    [SerializeField] private GameObject BaitSwitchPanel;
    [SerializeField] private GameObject BaitItem;

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

    public void Update()
    {
        // Update clock
        DateTime time = DateTime.Now;
        string hour = LeadingZero(time.Hour);
        string minute = LeadingZero(time.Minute);

        UiClock.text = hour + ":" + minute;

        // When the hour switches, update weather
        if(time.Hour != GameControl.Control.CurrentHour)
        {
            GameControl.Control.CurrentHour = time.Hour;
            GameControl.Control.SetCurrentWeather();
            // If it's also a new day, update season
            if(time.Hour == 0)
            {
                GameControl.Control.SetCurrentSeason();
            }

            // Update fish list accordingly
            GameControl.Control.SetCurrentFishList();
        }

        if(Input.GetKey(KeyCode.Tab))
        {
            BaitSwitchPanel.gameObject.SetActive(true);
        }
        else
        {
            BaitSwitchPanel.gameObject.SetActive(false);
        }
    }

    public void BuildBaitInventory()
    {
        GridLayoutGroup baitGrid = BaitSwitchPanel.GetComponentInChildren<GridLayoutGroup>();
        // Clear existing panels
        foreach(Transform child in baitGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (KeyValuePair<string, int> bait in GameControl.Control.BaitInventory)
        {
            if(bait.Value > 0)
            {
                GameObject newBaitBtn = Instantiate(BaitItem);
                newBaitBtn.transform.Find("Icon").GetComponent<Image>().sprite = StaticData.Static.BaitSprites[bait.Key];
                newBaitBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = bait.Value.ToString();
                newBaitBtn.transform.SetParent(baitGrid.transform);
                newBaitBtn.GetComponent<Button>().onClick.AddListener(delegate () { GameControl.Control.SelectedBait = bait.Key; });
            }
        }
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    public void UpdateSeasonSprite()
    {
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

    public void UpdateWeatherSprite()
    {
        switch (GameControl.Control.CurrentWeather)
        {
            case "clear":
                UiWeatherImage.sprite = WeatherSprites[0];
                break;

            case "cloudy":
                UiWeatherImage.sprite = WeatherSprites[1];
                break;

            case "rainy":
                UiWeatherImage.sprite = WeatherSprites[2];
                break;

            case "snowy":
                UiWeatherImage.sprite = WeatherSprites[3];
                break;

            case "stormy":
                UiWeatherImage.sprite = WeatherSprites[4];
                break;

            case "foggy":
                UiWeatherImage.sprite = WeatherSprites[5];
                break;
        }
    }
}
