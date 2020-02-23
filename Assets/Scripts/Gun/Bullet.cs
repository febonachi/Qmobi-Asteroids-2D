using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : PoolObject {
    #region editor
    [SerializeField] private ParticleSystem hitEffectPrefab = default;
    #endregion

    private Rigidbody2D rb2d = default;

    #region private
    private void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Asteroid")) {
            Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
            if (hitEffectPrefab != null) {
                ParticleSystem hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
                ParticleSystem.MainModule mainModule = hitEffect.main;
                mainModule.startColor = asteroid.asteroidType.color;
                Destroy(hitEffect.gameObject, 1f);
            }
            asteroid.takeDamage(collision);
        }
    }

    private void OnBecameInvisible() => returnToPool();
    #endregion

    #region public
    public void shoot(float speed) => rb2d.AddForce(transform.right * speed, ForceMode2D.Impulse);

    public override void reuse() {
        
    }
    #endregion
}
