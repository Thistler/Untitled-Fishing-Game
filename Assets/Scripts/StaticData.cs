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
    public Dictionary<string, int> CabinDirtDropTable;
    public Dictionary<string, int> CabinPebbleDropTable;
    public Dictionary<string, int> CabinLeavesDropTable;

    void Awake()
    {
        if (Static == null)
        {
            DontDestroyOnLoad(gameObject);
            Static = this;

            LevelXpThresholds = new int[]
            {
                // TODO: This is sloppy but it's November 27, so what can ya do?
                0, 10, 20, 50, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            };

            // FISH LIST
            FullFishSpeciesList = new List<FishSpecies>();
            // Rough fish must be listed first
            // Rough fish's data is not actually used, as it is the fail fish, but tile data must be definied to stop it from showing up in the regular loot table
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Rough Fish",
                sprite = FishSpritesArray[0],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 0, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 0, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 0, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 0, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 0, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 0, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 0 }, { "grub", 0 }, { "crawler", 0 }, { "pillbug", 0 }, { "grasshopper", 0 }, { "locust", 0 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 1
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Aqua Bass",
                sprite = FishSpritesArray[1],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "pond_shallow", droprate = 10, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 80, weightbonus = 10 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                },
                weathers = new Dictionary<string, int> { { "clear", 10 } },
                seasons = new Dictionary<string, int> { { "spring", 0 }, { "summer", 10 } },
                hours = new Dictionary<int, int> { { 10, 0 }, { 11, 0 }, { 12, 0 }, { 13, 0 }, { 14, 0 }, { 15, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 } },
                fishBaseHp = 5000,
                fishBaseStrength = 1,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Mud Crawdad",
                sprite = FishSpritesArray[2],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 40, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 40, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 40, weightbonus = 0 } },
                weathers = null,
                seasons = new Dictionary<string, int> { { "spring", 10 }, { "winter", 20 } },
                hours = new Dictionary<int, int> { { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 18, 0 }, { 19, 0 }, { 20, 0 }, { 21, 0 } },
                baits = new Dictionary<string, int> { { "worm", 0 }, { "grub", 0 } },
                fishBaseHp = 6000,
                fishBaseStrength = 2,
                fishBaseXp = 5
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Cloud Trout",
                sprite = FishSpritesArray[3],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "pond_deep", droprate = 20, weightbonus = 0 }
                },
                weathers = new Dictionary<string, int> { { "cloudy", 30 }, { "rainy", 40 }, { "stormy", 60 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 0 } },
                fishBaseHp = 6000,
                fishBaseStrength = 4,
                fishBaseXp = 5
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Pinky Shrimp",
                sprite = FishSpritesArray[4],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 40, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 20, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 40, weightbonus = 0 }
                },
                weathers = null,
                seasons = new Dictionary<string, int> { { "spring", 30 }, { "summer", 20 } },
                hours = null,
                baits = new Dictionary<string, int> { { "crawler", 10 }, { "pillbug", 20 } },
                fishBaseHp = 4000,
                fishBaseStrength = 2,
                fishBaseXp = 5
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Blue Loach",
                sprite = FishSpritesArray[5],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 40, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 20, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 20, weightbonus = 0 },
                },
                weathers = null,
                seasons = new Dictionary<string, int> { { "fall", 30 }, { "winter", 20 } },
                hours = null,
                baits = new Dictionary<string, int> { { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Red General",
                sprite = FishSpritesArray[6],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 20, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 20, weightbonus = 0 }
                },
                weathers = null,
                seasons = new Dictionary<string, int> { { "spring", 30 }, { "winter", 40} },
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 3,
                fishBaseXp = 6
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Bright Bass",
                sprite = FishSpritesArray[7],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 40, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 20, weightbonus = 0 }
                },
                weathers = null,
                seasons = new Dictionary<string, int> { { "summer", 30 }, { "fall", 40 } },
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 3,
                fishBaseXp = 6
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Concord Catfish",
                sprite = FishSpritesArray[8],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "pond_deep", droprate = 40, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 8000,
                fishBaseStrength = 6,
                fishBaseXp = 6
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Maned Gar",
                sprite = FishSpritesArray[9],
                tiles = new List<FishTileData> {
                     new FishTileData{ tilename = "pond_shallow", droprate = 20, weightbonus = 0 },
                     new FishTileData{ tilename = "pond_deep", droprate = 20, weightbonus = 0 },
                     new FishTileData{ tilename = "reeds", droprate = 20, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 2,
                fishBaseXp = 4
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Simple Grey",
                sprite = FishSpritesArray[10],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = new Dictionary<string, int> { { "cloudy", 30 }, { "rainy", 40 }, { "stormy", 60 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 4
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Simple Sandy",
                sprite = FishSpritesArray[11],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = new Dictionary<string, int> { { "clear", 30 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 4
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Blue Guppy",
                sprite = FishSpritesArray[12],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "pond_shallow", droprate = 40, weightbonus = 0 },
                },
                weathers = new Dictionary<string, int> { { "clear", 30 }, { "cloudy", 20 }, { "snowy", 10 } },
                seasons = null,
                hours = new Dictionary<int, int> { { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 } },
                baits = new Dictionary<string, int> { { "worm", 10 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Citrus Seedler",
                sprite = FishSpritesArray[13],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = new Dictionary<string, int> { { "clear", 30 }, { "cloudy", 20 }, { "snowy", 10 } },
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 1,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Mini Loach",
                sprite = FishSpritesArray[14],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 1
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Koi Goldfish",
                sprite = FishSpritesArray[15],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "waterfall_pond", droprate = 40, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "pillbug", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 3,
                fishBaseXp = 6
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Starry Nightfish",
                sprite = FishSpritesArray[16],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "waterfall_pond", droprate = 40, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 22, 0 }, { 23, 0 } },
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 } },
                fishBaseHp = 6000,
                fishBaseStrength = 3,
                fishBaseXp = 6
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Sun Walleye",
                sprite = FishSpritesArray[17],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grub", 10 }, { "pillbug", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Longnose Sturgeon",
                sprite = FishSpritesArray[18],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Whiskered Pleco",
                sprite = FishSpritesArray[19],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grasshopper", 10 }, { "locust", 20 } },
                fishBaseHp = 6000,
                fishBaseStrength = 5,
                fishBaseXp = 7
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Brown Crab",
                sprite = FishSpritesArray[20],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grub", 10 }, { "pillbug", 20 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Simple Greenfish",
                sprite = FishSpritesArray[21],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Bull Bass",
                sprite = FishSpritesArray[22],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grub", 10 } },
                fishBaseHp = 3000,
                fishBaseStrength = 2,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Spikey Gar",
                sprite = FishSpritesArray[23],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grub", 10 }, { "pillbug", 20 }, { "locust", 20 } },
                fishBaseHp = 4000,
                fishBaseStrength = 2,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Horrifying Purple Thing",
                sprite = FishSpritesArray[24],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 10, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 10, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 3000,
                fishBaseStrength = 3,
                fishBaseXp = 3
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Radiant Sunburst",
                sprite = FishSpritesArray[25],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 2,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Dotty Pleco",
                sprite = FishSpritesArray[26],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "algae", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_shallow", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "pond_deep", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 2,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Spinetail Guppy",
                sprite = FishSpritesArray[27],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "pond_shallow", droprate = 10, weightbonus = 0 },
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Blue Elegant",
                sprite = FishSpritesArray[28],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "reeds", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "worm", 10 }, { "grub", 10 }, { "crawler", 10 }, { "pillbug", 20 }, { "grasshopper", 20 }, { "locust", 20 } },
                fishBaseHp = 2000,
                fishBaseStrength = 1,
                fishBaseXp = 2
            });
            FullFishSpeciesList.Add(new FishSpecies
            {
                species = "Chompy Bitefish",
                sprite = FishSpritesArray[28],
                tiles = new List<FishTileData> {
                    new FishTileData{ tilename = "creek", droprate = 30, weightbonus = 0 },
                    new FishTileData{ tilename = "waterfall_pond", droprate = 30, weightbonus = 0 }
                },
                weathers = null,
                seasons = null,
                hours = null,
                baits = new Dictionary<string, int> { { "grub", 10 }, { "locust", 20 } },
                fishBaseHp = 4000,
                fishBaseStrength = 6,
                fishBaseXp = 8
            });

            // BAIT LIST
            BaitSpritesDictionary = new Dictionary<string, Sprite>() {
                { "worm", BaitSpritesArray[0] },
                { "grub", BaitSpritesArray[1] },
                { "crawler", BaitSpritesArray[2] },
                { "pillbug", BaitSpritesArray[3] },
                { "grasshopper", BaitSpritesArray[4] },
                { "locust", BaitSpritesArray[5] }
            };

            // BAIT TABLES
            CabinDirtDropTable = new Dictionary<string, int>()
            {
                { "worm", 70 },
                { "grub", 20 }
            };
            CabinPebbleDropTable = new Dictionary<string, int>()
            {
                { "crawler", 70 },
                { "pillbug", 20 }
            };
            CabinLeavesDropTable = new Dictionary<string, int>()
            {
                { "grasshopper", 70 },
                { "locust", 20 }
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
        public int fishBaseStrength;
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
