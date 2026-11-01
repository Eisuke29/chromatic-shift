using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public sealed class GoalTrigger : MonoBehaviour
{
    [Header("Player Control")]
    [SerializeField]
    private PlayerInput playerInput;

    [Header("UI")]
    [SerializeField]
    private GameObject clearPanel;

    [Header("Debug")]
    [SerializeField]
    private bool logClearMessage = true;

    private bool hasCleared;

    private void Awake()
    {
        Collider goalCollider = GetComponent<Collider>();
        goalCollider.isTrigger = true;

        if (clearPanel != null)
        {
            clearPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasCleared)
        {
            return;
        }

        CharacterController playerController =
            other.GetComponentInParent<CharacterController>();

        if (playerController == null)
        {
            return;
        }

        ClearStage();
    }

    private void ClearStage()
    {
        hasCleared = true;

        if (logClearMessage)
        {
            Debug.Log("Stage Clear");
        }

        BackgroundMusicPlayer.Instance?.PlayClearBgm();

        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}