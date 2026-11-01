using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class StageResetController : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("ONなら、プレイ中にRキーでステージを最初からやり直せます。")]
    [SerializeField]
    private bool enableKeyboardReset = true;

    [Header("Debug")]
    [SerializeField]
    private bool logResetMessage = true;

    private bool isResetting;

    private void Update()
    {
        if (!enableKeyboardReset || isResetting)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetStage();
        }
    }

    public void ResetStage()
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;

        if (logResetMessage)
        {
            Debug.Log("Stage Reset");
        }

        BackgroundMusicPlayer.Instance?.ResumeMainBgmAfterClear();

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }
}