using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

using static UnityEngine.Random;

[Serializable]
public struct AsteroidType {
    public int life;
    public Color color;
}

public class AsteroidSpawner : MonoBehaviour {
    public static AsteroidSpawner instance = default;

    #region editor
    [Header("Spawn settings")]
    [SerializeField] private int[] waves = default;
    [SerializeField] private float spawnDelay = .5f;

    [Space]
    [Header("Asteroid settings")]
    [SerializeField] private Asteroid asteroidPrefab = default;
    [SerializeField] private AsteroidType[] asteroidTypes = default;
    #endregion

    private int currentWaveIndex = 0;
    private int destroyedAsteroids = 0;

    private Spaceship spaceShip = default;

    private PoolManager poolManager = default;
    private GameController gameController = default;

    #region private
    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        spaceShip = FindObjectOfType<Spaceship>();
    }

    private void Start() {
        poolManager = PoolManager.instance;

        gameController = GameController.instance;

        InvokeRepeating("spawnAsteroid", 0f, spawnDelay);
    }

    private void spawnAsteroid() {
        int[] lifesRange = asteroidTypes.Select(t => t.life).ToArray();
        int asteroidLifes = lifesRange[Range(0, lifesRange.Length)];
        Vector2 asteroidPosition = gameController.cameraManager.mainCamera.ViewportToWorldPoint(randomSpawnPosition());
        Quaternion asteroidRotation = Quaternion.Euler(0f, 0f, Range(0f, 360f));
        Asteroid asteroid = buildAsteroid(asteroidLifes, asteroidPosition, asteroidRotation);
        asteroid.setDirection(spaceShip.transform.position - asteroid.transform.position);
    }

    private Vector2 randomSpawnPosition() {
        Vector2 spawnPosition = Vector3.zero;
        float randomFirstPoint = value < .5f ? -.1f : 1.1f;
        float randomSecondPoint = Range(0f, 1f);
        spawnPosition = value < .5f ? new Vector2(randomFirstPoint, randomSecondPoint) : new Vector2(randomSecondPoint, randomFirstPoint);
        return spawnPosition;
    }

    private void onAsteroidDestroyed(object sender, EventArgs args) {
        Asteroid asteroid = sender as Asteroid;
        asteroid.destroyed -= onAsteroidDestroyed;

        gameController.scoreText.text = $"{int.Parse(gameController.scoreText.text) + asteroid.asteroidType.life}";

        destroyedAsteroids++;
        if(destroyedAsteroids >= waves[currentWaveIndex]) {
            spaceShip.updateGuns();
            CancelInvoke("spawnAsteroid");

            spawnDelay -= spawnDelay * .2f;
            InvokeRepeating("spawnAsteroid", 2f, spawnDelay);

            currentWaveIndex = Mathf.Clamp(currentWaveIndex + 1, 0, waves.Length - 1);
            destroyedAsteroids = 0;
        }
    }
    #endregion

    #region public static
    public Asteroid buildAsteroid(int life, Vector2 position, Quaternion rotation) {
        if (asteroidTypes.Length == 0) return null;

        Asteroid asteroid = poolManager.getPoolObject("asteroids", position, rotation).GetComponent<Asteroid>();
        AsteroidType? asteroidType = asteroidTypes.FirstOrDefault(t => t.life == life);
        if (asteroidType == null) asteroidType = asteroidTypes.First();
        asteroid.destroyed += onAsteroidDestroyed;
        asteroid.setType(asteroidType.Value);

        float scale = (1f / asteroidTypes.Length) * life;
        asteroid.transform.localScale = asteroidPrefab.transform.localScale * scale;
        asteroid.transform.DOPunchScale(Vector3.one, .5f, 5).SetEase(Ease.OutBounce);

        return asteroid;
    }
    #endregion
}
