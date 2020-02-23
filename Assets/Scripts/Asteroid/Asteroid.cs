using System;
using UnityEngine;
using Utils;

using static UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Asteroid : PoolObject {
    #region editor
    [SerializeField] private MinMaxValue moveSpeed = new MinMaxValue(1f, 5f);
    [SerializeField] private MinMaxValue rotationSpeed = new MinMaxValue(1f, 5f);
    #endregion

    #region public events
    public event EventHandler destroyed;
    #endregion

    #region public properties
    public AsteroidType asteroidType => type;
    #endregion

    private bool isInteractive = false;

    private AsteroidType type = default;
    private float currentMoveSpeed = 1f;
    private float currentRotationSpeed = 1f;

    private Rigidbody2D rb2d = default;

    private Vector2 direction = Vector2.zero;

    private Camera mainCamera = default;

    #region private
    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        
        currentMoveSpeed = moveSpeed.random;
        currentRotationSpeed = rotationSpeed.random * (value < .5f ? -1f : 1f);
    }

    private void Start() => mainCamera = GameController.instance.cameraManager.mainCamera;

    private void Update() => keepObjectInsideScreenBounds();

    private void FixedUpdate() {
        rb2d.MovePosition(rb2d.position + direction * currentMoveSpeed * Time.fixedDeltaTime);
        rb2d.MoveRotation(rb2d.rotation + currentRotationSpeed * Time.fixedDeltaTime);
    }

    private void keepObjectInsideScreenBounds() {
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        
        Vector2 nextViewportPosition = Vector2.zero;
        if (viewportPosition.x < -.2f) nextViewportPosition = new Vector3(1.1f, viewportPosition.y);
        else if (viewportPosition.x > 1.2f) nextViewportPosition = new Vector3(-.1f, viewportPosition.y);

        if (viewportPosition.y < -.2f) nextViewportPosition = new Vector3(viewportPosition.x, 1.1f);
        else if (viewportPosition.y > 1.2f) nextViewportPosition = new Vector3(viewportPosition.x, -.1f);

        if (nextViewportPosition != Vector2.zero) transform.position = (Vector2)mainCamera.ViewportToWorldPoint(nextViewportPosition);
    }

    private void makeInteractive() => isInteractive = true;
    #endregion

    #region public
    public void setType(AsteroidType type) {
        this.type = type;

        GetComponent<SpriteRenderer>().color = type.color;
    }

    public void setDirection(Vector3 dir, float impulse = 0f) {
        direction = dir.normalized;
        rb2d.AddForce(direction * impulse, ForceMode2D.Impulse);

        Invoke("makeInteractive", .5f);
    }

    public void takeDamage(Collision2D collision) {
        if (!isInteractive) return;
        isInteractive = false;

        destroyed?.Invoke(this, EventArgs.Empty);

        if (type.life > 1) {
            float angle = 90f / type.life;
            for (int i = 0; i < 2; i++) {
                Asteroid asteroid = AsteroidSpawner.instance.buildAsteroid(type.life - 1, transform.position, transform.rotation);
                Vector3 asteroidForce = Quaternion.Euler(0f, 0f, angle) * -collision.GetContact(0).normal;
                asteroid.setDirection(asteroidForce, Range(3f, 6f));
                asteroid.currentMoveSpeed = currentMoveSpeed * 2f;

                angle *= -1f;

                Debug.DrawRay(transform.position, asteroidForce * 10, type.color, 1f);
            }
        }

        returnToPool();
    }

    public override void reuse() {
        isInteractive = false;
        direction = Vector2.zero;
        currentMoveSpeed = moveSpeed.random;
        currentRotationSpeed = rotationSpeed.random * (value < .5f ? -1f : 1f);
    }
    #endregion
}
