using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

public class GameController : MonoBehaviour
{
    #region Game variables

    public AudioSource audioSource;
	public GameObject enemy, player, splashScreen, pauseScreen, gameOverScreen;
	public Vector3 spawnValues;
    public Transform playerSpawn;
    public int enemyAmount, maxLives, maxHealth,  rocketCapacity, shieldCapacity, maxRocketCapacity, maxShieldCapacity;
	public float playerRespawnTime, roundTime, shieldRate, minShieldRate;
    public Button gameOverQuitButton, pauseQuitButton;
    public Text coinsText, timeText, livesText, healthText, killsText, rocketsText,
                shieldText, subText, mainText, scoreText, hiScoreText, pauseText;

    private int coins, score, round, kills, lives, rockets, enemiesToSpawn;
    private float roundTimeCounter, enemySpawnTime;
    private bool playerDamaged;

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    public int ShieldCapacity
    {
        get { return shieldCapacity; }
    }

    public float ShieldRate
    {
        get { return shieldRate; }
        set
        {
            if (value >= minShieldRate)
            {
                shieldRate = minShieldRate;
            }
            else
            {
                shieldRate = value;
            }
        }
    }

    public int Rockets
    {
        get { return rockets; }
    }

    private enum GameStates
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

    private string hiScorePath = Path.Combine(Application.streamingAssetsPath, "hiscore.txt");

    public delegate void GameFreeze();
    public event GameFreeze OnGameFreeze;

    public delegate void GamePause();
    public event GamePause OnGamePause;

    public delegate void GameFree();
    public event GameFree OnGameFree;

    public delegate void HealthRestored();
    public event HealthRestored OnHealthRestored;

    public delegate void ShieldUpgraded();
    public event ShieldUpgraded OnShieldUpgraded;

    #endregion

    #region Shop variables

    public GameObject shop;
    public int lifePrice, healthPrice, rocketsPrice, rocketCapacityPrice, shieldRatePrice, shieldCapacityPrice;
    private int currentRockets;
    public Button lifeButton, healthButton, rocketsButton, rocketCapacityButton, shieldRateButton, shieldCapacityButton;
    public GameObject lifeCheck, healthCheck, rocketsCheck, rocketCapacityCheck, shieldRateCheck, shieldCapacityCheck; 
    public Text lifePriceText, healthPriceText, rocketsPriceText, rocketCapacityPriceText, shieldRatePriceText, shieldCapacityPriceText;

    #endregion

    #region Game

    void Start ()
	{
        SpawnPlayer();
        round = 0;
		coins = 0;
        score = 0;
        kills = 0;
        lives = maxLives;
        rockets = rocketCapacity;

        lifePriceText.text = lifePrice + " coins";
        healthPriceText.text = healthPrice + " coins";
        rocketsPriceText.text = rocketsPrice + " coins";
        rocketCapacityPriceText.text = rocketCapacityPrice + " coins";
        shieldRatePriceText.text = shieldRatePrice + " coins";
        shieldCapacityPriceText.text = shieldCapacityPrice + " coins";

        gameOverQuitButton.interactable = false;
        pauseQuitButton.interactable = false;

        UpdateKills();
        UpdateRockets();
		UpdateCoins();
        UpdateScore(score);
        UpdateLives();

        ChangeState(GameStates.GameStart);

        shop.SetActive(false);
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        splashScreen.SetActive(true);

        if (File.Exists(hiScorePath))
        {
            StreamReader reader = new StreamReader(hiScorePath);
            hiScoreText.text = "Highest score: " + reader.ReadLine();
            reader.Close();
        }
        else
        {
            hiScoreText.text = "";
        }
    }
	
	void Update ()
	{
        switch (gameState)
        {
            case GameStates.GameStart:
                if (Input.anyKeyDown)
                {
                    RoundStart();
                    splashScreen.SetActive(false);
                }
                break;
            case GameStates.Round:
                roundTimeCounter += Time.deltaTime;

                float timeDiff = Mathf.RoundToInt(roundTime - roundTimeCounter);

                if (timeDiff >= 0)
                {
                    timeText.text = "Time: " + timeDiff;
                }
                else
                {
                    timeText.text = "Time: 0";
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    Pause();
                }
                break;
        }
    }

