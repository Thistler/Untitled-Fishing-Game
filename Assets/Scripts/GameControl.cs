using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using static StaticData;
using System.Linq;
using Random = UnityEngine.Random;

public class GameControl : MonoBehaviour
{
    public static GameControl Control;

    // Saveable data
    // Player Stats
    public float PlayerXp;

    // Inventory
    public Dictionary<string, int> BaitInventory = new Dictionary<string, int>();
    public string SelectedBait;

    public System.DateTime TimeOfFirstSave;
    public System.DateTime TimeOfLastSave;

    public string CurrentWeather;

    // Non saveable data
    public List<FishSpecies> CurrentAvailableFish;

    public string CurrentSeason;
    public int CurrentHour;

    void Start()
    {
        Debug.Log("Game control awake");
        if (Control == null)
        {
            DontDestroyOnLoad(gameObject);
            Control = this;

            Load();
        }
        else if (Control != this)
        {
            Destroy(gameObject);
        }
    }

    public void Save()
    {
        // Create file
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        // Get current time
        var currentTime = System.DateTime.Now;

        // Set data to save
        PlayerData data = new PlayerData();

        // Player Data
        data.PlayerXp = PlayerXp;

        // Inventory
        data.BaitInventory = BaitInventory;
        data.SelectedBait = SelectedBait;

        // Time Since Last Save
        data.timeOfFirstSave = TimeOfFirstSave;
        data.timeOfLastSave = currentTime;

        // Weather
        data.currentWeather = CurrentWeather;

        // Save and close file
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        Debug.Log("Loading from " + Application.persistentDataPath);

        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            // Load existing data
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            TimeOfFirstSave = data.timeOfFirstSave;
            // Calculate time since last save
            TimeOfLastSave = data.timeOfLastSave;

            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = currentTime - TimeOfLastSave;

            Debug.Log("Game file created on " + TimeOfFirstSave.Date);
            Debug.Log(timeDifference.Minutes + " minutes since last save.");

            CurrentWeather = data.currentWeather ?? "clear";

            // Player Stats
            PlayerXp = data.PlayerXp;

            // Inventory
            BaitInventory = data.BaitInventory;
            SelectedBait = data.SelectedBait ?? "worm"; // TODO: Probably temp
        }
        else
        {
            // Create new game
            Debug.Log("Creating a new game.");

            // World's start time will be midnight of the current day
            DateTime currentDate = DateTime.Now;
            TimeOfFirstSave = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);

