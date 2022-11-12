using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    public static StaticData Static;

    public List<FishSpecies> FullFishSpeciesList;
    
    public Sprite[] FishSpritesArray;
    public Sprite[] BaitSpritesArray;

    public Dictionary<string, Sprite> BaitSpritesDictionary;

    void Awake()
    {
        if (Static == null)
        {
            DontDestroyOnLoad(gameObject);
            Static = this;

            // FISH LIST
            FullFishSpeciesList = new List<FishSpecies>();
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Aqua Bass",
                sprite = FishSpritesArray[0],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 10, weightbonus = 0 },
                    new FishTileData{ tilename = "cabin_pond_deep", droprate = 80, weightbonus = 10 } },
                weathers = new Dictionary<string, int> { { "clear", 10 } },
                seasons = new Dictionary<string, int> { { "spring", 0 }, { "summer", 10 } },
                hours = new Dictionary<int, int> { { 3, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 }, { 13, 0 }, { 14, 0 }, { 15, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 } }
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Cloud Trout",
                sprite = FishSpritesArray[1],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 20, weightbonus = 0 } },
                weathers = new Dictionary<string, int> { { "cloudy", 30 }, { "rainy", 40 }, { "stormy", 60 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 0 } }
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Mud Crawdad",
                sprite = FishSpritesArray[2],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 40, weightbonus = 0 } },
                weathers = null,
                seasons = new Dictionary<string, int> { { "fall", 10 }, { "winter", 20 } },
                hours = new Dictionary<int, int> { { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 18, 0 }, { 19, 0 }, { 20, 0 }, { 21, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 }, { "grub", 0 } }
            });

            // BAIT LIST
            BaitSpritesDictionary = new Dictionary<string, Sprite>() {
                { "worm", BaitSpritesArray[0] },
                { "grub", BaitSpritesArray[1] }
            };
        }
        else if (Static != this)
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class FishSpecies
    {
        public string species;
        public Sprite sprite;

        // DROP RATE is affected by tiles, weather, hour, and bait
        // FISH WEIGHT is affected by tiles, season, and bait

        // Name of tile and BASE DROP RATE for that tile. TODO: Should this be a class or other type so we can have drop rate AND weight bonus?
        public List<FishTileData> tiles;

        // Name of weather and DROP RATE bonus
        public Dictionary<string, int> weathers;

        // Name of season and WEIGHT bonus // TODO: Should this also affect drop rate?
        public Dictionary<string, int> seasons;

        // List of hours (0 - 23) that fish is available and DROP RATE bonus
        public Dictionary<int, int> hours;

        // Name of bait and WEIGHT bonus
        public Dictionary<string, int> baits;
    }

    [System.Serializable]
    public class FishTileData
    {
        public string tilename;
        public int droprate;
        public int weightbonus;
    }
}
