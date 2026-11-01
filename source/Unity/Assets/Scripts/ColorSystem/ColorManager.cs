using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [Header("Initial State")]
    [SerializeField]
    private StageColor initialColor = StageColor.Yellow;

    [Header("Stage Camera")]
    [SerializeField]
    private Camera stageCamera;

    [Header("Background Colors")]
    [SerializeField]
    private Color redBackground = new Color(0.75f, 0.12f, 0.12f);

    [SerializeField]
    private Color blueBackground = new Color(0.08f, 0.35f, 0.75f);

    [SerializeField]
    private Color yellowBackground = new Color(0.85f, 0.72f, 0.08f);

    [Header("Temporary Test Input")]
    [SerializeField]
    private bool enableNumberKeyDebug = true;

    private readonly HashSet<ColorObject> colorObjects = new();

    public StageColor CurrentColor { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("ColorManagerがScene内に複数存在します。", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentColor = initialColor;

        if (stageCamera == null)
        {
            stageCamera = Camera.main;
        }
    }

    private void Start()
    {
        ColorObject[] objects = FindObjectsByType<ColorObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (ColorObject colorObject in objects)
        {
            Register(colorObject);
        }

        ApplyColor(CurrentColor);
    }

    private void Update()
    {
        if (!enableNumberKeyDebug || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame ||
            Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            TrySetColor(StageColor.Red);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            TrySetColor(StageColor.Blue);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame ||
            Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            TrySetColor(StageColor.Yellow);
        }
    }

    public bool TrySetColor(StageColor newColor)
    {
        if (newColor == CurrentColor)
        {
            return true;
        }

        foreach (ColorObject colorObject in colorObjects)
        {
            if (colorObject == null)
            {
                continue;
            }

            bool willBecomeSolid =
                colorObject.ObjectColor != newColor &&
                colorObject.IsGhost;

            if (!willBecomeSolid)
            {
                continue;
            }

            if (colorObject.WouldBlockPlayerIfSolid())
            {
                Debug.LogWarning(
                    $"色変更を拒否しました。{colorObject.name}の内部にPlayerがいます。",
                    colorObject
                );

                return false;
            }

            if (colorObject.WouldBlockAnotherPushableBoxIfSolid())
            {
                Debug.LogWarning(
                    $"色変更を拒否しました。{colorObject.name}の内部に別の箱があります。",
                    colorObject
                );

                return false;
            }
        }

        ApplyColor(newColor);
        return true;
    }

    public void Register(ColorObject colorObject)
    {
        if (colorObject == null)
        {
            return;
        }

        colorObjects.Add(colorObject);
        colorObject.ApplyState(CurrentColor);
    }

    public void Unregister(ColorObject colorObject)
    {
        if (colorObject == null)
        {
            return;
        }

        colorObjects.Remove(colorObject);
    }

    private void ApplyColor(StageColor newColor)
    {
        CurrentColor = newColor;

        colorObjects.RemoveWhere(item => item == null);

        foreach (ColorObject colorObject in colorObjects)
        {
            colorObject.ApplyState(CurrentColor);
        }

        ApplyCameraBackground(CurrentColor);
    }

    private void ApplyCameraBackground(StageColor color)
    {
        if (stageCamera == null)
        {
            stageCamera = Camera.main;
        }

        if (stageCamera == null)
        {
            Debug.LogWarning("背景色を変更するCameraが見つかりません。", this);
            return;
        }

        stageCamera.clearFlags = CameraClearFlags.SolidColor;
        stageCamera.backgroundColor = GetBackgroundColor(color);
    }

    private Color GetBackgroundColor(StageColor color)
    {
        return color switch
        {
            StageColor.Red => redBackground,
            StageColor.Blue => blueBackground,
            StageColor.Yellow => yellowBackground,
            _ => Color.black
        };
    }
}