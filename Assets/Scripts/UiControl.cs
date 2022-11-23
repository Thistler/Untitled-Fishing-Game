using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiControl : MonoBehaviour
{
    public static UiControl uiControl;

    [SerializeField] private GameObject LevelBar;

    [SerializeField] private Image UiSeasonImage;
    [SerializeField] private Sprite[] SeasonSpritesArray;
    private Dictionary<string, Sprite> SeasonSpriteDictionary;
    private Dictionary<string, int> SeasonDictionaryParam; // Hack for param typing


    [SerializeField] private Image UiWeatherImage;
    [SerializeField] private Sprite[] WeatherSpritesArray;
    private Dictionary<string, Sprite> WeatherSpritesDictionary;
    private Dictionary<string, int> WeatherDictionaryParam; // Hack for param typing

    [SerializeField] private Image UiCurrentBaitPanel;
    [SerializeField] private TextMeshProUGUI CurrentBaitCount;

    [SerializeField] private Sprite[] LocationSpritesArray;
    private Dictionary<string, Sprite> LocationSpriteDictionary;

    [SerializeField] private TextMeshProUGUI UiClock;

    [SerializeField] public GameObject BaitSwitchPanel;
    [SerializeField] private GameObject BaitItem;

    [SerializeField] public GameObject TalentPanel;

    [SerializeField] public GameObject MessagePanel;

    [SerializeField] public GameObject FishDex;
    [SerializeField] private GameObject FishDexGeneralIcon;
    [SerializeField] private GameObject FishDexHourIcon;
    [SerializeField] private Sprite QuestionmarkSprite;

    [SerializeField] private GameObject FishList;

    [SerializeField] private Image FishPanelIcon;
    [SerializeField] private TextMeshProUGUI FishPanelName;
    [SerializeField] private GameObject FishPanelHoursPanel;
    [SerializeField] private GameObject FishPanelSeasonsPanel;
    [SerializeField] private GameObject FishPanelWeathersPanel;
    [SerializeField] private GameObject FishPanelLocationsPanel;
    [SerializeField] private GameObject FishPanelBaitPanel;

    [SerializeField] private Sprite ErrorSprite;

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

        // TODO: Should probably have a more robust system for determining if another menu is open
        if(Input.GetKey(KeyCode.Tab) && !FishDex.activeInHierarchy && !TalentPanel.activeInHierarchy)
        {
            BaitSwitchPanel.gameObject.SetActive(true);
        }
        else
        {
            BaitSwitchPanel.gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.J) && !BaitSwitchPanel.activeInHierarchy && !TalentPanel.activeInHierarchy)
        {
            BuildFishDex(); // TODO: Might not be necessary if this is getting done elsewhere
            FishDex.SetActive(!FishDex.activeInHierarchy);
        }

        if(Input.GetKeyDown(KeyCode.Escape) && !BaitSwitchPanel.activeInHierarchy && !FishDex.activeInHierarchy)
        {
            BuildTalentMenu(); // TODO: Might not be necessary if this is getting done elsewhere
            TalentPanel.SetActive(!TalentPanel.activeInHierarchy);
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
                newBaitBtn.GetComponent<Button>().onClick.AddListener(delegate () { GameControl.Control.SelectedBait = bait.Key; UpdateBaitSprite(); });
            }
        }
    }

    public void BuildTalentMenu()
    {
        TalentPanel.transform.Find("TalentPointsText").GetComponent<TextMeshProUGUI>().text = "+" + GameControl.Control.PlayerTalentPoints.ToString();

        GridLayoutGroup talentGrid = TalentPanel.GetComponentInChildren<GridLayoutGroup>();
        foreach(KeyValuePair<string, int> talent in GameControl.Control.PlayerTalents)
        {
            Transform talentPanel = talentGrid.transform.Find(talent.Key).transform;
            talentPanel.Find("Level").GetComponent<TextMeshProUGUI>().text = talent.Value.ToString();
            if(GameControl.Control.PlayerTalentPoints > 0)
            {
                talentPanel.Find("AddTalentBtn").gameObject.SetActive(true);
                talentPanel.Find("AddTalentBtn").GetComponent<Button>().onClick.AddListener(delegate () { AddTalent(talent.Key); });
            }
            else
            {
                talentPanel.Find("AddTalentBtn").gameObject.SetActive(false);
            }
        }
    }

    private void AddTalent(string argTalent)
    {
        GameControl.Control.PlayerTalentPoints--;
        GameControl.Control.PlayerTalents[argTalent] += 1;
        BuildTalentMenu();
        UpdateTalentNotification();
        GameControl.Control.Save();
    }

    public void BuildFishDex()
    {
        PopulateFishListForLevel();
        //RenderFishPanelWithSelectedFish(StaticData.Static.FullFishSpeciesList[0]);  // TODO: Remove this for now, may replace it with rough fish later
    }

    // TODO Fish for each level should probably be calculated at the beginning of the game, or maybe just hardcoded
    public void PopulateFishListForLevel()
    {
        // TODO: Temp
        string[] currentLevelTiles = { "cabin_pond_shallow", "cabin_pond_deep" };
        /////

        List<StaticData.FishSpecies> fishInLevel = new List<StaticData.FishSpecies>();
        
        // TODO: Redo how we find the fish in a given level, we shouldn't be recalculating it constantly
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


        ClearGridGroupChildren(FishList);
        int fishIconCount = 0;
        foreach(StaticData.FishSpecies species in fishInLevel)
        {
            if(GameControl.Control.UnlockedFishDataList.ContainsKey(species.species))
            {
                GameObject newFishIcon = AddIconToGrid(FishDexGeneralIcon, FishList, species.sprite);
                newFishIcon.GetComponent<Button>().onClick.AddListener(delegate () { RenderFishPanelWithSelectedFish(species); });
                fishIconCount++;
            }
        }
        // Add question mark if there are undiscovered fish
        if(fishIconCount < fishInLevel.Count)
        {
            AddIconToGrid(FishDexGeneralIcon, FishList, QuestionmarkSprite);
        }
        else
        {
            FishList.GetComponent<Image>().color = Color.yellow;
        }
    }

    public void RenderFishPanelWithSelectedFish(StaticData.FishSpecies argSpecies)
    {
        FishPanelIcon.GetComponent<Image>().sprite = argSpecies.sprite;
        FishPanelName.GetComponent<TextMeshProUGUI>().text = argSpecies.species;

        // Hours
        ClearGridGroupChildren(FishPanelHoursPanel);
        for (int i = 0; i < 24; i++)
        {
            // Check if we have all available hours discovered for the fish
            bool displayFullHoursList = false;
            if (GameControl.Control.UnlockedFishDataList[argSpecies.species] != null && GameControl.Control.UnlockedFishDataList[argSpecies.species].hours.Count == argSpecies.hours.Count)
            {
                displayFullHoursList = true;
                FishPanelHoursPanel.GetComponent<Image>().color = Color.yellow;
            }
            // If all hours are discovered OR current hour is discovered, display that data
            if(displayFullHoursList || GameControl.Control.UnlockedFishDataList[argSpecies.species].hours.Contains(i))
            {
                GameObject hourIcon = AddIconToGrid(FishDexHourIcon, FishPanelHoursPanel, null, i.ToString());
                if (argSpecies.hours == null || argSpecies.hours.ContainsKey(i))
                {
                    hourIcon.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
                }
                else
                {
                    hourIcon.GetComponentInChildren<TextMeshProUGUI>().color = Color.grey;
                }
            }
            // If current hour is not discovered, display question block
            else
            {
                AddIconToGrid(FishDexGeneralIcon, FishPanelHoursPanel, QuestionmarkSprite);
            }
        }

        // Seasons
        if (argSpecies.seasons == null)
        {
            RenderFishDexPanel(FishPanelSeasonsPanel, SeasonDictionaryParam, SeasonSpriteDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].seasons);
        }
        else
        {
            RenderFishDexPanel(FishPanelSeasonsPanel, argSpecies.seasons, SeasonSpriteDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].seasons);
        }

        // Weathers
        if (argSpecies.weathers == null)
        {
            RenderFishDexPanel(FishPanelWeathersPanel, WeatherDictionaryParam, WeatherSpritesDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].weathers);
        }
        else
        {
            RenderFishDexPanel(FishPanelWeathersPanel, argSpecies.weathers, WeatherSpritesDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].weathers);
        }

        // Locations
        Dictionary<string, int> speciesTiles = new Dictionary<string, int>();
        foreach(StaticData.FishTileData tile in argSpecies.tiles)
        {
            speciesTiles.Add(tile.tilename, 0);
        }
        RenderFishDexPanel(FishPanelLocationsPanel, speciesTiles, LocationSpriteDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].tiles);

        // Baits
        RenderFishDexPanel(FishPanelBaitPanel, argSpecies.baits, StaticData.Static.BaitSpritesDictionary, GameControl.Control.UnlockedFishDataList[argSpecies.species].baits);
    }

    public void DisplayMessage(string argMessage, Sprite argSprite)
    {
        StopCoroutine("WaitToHideMessage");
        MessagePanel.SetActive(true);
        MessagePanel.GetComponentInChildren<TextMeshProUGUI>().text = argMessage;
        if(argSprite == null)
        {
            MessagePanel.transform.Find("MsgImage").GetComponent<Image>().sprite = ErrorSprite;
        }
        else
        {
            MessagePanel.transform.Find("MsgImage").GetComponent<Image>().sprite = argSprite;
        }
        StartCoroutine("WaitToHideMessage");
    }

    private IEnumerator WaitToHideMessage()
    {
        yield return new WaitForSeconds(2.0f);
        MessagePanel.SetActive(false);
    }

    ////////////////////////////////////
    // Utils
    ////////////////////////////////////
    private void RenderFishDexPanel(GameObject argGridPanel, Dictionary<string, int> argFishField, Dictionary<string, Sprite> argSpriteDictionary, List<string> argFishDiscoveryList)
    {
        ClearGridGroupChildren(argGridPanel);
        // Add new panels
        foreach (KeyValuePair<string, int> item in argFishField)
        {
            // Only add the panel if the information is discovered
            if(argFishDiscoveryList.Contains(item.Key))
            {
                AddIconToGrid(FishDexGeneralIcon, argGridPanel, argSpriteDictionary[item.Key]);
}
        }
        // Add question mark if any information is still undiscovered
        if(argFishDiscoveryList.Count < argFishField.Count)
        {
            AddIconToGrid(FishDexGeneralIcon, argGridPanel, QuestionmarkSprite);
        }
        else
        {
            argGridPanel.GetComponent<Image>().color = Color.yellow;
        }
    }

    private void ClearGridGroupChildren(GameObject argGridPanel)
    {
        GridLayoutGroup grid = argGridPanel.GetComponent<GridLayoutGroup>();
        foreach (Transform child in grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    // Returns icon as GameObject so we can add listeners to it afterwards
    // TODO: Possibly add an optional param here that removes the button component from the object
    private GameObject AddIconToGrid(GameObject argIcon, GameObject argGridPanel, Sprite argSprite = null, string argText = null)
    {
        GameObject newIcon = Instantiate(argIcon);
        newIcon.transform.SetParent(argGridPanel.transform);
        if(argSprite != null) newIcon.GetComponent<Image>().sprite = argSprite;
        if (argText != null) newIcon.GetComponentInChildren<TextMeshProUGUI>().text = argText;
        return newIcon;
    }

    ////////////////////////////////////
    // Top UI
    ////////////////////////////////////
    public void UpdateLevelAndXpBar()
    {
        LevelBar.transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = GameControl.Control.PlayerLevel.ToString();
        LevelBar.transform.Find("LevelBarXpText").GetComponent<TextMeshProUGUI>().text = 
            GameControl.Control.PlayerXp.ToString() + " / " + StaticData.Static.LevelXpThresholds[GameControl.Control.PlayerLevel].ToString();

        // TODO: Maybe we should store xp as floats so we don't have to do this conversion
        float xpPercent = (float)GameControl.Control.PlayerXp / (float)StaticData.Static.LevelXpThresholds[GameControl.Control.PlayerLevel];
        LevelBar.transform.Find("LevelBarFill").transform.localScale = new Vector3(xpPercent, 1);

        UpdateTalentNotification();
    }

    public void UpdateTalentNotification()
    {
        if (GameControl.Control.PlayerTalentPoints > 0)
        {
            LevelBar.transform.Find("NewTalentPanel").gameObject.SetActive(true);
        }
        else
        {
            LevelBar.transform.Find("NewTalentPanel").gameObject.SetActive(false);
        }
    }

    public void UpdateSeasonSprite()
    {
        UiSeasonImage.sprite = SeasonSpriteDictionary[GameControl.Control.CurrentSeason];
    }

    public void UpdateWeatherSprite()
    {
        UiWeatherImage.sprite = WeatherSpritesDictionary[GameControl.Control.CurrentWeather];
    }

    public void UpdateBaitSprite()
    {
        if(GameControl.Control.SelectedBait == null)
        {
            UiCurrentBaitPanel.sprite = ErrorSprite;
            CurrentBaitCount.text = "";
        }
        else
        {
            UiCurrentBaitPanel.sprite = StaticData.Static.BaitSpritesDictionary[GameControl.Control.SelectedBait];
            CurrentBaitCount.text = GameControl.Control.BaitInventory[GameControl.Control.SelectedBait].ToString();
        }
    }
}