    void ChangeState(GameStates anotherGameState)
    {
        gameState = anotherGameState;

        switch (gameState)
        {
            case GameStates.GameStart:
                OnGameFreeze?.Invoke();
                mainText.text = "Arcade GameFree for All Space Battle!";
                subText.text = "Press any key to start";
                pauseText.text = "";
                break;
            case GameStates.Pause:
                OnGameFreeze?.Invoke();
                OnGamePause?.Invoke();
                mainText.text = "Pause";
                subText.text = "";
                pauseText.text = "";
                break;
            case GameStates.RoundStart:
                OnGameFree?.Invoke();
                mainText.text = "Round " + round;
                subText.text = "Are you ready?";
                pauseText.text = "";
                break;
            case GameStates.Round:
                mainText.text = "";
                subText.text = "";
                pauseText.text = "Press 'P' to pause";
                break;
            case GameStates.RoundEnd:
                mainText.text = "Round over!";
                subText.text = "You did great!";
                pauseText.text = "";
                break;
            case GameStates.Shopping:
                OnGameFreeze?.Invoke();
                mainText.text = "";
                subText.text = "";
                pauseText.text = "";
                break;
            case GameStates.GameOver:
                mainText.text = "Game Over";
                subText.text = "";
                pauseText.text = "";
                break;
        }
    }

    public void Pause()
    {
        OnGameFreeze?.Invoke();
        ChangeState(GameStates.Pause);
        pauseScreen.SetActive(true);

        if (!Application.isEditor)
        {
            pauseQuitButton.interactable = true;
        }
    }

    void Resume()
    {
        OnGameFree?.Invoke();
        pauseScreen.SetActive(false);
        ChangeState(GameStates.Round);
    }

    void RoundStart()
    {
        OnGameFree?.Invoke();
        roundTimeCounter = 0;
        round++;
        timeText.text = "Time: " + roundTime;
        enemiesToSpawn = enemyAmount;
        ChangeState(GameStates.RoundStart);
        StartCoroutine(Round());
    }
	
	IEnumerator Round ()
	{
        enemySpawnTime = roundTime / enemyAmount;

        yield return new WaitForSeconds (5);

        ChangeState(GameStates.Round);

        while (enemiesToSpawn > 0)
        {
            while (gameState == GameStates.GameOver || gameState == GameStates.Pause)
            {
                yield return null;
            }

            Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(enemy, spawnPosition, spawnRotation);

            enemiesToSpawn--;

            yield return new WaitForSeconds(enemySpawnTime);
        }

        if (gameState == GameStates.Round)
        {
            ChangeState(GameStates.RoundEnd);

            enemyAmount *= 2;
            enemiesToSpawn = enemyAmount;

            minShieldRate -= 0.5f;

            UpdateHiScore();

            yield return new WaitForSeconds(5);

            Shopping();
        }
    }

    public void EnemyDown()
    {
        kills++;
        UpdateKills();
    }

    public void PlayerDown()
    {
        lives--;
        UpdateLives();

        if (lives > 0)
        {
            StartCoroutine(Respawn());
        }
        else
        {
            GameOver();
        }
    }

    IEnumerator Respawn()
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

    public void RocketLaunched()
    {
        rockets--;
        UpdateRockets();
    }

    void GameOver()
    {
        ChangeState(GameStates.GameOver);
        gameOverScreen.SetActive(true);

        if (!Application.isEditor)
        {
            gameOverQuitButton.interactable = true;
        }

        UpdateHiScore();
    }

