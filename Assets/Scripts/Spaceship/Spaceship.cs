using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Spaceship : MonoBehaviour {
    #region editor
    [SerializeField] private float speed = 1f;

    [Header("Guns settings")]
    [SerializeField] private float reloadTime = .1f;
    [SerializeField] private Transform gunsHolder = default;

    [Space]
    [SerializeField] private GameObject explosionEffectPrefab = default;
    #endregion

    private bool mousePressed = false;
    private float elapsedReloadTime = 0f;

    private Vector2 velocity = Vector2.zero;
    private Vector3 mousePosition = Vector3.zero;

    private Rigidbody2D rb2d = default;

    private int gunsHolderIndex = -1;
    private List<Gun> guns = new List<Gun>();

    private CameraManager cameraManager = default;
    private GameController gameController = default;

    #region private
    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();

        updateGuns();
    }

    private void Start() {
        gameController = GameController.instance;
        cameraManager = gameController.cameraManager;
        cameraManager.virtualCamera.Follow = transform;
    }

    private void Update() {
        mousePosition = cameraManager.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        transform.right = direction;

        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");
        velocity = velocity.normalized;

        if (Input.GetMouseButtonDown(0)) mousePressed = true;
        if (Input.GetMouseButtonUp(0)) mousePressed = false;

        if (mousePressed && elapsedReloadTime >= reloadTime) {
            guns.ForEach(gun => gun.shoot());

            elapsedReloadTime = 0f;
        }

        elapsedReloadTime += Time.deltaTime;
    }

    private void FixedUpdate() {
        if(velocity == Vector2.zero) {
            rb2d.velocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
        }else rb2d.velocity = velocity * speed * Time.fixedDeltaTime;
    }

    private async void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Asteroid")) {
            gameObject.SetActive(false);
            gameController.cameraManager.shakeOnce(5f, 3f);
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation);

            await Task.Delay(1500);

            gameController.restartLevel();
        }
    }
    #endregion

    #region public
    public void updateGuns() {
        gunsHolderIndex = Mathf.Clamp(gunsHolderIndex + 1, 0, gunsHolder.childCount - 1);
        guns.AddRange(gunsHolder.GetChild(gunsHolderIndex).GetComponentsInChildren<Gun>());
    }
    #endregion
}
