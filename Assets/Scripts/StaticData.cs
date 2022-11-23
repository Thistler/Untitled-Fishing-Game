using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{
    public static StaticData Static;

    public int[] LevelXpThresholds;

    public List<FishSpecies> FullFishSpeciesList;
    
    public Sprite[] FishSpritesArray;
    public Sprite[] BaitSpritesArray;

    public Dictionary<string, Sprite> BaitSpritesDictionary;
    
    // Drop table for lootable bait points
    public Dictionary<string, int> WormDirtDropTable;

    void Awake()
    {
        if (Static == null)
        {
            DontDestroyOnLoad(gameObject);
            Static = this;

            LevelXpThresholds = new int[]
            {
                0, 10, 20, 50, 100, 200, 400, 700, 1000, 2000
            };

            // FISH LIST
            FullFishSpeciesList = new List<FishSpecies>();
            // Rough fish must be listed first
            // Rough fish's data is not actually used, as it is the fail fish, but tile data must be definied to stop it from showing up in the regular loot table
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Rough Fish",
                sprite = FishSpritesArray[3], // TODO: This should be 0 eventually
                tiles = new List<FishTileData> { new FishTileData { tilename = "nothing", droprate = 0, weightbonus = 0 } },
                weathers = null,
                seasons = null,
                hours = null,
                baits = null,
                fishBaseHp = 3000,
                fishBaseStregnth = 1,
                fishBaseXp = 5 // TODO: TEmp
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Aqua Bass",
                sprite = FishSpritesArray[1],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 10, weightbonus = 0 },
                    new FishTileData{ tilename = "cabin_pond_deep", droprate = 80, weightbonus = 10 } },
                weathers = new Dictionary<string, int> { { "clear", 10 } },
                seasons = new Dictionary<string, int> { { "spring", 0 }, { "summer", 10 } },
                hours = new Dictionary<int, int> { { 3, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 }, { 13, 0 }, { 14, 0 }, { 15, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 } },
                fishBaseHp = 5000,
                fishBaseStregnth = 1,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Cloud Trout",
                sprite = FishSpritesArray[2],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 20, weightbonus = 0 } },
                weathers = new Dictionary<string, int> { { "cloudy", 30 }, { "rainy", 40 }, { "stormy", 60 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 0 } },
                fishBaseHp = 6000,
                fishBaseStregnth = 2,
                fishBaseXp = 5
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Mud Crawdad",
                sprite = FishSpritesArray[2],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "cabin_pond_shallow", droprate = 40, weightbonus = 0 } },
                weathers = null,
                seasons = new Dictionary<string, int> { { "spring", 10 }, { "winter", 20 } },
                hours = new Dictionary<int, int> { { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 18, 0 }, { 19, 0 }, { 20, 0 }, { 21, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 }, { "grub", 0 } },
                fishBaseHp = 6000,
                fishBaseStregnth = 2,
                fishBaseXp = 5
            });

            // BAIT LIST
            BaitSpritesDictionary = new Dictionary<string, Sprite>() {
                { "worm", BaitSpritesArray[0] },
                { "grub", BaitSpritesArray[1] }
            };

            // BAIT TABLES
            WormDirtDropTable = new Dictionary<string, int>()
            {
                { "worm", 70 },
                { "grub", 20 }
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

        // HP represents how fast the catch bar (green bar above bobber) gets depleted
        // Strength represents how fast the tension goes up
        public int fishBaseHp;
        public int fishBaseStregnth;
        public int fishBaseXp; // Eventually these may be affected by multipliers/weight

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