    void UpdateHiScore()
    {
        if (File.Exists(hiScorePath))
        {
            StreamReader reader = new StreamReader(hiScorePath);
            int previousHiScore = int.Parse(reader.ReadLine());
            reader.Close();

            if (previousHiScore < score)
            {
                TextWriter writer = new StreamWriter(hiScorePath, false);
                writer.WriteLine(score.ToString());
                writer.Close();
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion

    #region Shop

    void Shopping ()
    {
        ChangeState(GameStates.Shopping);

        currentRockets = rockets;

        lifeCheck.SetActive(false);
        healthCheck.SetActive(false);
        rocketsCheck.SetActive(false);
        rocketCapacityCheck.SetActive(false);
        shieldRateCheck.SetActive(false);
        shieldCapacityCheck.SetActive(false);

        shop.SetActive(true);

        if (coins > 0)
        {
            if (lives < maxLives && coins >= lifePrice)
            {
                lifeButton.interactable = true;
            }
            else
            {
                lifeButton.interactable = false;
            }

            if (playerDamaged && coins >= healthPrice)
            {
                healthButton.interactable = true;
            }
            else
            {
                healthButton.interactable = false;
            }

            if (rockets < rocketCapacity && coins >= rocketsPrice)
            {
                rocketsButton.interactable = true;
            }
            else
            {
                rocketsButton.interactable = false;
            }

            if (rocketCapacity < maxRocketCapacity && coins >= rocketCapacityPrice)
            {
                rocketCapacityButton.interactable = true;
            }
            else
            {
                rocketCapacityButton.interactable = false;
            }

            if (shieldRate <= minShieldRate && coins >= shieldRatePrice)
            {
                shieldRateButton.interactable = false;
            }
            else
            {
                shieldRateButton.interactable = true;
            }

            if (shieldCapacity < maxShieldCapacity && coins >= shieldCapacityPrice)
            {
                shieldCapacityButton.interactable = true;
            }
            else
            {
                shieldCapacityButton.interactable = false;
            }
        }
        else
        {
            lifeButton.interactable = false;
            healthButton.interactable = false;
            rocketsButton.interactable = false;
            rocketCapacityButton.interactable = false;
            shieldCapacityButton.interactable = false;
            shieldRateButton.interactable = false;
        }
        
    }

    public void AddCoin()
    {
        coins++;
        UpdateCoins();
        audioSource.Play();
    }

    public void SubstractCoins(int amount)
    {
        coins -= amount;

        if (coins <= 0)
        {
            coins = 0;

            lifeButton.interactable = false;
            healthButton.interactable = false;
            rocketsButton.interactable = false;
            rocketCapacityButton.interactable = false;
            shieldRateButton.interactable = false;
            shieldCapacityButton.interactable = false;
        }
        else
        {
            if (coins < lifePrice)
            {
                lifeButton.interactable = false;
            }
            if (coins < healthPrice)
            {
                healthButton.interactable = false;
            }
            if (coins < rocketsPrice || rocketsCheck.activeSelf)
            {
                rocketsButton.interactable = false;
            }
            if (coins < rocketCapacityPrice)
            {
                rocketCapacityButton.interactable = false;
            }
            if (coins < shieldRatePrice)
            {
                shieldRateButton.interactable = false;
            }
            if (coins < shieldCapacityPrice)
            {
                shieldCapacityButton.interactable = false;
            }
        }

        UpdateCoins();
    }

    public void PurchaseLives()
    {
        lives++;
        SubstractCoins(lifePrice);
        lifeButton.interactable = false;
        lifeCheck.SetActive(true);
        UpdateLives();
    }

    public void PurchaseRockets()
    {
        SubstractCoins(rocketsPrice);
        rockets = rocketCapacity;
        rocketsButton.interactable = false;
        rocketsCheck.SetActive(true);
        UpdateRockets();
    }

    public void PurchaseHealth()
    {
        OnHealthRestored?.Invoke();
        SubstractCoins(healthPrice);
        healthButton.interactable = false;
        healthCheck.SetActive(true);
    }

    public void PurchaseRocketCapacity()
    {
        SubstractCoins(rocketCapacityPrice);
        rocketCapacity++;
        rocketCapacityButton.interactable = false;

        if (!rocketsButton.interactable && rockets < rocketCapacity && coins >= rocketsPrice && !rocketsCheck.activeSelf)
        {
            rocketsButton.interactable = true;
        }

        rocketCapacityCheck.SetActive(true);
        UpdateRockets();
    }

    public void PurchaseShieldRate()
    {
        SubstractCoins(shieldRatePrice);
        shieldRate -= 0.5f;
        shieldRateButton.interactable = false;
        shieldRateCheck.SetActive(true);
    }

    public void PurchaseShieldCapacity()
    {
        SubstractCoins(shieldCapacityPrice);
        shieldCapacity += 10;
        OnShieldUpgraded?.Invoke();
        shieldCapacityButton.interactable = false;
        shieldCapacityCheck.SetActive(true);
    }

    public void ShoppingDone ()
    {
        shop.SetActive(false);
        RoundStart();
    }

    #endregion

    #region HUD management

    void UpdateLives()
    {
        livesText.text = "Lives: " + lives + " / " + maxLives;
    }

    public void UpdateHealth(int health)
    {
        healthText.text = "Health: " + health + " / " + MaxHealth;

        if (health == MaxHealth)
        {
            playerDamaged = false;
        }
        else
        {
            playerDamaged = true;
        }
    }

    void UpdateCoins()
    {
        coinsText.text = "Coins: " + coins;
    }

    void UpdateRockets()
    {
        rocketsText.text = "Rockets: " + rockets + " / " + rocketCapacity;
    }

    void UpdateKills()
    {
        killsText.text = "Kills: " + kills;
    }

    public void UpdateShield(int shield)
    {
        shieldText.text = "Shield: " + shield + " / " + ShieldCapacity;
    }

    public void UpdateScore(int scorePoints)
    {
        score += scorePoints;
        scoreText.text = "Score: " + score;
    }

    #endregion
}