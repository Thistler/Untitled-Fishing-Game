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
    [SerializeField] private GameObject BobberPrefab;

    private GameObject BobberHpBar;
    private GameObject BobberHpBarFill; // TODO: Might want to change the typing on these
    private GameObject InstantiatedBobber;

    [SerializeField] private GameObject TensionHpBar;
    [SerializeField] private GameObject TensionHpBarFill;

    [SerializeField] private GameObject Arrow;

    private FishSpecies CurrentHookedFish;
    private string CurrentFishingTile;
    private float TotalFishHp = 5000;
    private float CurrentFishHp = 5000; // TODO: this will be set by fish.
    private float LineTension;
    private string ActiveDirection;
    private int ActiveDirectionCounter;
    private bool TimeToStrike;
    private float StrikeTimer;
    private bool FishIsPulling;

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
                if (Input.GetKeyDown(KeyCode.Mouse0) && !UiControl.uiControl.BaitSwitchPanel.activeInHierarchy && !UiControl.uiControl.FishDex.activeInHierarchy)
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

                    InstantiatedBobber = GameObject.Instantiate(BobberPrefab, BobberStartPoint.position, new Quaternion(0f, 0f, 0f, 0f));
                    InstantiatedBobber.GetComponent<Rigidbody>().AddForce(BobberStartPoint.forward * 100f);
                    BobberHpBar = InstantiatedBobber.transform.Find("HpBar").gameObject;
                    BobberHpBarFill = BobberHpBar.transform.Find("HpBarFill").gameObject;
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
                // Attempt to strike
                if(TimeToStrike)
                {
                    if(Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        TimeToStrike = false;
                        CurrentFishHp -= 1000; // TODO: Recalculate based on player level
                        break;
                    }
                    StrikeTimer += Time.deltaTime;
                    if (StrikeTimer > 2.0f) TimeToStrike = false; // TODO: Recalculate based on player xp
                }

                // Determine if player is holding the button in the right direction
                bool pushingCorrectDirection = false;
                switch (ActiveDirection)
                {
                    case "up":
                        if(PlayerMovementInput.z > 0.1 && PlayerMovementInput.x == 0) pushingCorrectDirection = true;
                        break;
                    case "down":
                        if (PlayerMovementInput.z < -0.1 && PlayerMovementInput.x == 0) pushingCorrectDirection = true;
                        break;
                    case "left":
                        if (PlayerMovementInput.x < -0.3 && PlayerMovementInput.z == 0) pushingCorrectDirection = true;
                        break;
                    case "right":
                        if (PlayerMovementInput.x > 0.3 && PlayerMovementInput.z == 0) pushingCorrectDirection = true;
                        break;
                }

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    CurrentFishHp--; // TODO: Different formula for determining how much the hp decreases
                    if(FishIsPulling)
                    {
                        if (pushingCorrectDirection) LineTension += 2;
                        else LineTension += 3;
                    }
                    else LineTension++;
                }
                else
                {
                    if(FishIsPulling)
                    {
                        if (pushingCorrectDirection) LineTension--;
                        else LineTension++;
                    }
                    else
                    {
                        LineTension--;
                    }
                }

                if (LineTension < 0)
                {
                    LineTension = 0;
                }

                // Handle bars
                float tensionPercent = LineTension / 1000; // TODO: Should this be a separate value increased by leveling up?
                // Keep scale and tension percent separate so bar doesn't overflow and we can save tension percent to calculate line break
                float tensionScaleX = tensionPercent;
                if (tensionScaleX > 1) tensionScaleX = 1;
                Vector3 tensionScale = new Vector3(tensionScaleX, TensionHpBarFill.transform.localScale.y, TensionHpBarFill.transform.localScale.z);
                TensionHpBarFill.transform.localScale = tensionScale;
                float percent = CurrentFishHp / TotalFishHp;
                Vector3 scale = new Vector3(percent, BobberHpBarFill.transform.localScale.y, BobberHpBarFill.transform.localScale.z);
                BobberHpBarFill.transform.localScale = scale;

                // Land or escape
                if (CurrentFishHp <= 0)
                {
                    LandFish();
                }
                else if (tensionPercent > 1)
                {
                    // Roll for chance of line breaking
                    float breakRoll = Random.Range(0, (tensionPercent * 100));
                    if (breakRoll > 100) EscapeFish(); // TODO: Put this in a couroutine so it doesn't fire every frame, or maybe make sure the line has to be at full tension for at least 0.5-1 sec before breaking
                }
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

    public void StartWaitingForBite(string tile)
    {
        PlayerState = 2;
        CurrentFishingTile = tile;
        StartCoroutine("AwaitFishBite");
    }

    private IEnumerator AwaitFishBite()
    {
        Debug.Log("Cast line in " + CurrentFishingTile + " with " + GameControl.Control.SelectedBait);

        int totalDropWeights = 0;

        // Get fish available for this tile
        Debug.Log("Getting drop rates for cast");
        Dictionary<FishSpecies, int> fishInTile = new Dictionary<FishSpecies, int>();
        foreach (FishSpecies species in GameControl.Control.CurrentAvailableFish)
        {
            if(species.baits == null || species.baits.ContainsKey(GameControl.Control.SelectedBait))
            {
                foreach(FishTileData tile in species.tiles)
                {
                    if(tile.tilename == CurrentFishingTile)
                    {
                        int dropweight = tile.droprate;
                        if (species.seasons != null && species.seasons.ContainsKey(GameControl.Control.CurrentSeason)) dropweight += species.seasons[GameControl.Control.CurrentSeason];
                        if (species.baits != null && species.baits.ContainsKey(GameControl.Control.SelectedBait)) dropweight += species.baits[GameControl.Control.SelectedBait];
                        if(species.hours != null && species.hours.ContainsKey(GameControl.Control.CurrentHour)) dropweight += species.hours[GameControl.Control.CurrentHour];
                        fishInTile.Add(species, dropweight);
                        totalDropWeights += dropweight;
                        Debug.Log("Added " + species.species + " to fish list with drop rate of " + dropweight + ". Total dropweight is now " + totalDropWeights);
                    }
                }
            }
        }

        for (int i = 0; i < 10; i++) // TODO: Bump back to 30
        {
            yield return new WaitForSecondsRealtime(2);
            Debug.Log(i*2 + " seconds have passed");
            int fishRoll = Random.Range(0, totalDropWeights + 1000); // TODO: This number will be calculated somehow later
            foreach (KeyValuePair<FishSpecies, int> fish in fishInTile)
            {
                fishRoll -= fish.Value;
                if(fishRoll < 0)
                {
                    CurrentHookedFish = fish.Key;
                    BeginReeling();
                    // TODO: Calculate fish weight
                    yield break;
                }
            }
        }
        // If no fish are caught, recieve Rough Fish
        CurrentHookedFish = StaticData.Static.FullFishSpeciesList[0];

        // Prepare to reel
        Arrow.SetActive(false);
        BobberHpBar.SetActive(true);
        TensionHpBar.SetActive(true);
        ActiveDirection = "";
        BeginReeling();

        // Remove bait and save
        GameControl.Control.BaitInventory[GameControl.Control.SelectedBait] -= 1;
        UiControl.uiControl.BuildBaitInventory();
        GameControl.Control.Save();
    }

    private void BeginReeling()
    {
        Debug.Log("Reeling " + CurrentHookedFish.species);
        PlayerState = 3;
        InstantiatedBobber.GetComponent<AudioSource>().Play();
        StartCoroutine("ActivatePullDirection");
        TimeToStrike = true;
        StrikeTimer = 0;
    }

    private IEnumerator ActivatePullDirection()
    {
        while(PlayerState == 3) // TODO: Change this to somethng that allows us to get the fish to stop pulling sooner
        {
            yield return new WaitForSecondsRealtime(10); // TODO: Randomize

            Arrow.transform.SetPositionAndRotation(Arrow.transform.position, new Quaternion(0, 0, 0, 0));
            Arrow.SetActive(!Arrow.activeInHierarchy);

            if (Arrow.activeInHierarchy == true)
            {
                FishIsPulling = true;

                int direction = Random.Range(0, 4);
                switch (direction)
                {
                    // Up
                    case 0:
                        ActiveDirection = "up";
                        break;
                    // Down
                    case 1:
                        ActiveDirection = "down";
                        Arrow.transform.Rotate(new Vector3(0, 0, 180.0f));
                        break;
                    // Left
                    case 2:
                        ActiveDirection = "left";
                        Arrow.transform.Rotate(new Vector3(0, 0, 90.0f));
                        break;
                    // Right
                    case 3:
                        ActiveDirection = "right";
                        Arrow.transform.Rotate(new Vector3(0, 0, 270.0f));
                        break;
                }
            }
            else
            {
                FishIsPulling = false;
            }
        }
    }

    private void LandFish()
    {
        Debug.Log(CurrentHookedFish.species + " is caught");
        ResetPlayerAndBobber();

        // Update fish dex
        if (GameControl.Control.UnlockedFishDataList.ContainsKey(CurrentHookedFish.species))
        {
            // Update
            // Tile
            if(!GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].tiles.Contains(CurrentFishingTile))
                GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].tiles.Add(CurrentFishingTile);
            // Hour
            if (!GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].hours.Contains(GameControl.Control.CurrentHour))
                GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].hours.Add(GameControl.Control.CurrentHour);
            // Weather
            if (!GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].weathers.Contains(GameControl.Control.CurrentWeather))
                GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].weathers.Add(GameControl.Control.CurrentWeather);
            // Season
            if (!GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].seasons.Contains(GameControl.Control.CurrentSeason))
                GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].seasons.Add(GameControl.Control.CurrentSeason);
            // Bait
            if (!GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].baits.Contains(GameControl.Control.SelectedBait))
                GameControl.Control.UnlockedFishDataList[CurrentHookedFish.species].baits.Add(GameControl.Control.SelectedBait);
        }
        else
        {
            // Add
            UnlockedFishData entry = new UnlockedFishData() {
                tiles = new List<string> { CurrentFishingTile },
                hours = new List<int> { GameControl.Control.CurrentHour },
                weathers = new List<string> { GameControl.Control.CurrentWeather },
                seasons = new List<string> { GameControl.Control.CurrentSeason },
                baits = new List<string> { GameControl.Control.SelectedBait }
            };
            GameControl.Control.UnlockedFishDataList.Add(CurrentHookedFish.species, entry);
        }

        GameControl.Control.Save();
    }

    private void EscapeFish()
    {
        Debug.Log(CurrentHookedFish.species + " escaped");
        ResetPlayerAndBobber();
        GameControl.Control.Save();
    }

    private void ResetPlayerAndBobber()
    {
        PlayerState = 0;
        if (InstantiatedBobber)
        {
            Destroy(InstantiatedBobber, 0);
        }
        StopCoroutine("AwaitFishBite");
        StopCoroutine("ActivatePullDirection");
        TensionHpBar.SetActive(false);
        LineTension = 0;
        CurrentFishHp = TotalFishHp; // TODO: This should probably be moved since fish hp will vary
    }
}
