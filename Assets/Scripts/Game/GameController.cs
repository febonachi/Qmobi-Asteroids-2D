using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {
    public static GameController instance;

    #region editor
    public TextMeshProUGUI scoreText = default;
    public CameraManager cameraManager = default;
    #endregion

    #region private
    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region public
    public void restartLevel() {
        scoreText.text = "0";
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}
