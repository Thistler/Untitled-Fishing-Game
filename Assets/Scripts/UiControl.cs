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
    [SerializeField] private Sprite[] SeasonSpritesArray;
    private Dictionary<string, Sprite> SeasonSpriteDictionary;
    private Dictionary<string, int> SeasonDictionaryParam; // Hack for param typing


    [SerializeField] private Image UiWeatherImage;
    [SerializeField] private Sprite[] WeatherSpritesArray;
    private Dictionary<string, Sprite> WeatherSpritesDictionary;
    private Dictionary<string, int> WeatherDictionaryParam; // Hack for param typing

    [SerializeField] private Sprite[] LocationSpritesArray;
    private Dictionary<string, Sprite> LocationSpriteDictionary;

    [SerializeField] private TextMeshProUGUI UiClock;

    [SerializeField] public GameObject BaitSwitchPanel;
    [SerializeField] private GameObject BaitItem;

    [SerializeField] public GameObject FishDex;
    [SerializeField] private GameObject FishDexGeneralIcon;
    [SerializeField] private GameObject FishDexHourIcon;

    [SerializeField] private GameObject FishList;

    [SerializeField] private Image FishPanelIcon;
    [SerializeField] private TextMeshProUGUI FishPanelName;
    [SerializeField] private GameObject FishPanelHoursPanel;
    [SerializeField] private GameObject FishPanelSeasonsPanel;
    [SerializeField] private GameObject FishPanelWeathersPanel;
    [SerializeField] private GameObject FishPanelLocationsPanel;
    [SerializeField] private GameObject FishPanelBaitPanel;


    private GameObject UiImage;

    void Awake()
    {
        if (uiControl == null)
        {
            DontDestroyOnLoad(gameObject);
            uiControl = this;

            // TODO: Possibly move this and other sprites to StaticData for consistency
            SeasonSpriteDictionary = new Dictionary<string, Sprite>()
            {
                { "spring", SeasonSpritesArray[0] },
                { "summer", SeasonSpritesArray[1] },
                { "fall", SeasonSpritesArray[2] },
                { "winter", SeasonSpritesArray[3] }
            };

            SeasonDictionaryParam = new Dictionary<string, int>()
            {
                { "spring", 0 },
                { "summer", 0 },
                { "fall", 0 },
                { "winter", 0 }
            };

            WeatherSpritesDictionary = new Dictionary<string, Sprite>()
            {
                { "clear", WeatherSpritesArray[0] },
                { "cloudy", WeatherSpritesArray[1] },
                { "rainy", WeatherSpritesArray[2] },
                { "snowy", WeatherSpritesArray[3] },
                { "stormy", WeatherSpritesArray[4] },
                { "foggy", WeatherSpritesArray[5] }

            };

            WeatherDictionaryParam = new Dictionary<string, int>()
            {
                { "clear", 0 },
                { "cloudy", 0 },
                { "rainy", 0 },
                { "snowy", 0 },
                { "stormy", 0 },
                { "foggy", 0 }

            };

            LocationSpriteDictionary = new Dictionary<string, Sprite>()
            {
                { "cabin_pond_shallow", LocationSpritesArray[0] },
                { "cabin_pond_deep", LocationSpritesArray[1] }
            };
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

        if(Input.GetKey(KeyCode.Tab) && !FishDex.activeInHierarchy)
        {
            BaitSwitchPanel.gameObject.SetActive(true);
        }
        else
        {
            BaitSwitchPanel.gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.J) && !BaitSwitchPanel.activeInHierarchy)
        {
            FishDex.SetActive(!FishDex.activeInHierarchy);
        }
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    // Menus
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
                newBaitBtn.transform.Find("Icon").GetComponent<Image>().sprite = StaticData.Static.BaitSpritesDictionary[bait.Key];
                newBaitBtn.transform.GetComponentInChildren<TextMeshProUGUI>().text = bait.Value.ToString();
                newBaitBtn.transform.SetParent(baitGrid.transform);
                newBaitBtn.GetComponent<Button>().onClick.AddListener(delegate () { GameControl.Control.SelectedBait = bait.Key; });
            }
        }
    }

    public void BuildFishDex()
    {
        PopulateFishListForLevel();
        RenderFishPanelWithSelectedFish(StaticData.Static.FullFishSpeciesList[0]);
    }

    // TODO Fish for each level should probably be calculated at the beginning of the game, or maybe just hardcoded
    public void PopulateFishListForLevel()
    {
        // TODO: Temp
        string[] currentLevelTiles = { "cabin_pond_shallow", "cabin_pond_deep" };
        /////

        List<StaticData.FishSpecies> fishInLevel = new List<StaticData.FishSpecies>();
        
        /// TODO: Redo how we find the fish in a given level, we shouldn't be recalculating it constantly
        foreach(StaticData.FishSpecies species in StaticData.Static.FullFishSpeciesList)
        {
            // TODO: Should probably call this at the beginning of the game so it's not recalculated multiple times
            List<string> tilesFishIsIn = new List<string>();
            foreach(StaticData.FishTileData tile in species.tiles)
            {
                tilesFishIsIn.Add(tile.tilename);
            }
            /////////
            foreach(string currentTile in currentLevelTiles)
            {
                foreach(string fishTile in tilesFishIsIn)
                {
                    if (currentTile == fishTile && !fishInLevel.Contains(species)) fishInLevel.Add(species);
                }
            }
        }
        /////
        foreach(StaticData.FishSpecies species in fishInLevel)
        {
            GameObject newFishIcon = Instantiate(FishDexGeneralIcon);
            newFishIcon.transform.SetParent(FishList.transform);
            newFishIcon.GetComponent<Image>().sprite = species.sprite;
            newFishIcon.GetComponent<Button>().onClick.AddListener(delegate () { RenderFishPanelWithSelectedFish(species); });
        }
    }

    public void RenderFishPanelWithSelectedFish(StaticData.FishSpecies argSpecies)
    {
        FishPanelIcon.GetComponent<Image>().sprite = argSpecies.sprite;
        FishPanelName.GetComponent<TextMeshProUGUI>().text = argSpecies.species;

        // Hours
        GridLayoutGroup hoursGrid = FishPanelHoursPanel.GetComponent<GridLayoutGroup>();
        // Clear existing panels
        foreach (Transform child in hoursGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        } // TODO: Make this its own method
        for (int i = 0; i < 24; i++)
        {
            GameObject hourIcon = Instantiate(FishDexHourIcon);
            hourIcon.transform.SetParent(FishPanelHoursPanel.transform);
            hourIcon.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            if(argSpecies.hours == null || argSpecies.hours.ContainsKey(i))
            {
                hourIcon.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                hourIcon.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
            }
        }

        // Seasons
        if (argSpecies.seasons == null)
        {
            RenderFishDexPanel(FishPanelSeasonsPanel, SeasonDictionaryParam, SeasonSpriteDictionary);
        }
        else
        {
            RenderFishDexPanel(FishPanelSeasonsPanel, argSpecies.seasons, SeasonSpriteDictionary);
        }

        // Weathers
        if (argSpecies.weathers == null)
        {
            RenderFishDexPanel(FishPanelWeathersPanel, WeatherDictionaryParam, WeatherSpritesDictionary);
        }
        else
        {
            RenderFishDexPanel(FishPanelWeathersPanel, argSpecies.weathers, WeatherSpritesDictionary);
        }

        // Locations
        Dictionary<string, int> speciesTiles = new Dictionary<string, int>();
        foreach(StaticData.FishTileData tile in argSpecies.tiles)
        {
            speciesTiles.Add(tile.tilename, 0);
        }
        RenderFishDexPanel(FishPanelLocationsPanel, speciesTiles, LocationSpriteDictionary);

        // Baits
        RenderFishDexPanel(FishPanelBaitPanel, argSpecies.baits, StaticData.Static.BaitSpritesDictionary);
    }

    private void RenderFishDexPanel(GameObject argGridPanel, Dictionary<string, int> argFishField, Dictionary<string, Sprite> argSpriteDictionary)
    {
        GridLayoutGroup grid = argGridPanel.GetComponent<GridLayoutGroup>();
        // Clear existing panels
        foreach (Transform child in grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (KeyValuePair<string, int> item in argFishField)
        {
            GameObject itemIcon = Instantiate(FishDexGeneralIcon);
            itemIcon.transform.SetParent(argGridPanel.transform);
            itemIcon.GetComponent<Image>().sprite = argSpriteDictionary[item.Key];
        }
    }

    // Top UI
    public void UpdateSeasonSprite()
    {
        UiSeasonImage.sprite = SeasonSpriteDictionary[GameControl.Control.CurrentSeason];
    }

    public void UpdateWeatherSprite()
    {
        UiWeatherImage.sprite = WeatherSpritesDictionary[GameControl.Control.CurrentWeather];
    }
}
