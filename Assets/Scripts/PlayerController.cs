using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;

public class PlayerController : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector3 Velocity;

    private SpriteRenderer PlayerSprite;

    // 0 - Rod is not out, can move around and interact with objects
    // 1 - Casting
    // 2 - Waiting - Bobber is in water, waiting for fish to bite
    // 3 - Reeling - Bringing in the fish, obviously
    private int PlayerState;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private float Speed;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;

    [SerializeField] private Transform BobberStartPoint;
    [SerializeField] private GameObject Bobber;

    [SerializeField] private Material SpriteGlowMaterial;
    [SerializeField] private Material DefaultSpriteMaterial;

    private GameObject newBobber;

    void Start()
    {
        PlayerSprite = GetComponentInChildren<SpriteRenderer>();    
    }

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        switch(PlayerState)
        {
            // While rod is put away
            case 0:
                // Get distance from all pickup items
                GameObject[] items = GameObject.FindGameObjectsWithTag("PickupItem");
                GameObject closest = null;
                foreach(GameObject item in items)
                {
                    if (closest == null) closest = item;
                    if ((Controller.transform.position - closest.transform.position).magnitude > (Controller.transform.position - item.transform.position).magnitude)
                    {
                        closest = item;
                    }
                    if((Controller.transform.position - closest.transform.position).magnitude < 1)
                    {
                        if (closest.gameObject.activeInHierarchy)
                        {
                            closest.GetComponentInChildren<InteractableHandler>().StartGlow();
                            if (Input.GetKey(KeyCode.E))
                            {
                                // Get loot table for loot point
                                Dictionary<string, int> lootTable = new Dictionary<string, int>();
                                switch (closest.name)
                                {
                                    case "WormDirt":
                                        lootTable = StaticData.Static.WormDirtDropTable;
                                        break;
                                }
                                
                                // Get total weight of loot table
                                int totalLootWeight = 0;
                                foreach (KeyValuePair<string, int> tableItem in lootTable)
                                {
                                    totalLootWeight += tableItem.Value;
                                }

                                // Roll for da loot
                                int lootRoll = Random.Range(0, totalLootWeight);
                                foreach (KeyValuePair<string, int> baitItem in lootTable)
                                {
                                    lootRoll -= baitItem.Value;
                                    if (lootRoll < 0)
                                    {
                                        Debug.Log("Picked up " + baitItem.Key);
                                        if (!GameControl.Control.BaitInventory.ContainsKey(baitItem.Key)) GameControl.Control.BaitInventory.Add(baitItem.Key, 0);
                                        GameControl.Control.BaitInventory[baitItem.Key] += 1;
                                        UiControl.uiControl.BuildBaitInventory();
                                        break;
                                    }
                                }

                                // Pick up, clean up
                                closest.gameObject.SetActive(false);
                                closest.GetComponentInChildren<InteractableHandler>().StopGlow();
                                closest = null;
                                GameControl.Control.Save();
                            }
                        }
                    }
                    else
                    {
                        closest.GetComponentInChildren<InteractableHandler>().StopGlow();
                    }
                }

                // Walk
                MovePlayer();

                // Click mouse to cast
                if (Input.GetKey(KeyCode.Mouse0) && !UiControl.uiControl.BaitSwitchPanel.activeInHierarchy && !UiControl.uiControl.FishDex.activeInHierarchy)
                {
                    // Begin our cast
                    PlayerState = 1;

                    // Get our direction
                    if (PlayerSprite.sprite == frontSprite)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 180f, 0f);
                    }
                    else if (PlayerSprite.sprite == backSprite)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    else if (PlayerSprite.sprite == sideSprite && PlayerSprite.flipX)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 270f, 0f);
                    }
                    else if (PlayerSprite.sprite == sideSprite && !PlayerSprite.flipX) {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 90f, 0f);
                    }

                    newBobber = (GameObject)Instantiate(Bobber, BobberStartPoint.position, new Quaternion(0f, 0f, 0f, 0f));
                    newBobber.GetComponent<Rigidbody>().AddForce(BobberStartPoint.forward * 100f);
                }
                break;
            
            // Casting, bobber is in the air
            case 1:
                PrepToCancelCast();
                break;

            // Waiting, bobber is in the water
            case 2:
                PrepToCancelCast();
                break;

            // Reeling, fish is comin' in hot
            case 3:
                break;
        }
    }

    // Controls player movement and sprite animation
    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(PlayerMovementInput);
        Controller.Move(moveVector * Speed * Time.deltaTime);

        if (PlayerMovementInput.z < -0.1)
        {
            PlayerSprite.sprite = frontSprite;
            PlayerSprite.flipX = false;
        }
        if (PlayerMovementInput.z > 0.1)
        {
            PlayerSprite.sprite = backSprite;
            PlayerSprite.flipX = false;
        }
        if (PlayerMovementInput.x < -0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = true;
        }
        if (PlayerMovementInput.x > 0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = false;
        }
    }

    // When casting or waiting, we can cancel the cast by pressing q
    private void PrepToCancelCast()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            ResetPlayerAndBobber();
        }
    }

    // Resets the bobber after it falls on the ground
    public void ResetBobberFromGround()
    {
        StartCoroutine("BobberDelay");
    }

    // Resets bobber and player state after a delay
    private IEnumerator BobberDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        ResetPlayerAndBobber();
    }

    public void StartWaitingForBite(string currentTile)
    {
        PlayerState = 2;
        StartCoroutine("AwaitFishBite", currentTile);
    }

    private IEnumerator AwaitFishBite(string currentTile)
    {
        Debug.Log("Cast line in " + currentTile + " with " + GameControl.Control.SelectedBait);

        int totalDropWeights = 0;

        // Get fish available for this tile
        Debug.Log("Getting drop rates for cast");
        Dictionary<string, int> fishInTile = new Dictionary<string, int>();
        foreach (FishSpecies species in GameControl.Control.CurrentAvailableFish)
        {
            if(species.baits != null || species.baits.ContainsKey(GameControl.Control.SelectedBait))
            {
                foreach(FishTileData tile in species.tiles)
                {
                    if(tile.tilename == currentTile)
                    {
                        int dropweight = tile.droprate;
                        if (species.seasons != null && species.seasons.ContainsKey(GameControl.Control.CurrentSeason)) dropweight += species.seasons[GameControl.Control.CurrentSeason];
                        if (species.baits != null && species.baits.ContainsKey(GameControl.Control.SelectedBait)) dropweight += species.baits[GameControl.Control.SelectedBait];
                        if(species.hours != null && species.hours.ContainsKey(GameControl.Control.CurrentHour)) dropweight += species.hours[GameControl.Control.CurrentHour];
                        fishInTile.Add(species.species, dropweight);
                        totalDropWeights += dropweight;
                        Debug.Log("Added " + species.species + " to fish list with drop rate of " + dropweight + ". Total dropweight is now " + totalDropWeights);
                    }
                }
            }
        }

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSecondsRealtime(2);
            Debug.Log(i*2 + " seconds have passed");
            int fishRoll = Random.Range(0, totalDropWeights + 1000); // TODO: This number will be calculated somehow later
            foreach (KeyValuePair<string, int> fish in fishInTile)
            {
                fishRoll -= fish.Value;
                if(fishRoll < 0)
                {
                    BeginReeling(fish.Key, currentTile);
                    // TODO: Calculate fish weight
                    yield break;
                }
            }
        }
        BeginReeling("Rough Fish", currentTile);
        GameControl.Control.BaitInventory[GameControl.Control.SelectedBait] -= 1;
        UiControl.uiControl.BuildBaitInventory();
        GameControl.Control.Save();
    }

    private void BeginReeling(string fish, string currentTile)
    {
        Debug.Log("Reeling " + fish);
        PlayerState = 3;
        LandFish(fish, currentTile);
    }

    private void LandFish(string fish, string currentTile)
    {
        Debug.Log(fish + " is caught");
        ResetPlayerAndBobber();
        if (GameControl.Control.UnlockedFishDataList.ContainsKey(fish))
        {
            // Update
            // Tile
            if(!GameControl.Control.UnlockedFishDataList[fish].tiles.Contains(currentTile))
                GameControl.Control.UnlockedFishDataList[fish].tiles.Add(currentTile);
            // Hour
            if (!GameControl.Control.UnlockedFishDataList[fish].hours.Contains(GameControl.Control.CurrentHour))
                GameControl.Control.UnlockedFishDataList[fish].hours.Add(GameControl.Control.CurrentHour);
            // Weather
            if (!GameControl.Control.UnlockedFishDataList[fish].weathers.Contains(GameControl.Control.CurrentWeather))
                GameControl.Control.UnlockedFishDataList[fish].weathers.Add(GameControl.Control.CurrentWeather);
            // Season
            if (!GameControl.Control.UnlockedFishDataList[fish].seasons.Contains(GameControl.Control.CurrentSeason))
                GameControl.Control.UnlockedFishDataList[fish].seasons.Add(GameControl.Control.CurrentSeason);
            // Bait
            if (!GameControl.Control.UnlockedFishDataList[fish].baits.Contains(GameControl.Control.SelectedBait))
                GameControl.Control.UnlockedFishDataList[fish].baits.Add(GameControl.Control.SelectedBait);
        }
        else
        {
            // Add
            UnlockedFishData entry = new UnlockedFishData() {
                tiles = new List<string> { currentTile },
                hours = new List<int> { GameControl.Control.CurrentHour },
                weathers = new List<string> { GameControl.Control.CurrentWeather },
                seasons = new List<string> { GameControl.Control.CurrentSeason },
                baits = new List<string> { GameControl.Control.SelectedBait }
            };
            GameControl.Control.UnlockedFishDataList.Add(fish, entry);
        }

        GameControl.Control.Save();
    }

    private void ResetPlayerAndBobber()
    {
        PlayerState = 0;
        if (newBobber)
        {
            Destroy(newBobber, 0);
        }
        StopCoroutine("AwaitFishBite");
    }
}
