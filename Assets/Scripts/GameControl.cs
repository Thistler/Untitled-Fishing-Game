using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using static StaticData;
using System.Linq;

public class GameControl : MonoBehaviour
{
    public static GameControl Control;

    // Saveable data
    // Player Stats
    public float PlayerXp;
    
    // Inventory
    public Dictionary<string, int> BaitInventory = new Dictionary<string, int>();

    // Time since last save
    public System.DateTime TimeOfLastSave;

    // Non saveable data
    public List<FishSpecies> CurrentAvailableFish;

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
        var currentTime = System.DateTime.UtcNow;

        // Set data to save
        PlayerData data = new PlayerData();

        // Player Data
        data.PlayerXp = PlayerXp;
        
        // Inventory
        data.BaitInventory = BaitInventory;

        // Time Since Last Save
        data.timeOfLastSave = currentTime;

        // Save and close file
        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        Debug.Log("Loading from " + Application.persistentDataPath);
        int minutesSinceLastSave = 0;

        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            // Load existing data
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            // Calculate minutes since last save
            TimeOfLastSave = data.timeOfLastSave;

            System.DateTime currentTime = System.DateTime.UtcNow;
            System.TimeSpan timeDifference = currentTime - TimeOfLastSave;
            minutesSinceLastSave = (int)timeDifference.TotalMinutes;

            Debug.Log(minutesSinceLastSave + " minutes have passed since closing the game.");

            // Player Stats
            PlayerXp = data.PlayerXp;
            
            // Inventory
            BaitInventory = data.BaitInventory;
        }
        else
        {
            // Create new game
            Debug.Log("Creating a new game.");
            
            // No growth should be added when creating a new game
            minutesSinceLastSave = 0;
        }

        SetCurrentFishList();
    }

    // Sets list of fish currently available based on current season, weather, and time of day
    public void SetCurrentFishList()
    {
        // TODO: Temp
        string currentSeason = "spring";
        int currentHour = 10;
        string currentWeather = "clear";
        ////////

        Debug.Log("Setting fish list");
        CurrentAvailableFish = new List<FishSpecies>();
        foreach (FishSpecies species in StaticData.Static.FullFishSpeciesList)
        {
            // If seasons, hours, or weathers is null, that means the fish is not restricted by those fields.
            // So we make sure that the field is either null or matches the current value for those fields
            if ((species.seasons == null || species.seasons.ContainsKey(currentSeason))
                && (species.hours == null || species.hours.ContainsKey(currentHour))
                && (species.weathers == null || species.weathers.ContainsKey(currentWeather))
                )
            {
                CurrentAvailableFish.Add(species);
                Debug.Log("Added " + species.species + " to fish list");
            }
        }
    }
}

[System.Serializable]
class PlayerData
{
    // Player Stats
    public float PlayerXp;

    // Inventory
    public Dictionary<string, int> BaitInventory = new Dictionary<string, int>();

    // Time since last save
    public DateTime timeOfLastSave;
}
