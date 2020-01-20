using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GameController : MonoBehaviour
{
    #region Game variables

    //Public variables that directly affect gameplay. Set within the inspector for ease of testing.

    public int enemyAmount, maxLives, rockets, rocketCapacity, maxRocketCapacity;

    public float maxHealth, playerRespawnTime, roundTime, shieldRate, minShieldRate, shieldCapacity, maxShieldCapacity;

    #endregion

    #region Game object references

    //References set in inspector.

    public GameObject enemy, player, splashScreen, pauseScreen, gameOverScreen;

    public Vector3 spawnValues;

    public Transform playerSpawn;

    public Button gameOverQuitButton, pauseQuitButton;

    public Text coinsText, timeText, livesText, maxLivesText, healthText, maxHealthText, killsText, rocketsText, rocketCapacityText,
                shieldText, shieldCapacityText, subText, mainText, scoreText, hiScoreText, pauseText;

    public AudioClip coin;

    #endregion

    #region Game private variables

    //Private variables that depend on other game components and should only be modified within this script.

    private int coins, kills, lives, round, enemiesToSpawn;

    private float score, roundTimeCounter, enemySpawnTime;

    private bool playerDamaged;

    private enum GameStates //Game states will determine game elements' activity.
    {
        GameStart,
        Pause,
        RoundStart,
        Round,
        RoundEnd,
        Shopping,
        GameOver
    }

    private GameStates gameState;

    private string hiScorePath = Path.Combine(Application.streamingAssetsPath, "hiscore.txt"); //The folder location of the hiscore file. A simple txt file will do for this game.

    private Dictionary<GameStates, string> mainTexts = new Dictionary<GameStates, string>() //We sync main texts with Game States so that they change accordingly.
    {
        { GameStates.GameStart, "Arcade Free for All Space Battle!" },
        { GameStates.Pause, "Pause" },
        { GameStates.RoundStart, "Round"},
        { GameStates.Round, ""},
        { GameStates.RoundEnd, "Round over!"},
        { GameStates.Shopping, ""},
        { GameStates.GameOver, "Game Over"}
    };

    private Dictionary<GameStates, string> subTexts = new Dictionary<GameStates, string>() //This way, text values are already stored and don't need to be created on runtime.
    {
        { GameStates.GameStart, "Press any key to start" },
        { GameStates.Pause, "" },
        { GameStates.RoundStart, "Are you ready?"},
        { GameStates.Round, ""},
        { GameStates.RoundEnd, "You did great!"},
        { GameStates.Shopping, ""},
        { GameStates.GameOver, ""}
    };

    #endregion

    #region Shop variables

    public GameObject shop; //The parent game object for all others.
    public int lifePrice, healthPrice, rocketsPrice, rocketCapacityPrice, shieldRatePrice, shieldCapacityPrice;
    public Button lifeButton, healthButton, rocketsButton, rocketCapacityButton, shieldRateButton, shieldCapacityButton;
    public GameObject lifeCheck, healthCheck, rocketsCheck, rocketCapacityCheck, shieldRateCheck, shieldCapacityCheck;
    public Text lifePriceText, healthPriceText, rocketsPriceText, rocketCapacityPriceText, shieldRatePriceText, shieldCapacityPriceText;

    #endregion

    #region Game

    void Start()
    {
        GameControl.SetGameController(this); //Set reference to static class, which is used to mediate between this script and the rest of the game.

        GameControl.SetAudioSource(GetComponent<AudioSource>()); //Set static class' reference to the one and only audio source (it will do for this game).

        lives = maxLives;
        rockets = rocketCapacity;
        score = 0;
        kills = 0;

        UpdateNumText(livesText, lives); //Update UI nummeric value texts.
        UpdateNumText(maxLivesText, maxLives);
        UpdateNumText(coinsText, coins);
        UpdateNumText(scoreText, score);
        UpdateNumText(rocketsText, rockets);
        UpdateNumText(rocketCapacityText, rocketCapacity);
        UpdateNumText(shieldCapacityText, shieldCapacity);
        UpdateNumText(killsText, kills);
        UpdateNumText(maxHealthText, maxHealth);


        lifePriceText.text = lifePrice + " coins"; //Set up shop texts according to prices set in inspector.
        healthPriceText.text = healthPrice + " coins";
        rocketsPriceText.text = rocketsPrice + " coins"; //We set them up this simple way because prices do not increase as per current version.
        rocketCapacityPriceText.text = rocketCapacityPrice + " coins";
        shieldRatePriceText.text = shieldRatePrice + " coins";
        shieldCapacityPriceText.text = shieldCapacityPrice + " coins";

        gameOverQuitButton.interactable = false;
        pauseQuitButton.interactable = false;

        shop.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        splashScreen.SetActive(true);

        ChangeState(GameStates.GameStart); //The first game state, from which all others derive.

        if (File.Exists(hiScorePath)) //Read hi score from file and update accordingly if it exists.
        {
            StreamReader reader = new StreamReader(hiScorePath);
            hiScoreText.text = "Highest score: " + reader.ReadLine();
            reader.Close();
        }
        else
        {
            hiScoreText.text = "";
        }

        SpawnPlayer(); //Ready player 1.

        GameControl.CallGameFreeze();
    }

    void Update()
    {
        switch (gameState)
        {
            case GameStates.GameStart: //Start the game upon any key press.

                if (Input.anyKeyDown)
                {
                    RoundStart();
                    splashScreen.SetActive(false);
                }
                break;
            case GameStates.Round: //Keep track of round time and update its display.

                roundTimeCounter += Time.deltaTime;

                float timeDiff = roundTime - roundTimeCounter;

                if (timeDiff >= 0)
                {
                    UpdateNumText(timeText, timeDiff);
                }
                else
                {
                    UpdateNumText(timeText, 0.0f); //Time is never negative.
                }

                if (Input.GetKeyDown(KeyCode.P)) //Pause the game upon P key down.
                {
                    Pause();
                }
                break;
        }
    }


    void ChangeState(GameStates anotherGameState) //Change states and update texts.
    {
        gameState = anotherGameState;

        mainText.text = mainTexts[gameState];

        subText.text = subTexts[gameState];

        switch (gameState)
        {
            case GameStates.GameStart:
                pauseText.text = "";
                break;
            case GameStates.Pause:
                pauseText.text = "";
                break;
            case GameStates.RoundStart:
                mainText.text += " " + round;
                break;
            case GameStates.Round:
                pauseText.text = "Press 'P' to pause";
                break;
            case GameStates.RoundEnd:
                pauseText.text = "";
                break;
            case GameStates.Shopping:
                break;
            case GameStates.GameOver:
                pauseText.text = "";
                break;
        }
    }

    public void Pause() //Called when 'P' key is pressed during round state.
    {
        ChangeState(GameStates.Pause);

        pauseScreen.SetActive(true);

        if (!Application.isEditor) //No quitting when playing in the editor.
        {
            pauseQuitButton.interactable = true;
        }

        GameControl.CallGameFreeze(); //Game objects subscribed to this event will stop doing things.
    }

    void Resume() //Called through the 'Resume' button in the In-Game Menu.
    {
        pauseScreen.SetActive(false);

        ChangeState(GameStates.Round);

        GameControl.CallGameFree();
    }

    void RoundStart() //Sets everything for a new round.
    {
        roundTimeCounter = 0;

        round++;

        UpdateNumText(timeText, roundTime);

        enemiesToSpawn = enemyAmount;

        ChangeState(GameStates.RoundStart);

        StartCoroutine(Round());

        GameControl.CallGameFree();
    }
	
	IEnumerator Round () //Spawns a certain amount of enemies at regular intervals.
	{
        enemySpawnTime = roundTime / enemyAmount;

        yield return new WaitForSeconds (5); //Five seconds are given at the start of each round.

        ChangeState(GameStates.Round);

        while (enemiesToSpawn > 0)
        {
            while (gameState == GameStates.GameOver || gameState == GameStates.Pause)
            {
                yield return null; //Do nothing if the game is over or paused.
            }

            Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);

            Quaternion spawnRotation = Quaternion.identity; //Spawn positions are randomly generated.

            Instantiate(enemy, spawnPosition, spawnRotation);

            enemiesToSpawn--;

            yield return new WaitForSeconds(enemySpawnTime);
        }

        if (gameState == GameStates.Round) //The round is ended after the last enemy is spawned.
        {
            ChangeState(GameStates.RoundEnd);

            enemyAmount *= 2;
            enemiesToSpawn = enemyAmount;

            UpdateHiScore();

            yield return new WaitForSeconds(5); //Wait five seconds before proceeding to the shop.

            ShopSetUp();
        }
    }

    public void PlayerDown() //Update lives upon player death.
    {
        lives--;

        if (lives > 0) //Respawn if there are lives left, otherwise end the game.
        {
            StartCoroutine(Respawn());

            playerDamaged = false;
        }
        else
        {
            lives = 0;

            GameOver();
        }

        UpdateNumText(livesText, lives);
    }

    public void EnemyDown(float shipScore) //Updates kills and score upon destroying an enemy ship.
    {
        kills++;
        score += shipScore;
        UpdateNumText(scoreText, score);
        UpdateNumText(killsText, kills);
    }

    IEnumerator Respawn() //Just a simple timer for respawn time, separate from all others.
    {
        yield return new WaitForSeconds(playerRespawnTime);

        while (gameState != GameStates.Round)
        {
            yield return null;
        }

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Instantiate(player, playerSpawn.position, playerSpawn.rotation);
    }

    public void RocketLaunched() //Rockets have a limited amount of ammo, which this script keeps track of.
    {
        rockets--;
        UpdateNumText(rocketsText, rockets);
    }

    public void AddCoin() //Called when the player collects a single coin.
    {
        coins++;
        UpdateNumText(coinsText, coins);
        GameControl.PlayClip(coin);
    }

    void GameOver() //Displays the Game Over screen with restart / quit buttons.
    {
        ChangeState(GameStates.GameOver);

        gameOverScreen.SetActive(true);

        if (!Application.isEditor)
        {
            gameOverQuitButton.interactable = true;
        }

        UpdateHiScore();
    }

    void UpdateHiScore() //Opens up a reader to check if the current score is higher than the last.
    {
        if (File.Exists(hiScorePath))
        {
            StreamReader reader = new StreamReader(hiScorePath);

            int previousHiScore = int.Parse(reader.ReadLine());

            reader.Close();

            if (previousHiScore < score) //If it's higher, overwrite it.
            {
                TextWriter writer = new StreamWriter(hiScorePath, false);

                writer.WriteLine(score.ToString());

                writer.Close();
            }
        }
    }

    public void Restart() //Reloads the scene.
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() //Called via UI.
    {
        Application.Quit();
    }

    #endregion

    #region Shop

    void ShopSetUp () //Activates / deactivates shop UI elements.
    {
        ChangeState(GameStates.Shopping);

        lifeCheck.SetActive(false); //All checkmarks are deactivated, as no items have been bought yet.
        healthCheck.SetActive(false);
        rocketsCheck.SetActive(false);
        rocketCapacityCheck.SetActive(false);
        shieldRateCheck.SetActive(false);
        shieldCapacityCheck.SetActive(false);

        shop.SetActive(true); //The shop parent object.

        ShopButtonsSetUp(); //Checks wether or not each shop item is purchasable.

        GameControl.CallGameFreeze();
    }

    void SubstractCoins(int amount) //Substract coins from the total and check again if items should be purchasable.
    {
        coins -= amount;

        ShopButtonsSetUp();

        UpdateNumText(coinsText, coins); //Finally, display the new amount.
    }

    void ShopButtonsSetUp() //Items can only be bought once per shopping phase. Once checked, buttons will become uninteractive.
    {
        lifeButton.interactable = lives < maxLives && coins >= lifePrice && !lifeCheck.activeSelf; //Each individual item will be purchasable if the player can afford it.

        healthButton.interactable = playerDamaged && coins >= healthPrice && !healthCheck.activeSelf; //Health will not be available for purchase if it is at full.

        rocketsButton.interactable = rockets < rocketCapacity && coins >= rocketsPrice && !rocketsCheck.activeSelf; //Equates the amount of rockets to its maximum capacity.

        rocketCapacityButton.interactable = rocketCapacity < maxRocketCapacity && coins >= rocketCapacityPrice && !rocketCapacityCheck.activeSelf; //Increases rocket maximum capacity.

        shieldRateButton.interactable = shieldRate <= minShieldRate && coins >= shieldRatePrice && !shieldRateCheck.activeSelf; //Decreases shield recharge rate.

        shieldCapacityButton.interactable = shieldCapacity < maxShieldCapacity && coins >= shieldCapacityPrice && !shieldCapacityCheck.activeSelf; //Increases shield capacity.
    }

    public void PurchaseLives()
    {
        lives++;

        UpdateNumText(livesText, lives);
        
        lifeCheck.SetActive(true);

        SubstractCoins(lifePrice);
    }

    public void PurchaseRockets()
    {
        rockets = rocketCapacity;

        UpdateNumText(rocketsText, rockets);

        rocketsCheck.SetActive(true);

        SubstractCoins(rocketsPrice);

        GameControl.CallRocketsReplenished(); //Rocket launchers will replenish their ammo.
    }

    public void PurchaseHealth()
    {
        UpdateHealth(maxHealth);

        healthCheck.SetActive(true);

        SubstractCoins(healthPrice);

        GameControl.CallHealthRestored(); //The player is subscribed to this event to refill its instance life.
    }

    public void PurchaseRocketCapacity() //Unlike with ammo, rocket launchers need not know their maximum capacity.
    {
        rocketCapacity++;

        UpdateNumText(rocketCapacityText, rocketCapacity);

        rocketCapacityCheck.SetActive(true);

        SubstractCoins(rocketCapacityPrice);
    }

    public void PurchaseShieldRate()
    {
        shieldRate -= 0.5f; //The shield rate is not displayed.

        shieldRateCheck.SetActive(true);

        SubstractCoins(shieldRatePrice);
    }

    public void PurchaseShieldCapacity()
    {
        shieldCapacity += 10;
        
        UpdateShield(shieldCapacity);
        UpdateNumText(shieldCapacityText, shieldCapacity);

        shieldCapacityCheck.SetActive(true);

        SubstractCoins(shieldCapacityPrice);

        GameControl.CallShieldUpgraded(); //The player's shield will hear this event and increase its capacity.
    }

    public void ShoppingDone () //Called upon by button interaction.
    {
        shop.SetActive(false);

        RoundStart(); //A new round starts.
    }

    #endregion

    #region HUD management

    public void UpdateHealth(float newHealth) //Update health according to damage received or restoration.
    {
        if (newHealth != maxHealth)
        {
            playerDamaged = true;
        }
        else
        {
            playerDamaged = false;
        }

        UpdateNumText(healthText, newHealth);
    }

    public void UpdateShield(float shield) //Update shield according to damage received or regeneration.
    {
        UpdateNumText(shieldText, shield);
    }

    public void UpdateNumText (Text text, float value) //Updates a UI nummerical value (score, kills, etc.).
    {
        text.text = Mathf.RoundToInt(value).ToString();
    }

    #endregion
}