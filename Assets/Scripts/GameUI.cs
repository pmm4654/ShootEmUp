using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    [Header("Banner Configuration")]
    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public float bannerDelayTime = 1f;
    public float bannerSpeed = 2.5f;
    public Vector2 newWaveBannerMinMaxPosition = new Vector2(-265f, 40f);

    [Header("Player UI")]
    public RectTransform healthBar;
    public Text scoreUI;
    public Text gameOverScoreUI;

    Spawner spawner;
    Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if(player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        newWaveTitle.text = "- Wave " + waveNumber + " -";
        string enemyCountString = spawner.waves[waveNumber - 1].infinite ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount.ToString();
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float bannerDelayTime = 1f;
        float bannerSpeed = 2.5f;
        float animationPercent = 0;
        int direction = 1;

        float endDelayTime = Time.time + 1 / bannerSpeed + bannerDelayTime;

        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * bannerSpeed * direction;

            if(animationPercent >= 1) // banner is as high as it's going to go, we want to pause
            {
                animationPercent = 1; // make sure it doesn't go too far above 1
                if(Time.time > endDelayTime) // finished waiting
                {
                    direction = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(newWaveBannerMinMaxPosition.x, newWaveBannerMinMaxPosition.y, animationPercent);
            yield return null;
        }
    }

    void OnGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.9f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float fadeTime)
    {
        float fadeSpeed = 1 / fadeTime;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }
}
