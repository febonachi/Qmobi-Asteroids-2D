using UnityEngine;

public class Follower : MonoBehaviour {
    #region editor
    [SerializeField] private Transform followTarget = default;
    #endregion

    private Vector3 offset = Vector3.zero;

    #region editor
    private void Awake() => offset = new Vector3(followTarget.position.x - transform.position.x, followTarget.position.y - transform.position.y, transform.position.z);

    private void Update() => transform.position = new Vector3(followTarget.position.x + offset.x, followTarget.position.y + offset.y, offset.z);
    #endregion
}
