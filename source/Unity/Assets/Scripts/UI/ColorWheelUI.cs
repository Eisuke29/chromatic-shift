using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class ColorWheelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField]
    private GameObject wheelRoot;

    [SerializeField]
    private RectTransform wheelCenter;

    [SerializeField]
    private Image redOption;

    [SerializeField]
    private Image blueOption;

    [SerializeField]
    private Image yellowOption;

    [Header("Player Input")]
    [Tooltip("空欄の場合はScene内のPlayer Inputを自動取得します。")]
    [SerializeField]
    private PlayerInput playerInput;

    [Header("Selection")]
    [SerializeField]
    private float deadZone = 40f;

    [SerializeField]
    [Range(0f, 1f)]
    private float normalAlpha = 0.65f;

    [SerializeField]
    [Range(0f, 1f)]
    private float selectedAlpha = 1f;

    [SerializeField]
    private float normalScale = 1f;

    [SerializeField]
    private float selectedScale = 1.2f;

    private StageColor selectedColor;
    private bool isWheelOpen;

    private Color redBaseColor;
    private Color blueBaseColor;
    private Color yellowBaseColor;

    private void Awake()
    {
        if (wheelRoot == null)
        {
            Debug.LogError(
                "ColorWheelUIのWheel Rootが設定されていません。",
                this
            );

            enabled = false;
            return;
        }

        if (playerInput == null)
        {
            playerInput = FindFirstObjectByType<PlayerInput>();
        }

        SaveBaseColors();
        wheelRoot.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            OpenWheel();
        }

        if (isWheelOpen)
        {
            UpdateSelection();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            ConfirmSelection();
        }
    }

    private void SaveBaseColors()
    {
        if (redOption != null)
        {
            redBaseColor = redOption.color;
        }

        if (blueOption != null)
        {
            blueBaseColor = blueOption.color;
        }

        if (yellowOption != null)
        {
            yellowBaseColor = yellowOption.color;
        }
    }

    private void OpenWheel()
    {
        if (isWheelOpen)
        {
            return;
        }

        isWheelOpen = true;

        selectedColor = ColorManager.Instance != null
            ? ColorManager.Instance.CurrentColor
            : StageColor.Red;

        wheelRoot.SetActive(true);

        Time.timeScale = 0f;

        // TPSカメラがマウス移動を受け取らないようにします。
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateVisuals();
    }

    private void UpdateSelection()
    {
        if (wheelCenter == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector2 centerPosition =
            RectTransformUtility.WorldToScreenPoint(
                null,
                wheelCenter.position
            );

        Vector2 direction = mousePosition - centerPosition;

        if (direction.magnitude < deadZone)
        {
            return;
        }

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0f)
        {
            angle += 360f;
        }

        // 左上：赤
        if (angle >= 60f && angle < 180f)
        {
            selectedColor = StageColor.Red;
        }
        // 右上：青
        else if (
            (angle >= 0f && angle < 60f) ||
            (angle >= 300f && angle < 360f)
        )
        {
            selectedColor = StageColor.Blue;
        }
        // 下側：黄
        else
        {
            selectedColor = StageColor.Yellow;
        }

        UpdateVisuals();
    }

    private void ConfirmSelection()
    {
        if (!isWheelOpen)
        {
            return;
        }

        bool colorChanged =
    ColorManager.Instance != null &&
    ColorManager.Instance.TrySetColor(selectedColor);

        if (colorChanged)
        {
            ColorShiftSfxPlayer.Instance?.PlaySuccess();
        }
        else
        {
            ColorShiftSfxPlayer.Instance?.PlayBlocked();
        }

        CloseWheel();
    }

    private void CloseWheel()
    {
        isWheelOpen = false;

        wheelRoot.SetActive(false);

        Time.timeScale = 1f;

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UpdateVisuals()
    {
        SetOptionVisual(
            redOption,
            redBaseColor,
            selectedColor == StageColor.Red
        );

        SetOptionVisual(
            blueOption,
            blueBaseColor,
            selectedColor == StageColor.Blue
        );

        SetOptionVisual(
            yellowOption,
            yellowBaseColor,
            selectedColor == StageColor.Yellow
        );
    }

    private void SetOptionVisual(
        Image option,
        Color baseColor,
        bool selected
    )
    {
        if (option == null)
        {
            return;
        }

        float alpha = selected
            ? selectedAlpha
            : normalAlpha;

        option.color = new Color(
            baseColor.r,
            baseColor.g,
            baseColor.b,
            alpha
        );

        float scale = selected
            ? selectedScale
            : normalScale;

        option.rectTransform.localScale =
            Vector3.one * scale;
    }

    private void OnDisable()
    {
        if (!isWheelOpen)
        {
            return;
        }

        Time.timeScale = 1f;

        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isWheelOpen = false;
    }
}