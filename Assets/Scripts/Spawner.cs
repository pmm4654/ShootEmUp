using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool developerMode;
    public Wave[] waves;
    public Enemy enemy;
    LivingEntity playerEntity;
    Transform playerTransform;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    Wave currentWave;
    int currentWaveNumber;

    MapGenerator map;

    float timeBetweenCampingChecks = 2f; // how long the player can stand still befor spawning enemy on top of it
    float campThresholdDistance = 1.5f; // how far you have to move for it to be considered not camping
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;

        nextCampCheckTime = Time.time + timeBetweenCampingChecks;
        campPositionOld = playerTransform.position;


        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if(Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = Vector3.Distance(playerTransform.position, campPositionOld) < campThresholdDistance;
                campPositionOld = playerTransform.position;
            }
            if((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnEnemy");
            }   
        }
        if(developerMode && Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine("SpawnEnemy");
            foreach(Enemy _enemy in FindObjectsOfType<Enemy>())
            {
                GameObject.Destroy(_enemy.gameObject);
            }
            NextWave();
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4; // per second

        Transform spawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerTransform.position);
        }
        Material tileMaterial = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMaterial.color;
        Color flashColor = Color.red;
        float spawnTimer = 0; // how much time has passed since starting coroutine

        while(spawnTimer < spawnDelay)
        {
            tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        tileMaterial.color = initialColor;
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerTransform.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3f;
    }

    void NextWave()
    {
        if(currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("LevelComplete");
        }
        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer; // damage indicator
        public float enemyHealth;
        public Color skinColor;

    }
}