            // Set default variables and create the save file
            TimeOfLastSave = DateTime.Now;
            CurrentSeason = "spring";
            CurrentWeather = "clear";
            Save();
        }

        // Set time
        CurrentHour = DateTime.Now.Hour;

        // Set season
        SetCurrentSeason();

        // Set current weather
        Debug.Log("Time of last save: " + TimeOfLastSave);
        int hoursSinceSave = (DateTime.Now - TimeOfLastSave).Hours;
        Debug.Log("Hours since last save: " + hoursSinceSave);
        if (hoursSinceSave > 4) hoursSinceSave = 4;
        // Re-Roll weather an appropriate amount of times
        // This makes weather a bit more realistic for players who reopen the game after only an hour or two
        for (int i=0; i < hoursSinceSave; i++)
        {
            SetCurrentWeather();
        }
        Debug.Log("Current weather is " + CurrentWeather);
        

        // Set fish list
        SetCurrentFishList();

        // Set up Ui
        UiControl.uiControl.BuildBaitInventory();
        UiControl.uiControl.UpdateWeatherSprite();
    }

    // TODO: Make sure this is called every hour
    // Sets list of fish currently available based on current season, weather, and time of day
    public void SetCurrentFishList()
    {
        Debug.Log("Setting fish list");
        CurrentAvailableFish = new List<FishSpecies>();
        foreach (FishSpecies species in StaticData.Static.FullFishSpeciesList)
        {
            // If seasons, hours, or weathers is null, that means the fish is not restricted by those fields.
            // So we make sure that the field is either null or matches the current value for those fields
            if ((species.seasons == null || species.seasons.ContainsKey(CurrentSeason))
                && (species.hours == null || species.hours.ContainsKey(CurrentHour))
                && (species.weathers == null || species.weathers.ContainsKey(CurrentWeather))
                )
            {
                CurrentAvailableFish.Add(species);
                Debug.Log("Added " + species.species + " to fish list");
            }
        }
    }

    // Gets the amount of days since the game save was created, then divides by eight, and uses decimal place to calculate the time of "year"
    public void SetCurrentSeason()
    {
        // TODO: Might be prudent to store currentDateMidnight elsewhere so we don't have to recalculate too much
        DateTime currentDate = DateTime.Now;
        DateTime currentDateMidnight = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);

        double daysSinceFirstSave = (currentDateMidnight - TimeOfFirstSave.Date).TotalDays;
        Debug.Log(daysSinceFirstSave + " days since file was created");

        double daysDivided = daysSinceFirstSave / 8;
        double decimalOnly = daysDivided - Math.Truncate(daysDivided);

        Debug.Log("Days divided decimal is " + decimalOnly);

        // 0/8 = 0.0 - first day of spring
        // 1/8 = 0.125 - second day of spring
        // 2/8 = 0.25 - first day of summer
        // 3/8 = 0.375 - second day of summer
        // 4/8 = 0.5 - first day of fall
        // 5/8 = 0.625 - second day of fall
        // 6/8 = 0.75 - first day of winter
        // 7/8 = 8.75 - Second day of winter
        if (decimalOnly <= 0.125)
        {
            CurrentSeason = "spring";
        }
        else if (decimalOnly <= 0.375)
        {
            CurrentSeason = "summer";
        }
        else if (decimalOnly <= 0.625)
        {
            CurrentSeason = "fall";
        }
        else
        {
            CurrentSeason = "winter";
        }

        Debug.Log("Current season is " + CurrentSeason);
        UiControl.uiControl.UpdateSeasonSprite();
    }

    // Note: Make sure the season is updated BEFORE calling this method, when appropriate
    public void SetCurrentWeather()
    {
        // Set the list of available weather
        List<string> availableWeather = new List<string>();
        switch (CurrentWeather)
        {
            case "clear":
                //  Can stay the same, or switch to either cloudy or foggy, depending on season
                availableWeather = new List<string>() { "clear", "rainy", "foggy" };
                break;
            case "cloudy":
                //  Can stay the same, or switch to clear, rainy or snowy, depending on the season
                availableWeather = new List<string>() { "clear" };
                if (CurrentWeather == "winter") availableWeather.Add("snowy");
                else availableWeather.Add("rainy");
                break;
            case "rainy":
                //  Can stay the same, or switch to foggy or cloudy, depending on the season
                availableWeather = new List<string>() { "rainy", "cloudy", "foggy" };
                break;
            case "snowy":
                // Can stay the same, or switch to rainy, cloudy or foggy, depending on the season
                availableWeather = new List<string>() { "cloudy", "foggy" };
                if (CurrentWeather == "winter") availableWeather.Add("snowy");
                else availableWeather.Add("rainy");
                break;
            case "stormy":
                // can NOT stay the same, must switch to rainy
                availableWeather = new List<string>() { "rainy" };
                break;
            case "foggy":
                // Can stay the same or switch to clear, cloudy, or rainy
                availableWeather = new List<string>() { "cloudy", "foggy" };
                if (CurrentWeather == "winter") availableWeather.Add("snowy");
                else availableWeather.Add("rainy");
                break;
        }

        // TODO: Evaluate if some weathers should be weighted to occur more/less. For now, they are equal
        int weatherRoll = Random.Range(0, availableWeather.Count);
        foreach (string weather in availableWeather)
        {
            weatherRoll -= availableWeather.IndexOf(weather);
            if (weatherRoll < 0)
            {
                CurrentWeather = weather;
                break;
            }
        }

        Debug.Log("Current weather is " + CurrentWeather);
        UiControl.uiControl.UpdateWeatherSprite();
        Save(); // Prevent reload hijinks
    }
}

[System.Serializable]
class PlayerData
{
    // Player Stats
    public float PlayerXp;

    // Inventory
    public Dictionary<string, int> BaitInventory = new Dictionary<string, int>();
    public string SelectedBait;

    // Times
    public DateTime timeOfFirstSave;
    public DateTime timeOfLastSave;

    // Weather
    public string currentWeather;
}
