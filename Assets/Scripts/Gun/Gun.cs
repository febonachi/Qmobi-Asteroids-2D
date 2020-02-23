using UnityEngine;

public class Gun : MonoBehaviour {
    #region editor
    [SerializeField] private float bulletSpeed = 1f;
    #endregion

    private PoolManager poolManager;

    #region private
    private void Start() => poolManager = PoolManager.instance;
    #endregion

    #region public
    public void shoot() {
        Bullet bullet = poolManager.getPoolObject("bullets", transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.shoot(bulletSpeed);
    }
    #endregion
}
