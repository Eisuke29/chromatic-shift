using UnityEngine;

[DisallowMultipleComponent]
public sealed class ColorObject : MonoBehaviour
{
    [Header("Object Color")]
    [SerializeField]
    private StageColor objectColor = StageColor.Red;

    [Header("Materials")]
    [SerializeField]
    private Material solidMaterial;

    [SerializeField]
    private Material ghostMaterial;

    [Header("Automatic Targets")]
    [Tooltip("空欄なら、このGameObjectと子GameObjectから自動取得します。")]
    [SerializeField]
    private Renderer[] targetRenderers;

    [Tooltip("空欄なら、このGameObjectと子GameObjectから自動取得します。")]
    [SerializeField]
    private Collider[] targetColliders;

    [Header("Player Rematerialization Check")]
    [Tooltip("実体化予定の範囲にPlayerが重なっていないか確認します。")]
    [SerializeField]
    private LayerMask playerLayerMask;

    [Tooltip("プレイヤー判定だけに使う余白です。")]
    [SerializeField]
    private Vector3 overlapCheckPadding =
        new Vector3(0.12f, 0.12f, 0.12f);

    public StageColor ObjectColor => objectColor;
    public bool IsGhost { get; private set; }

    private PushableBox ownPushableBox;

    private void Awake()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<Renderer>(true);
        }

        if (targetColliders == null || targetColliders.Length == 0)
        {
            targetColliders = GetComponentsInChildren<Collider>(true);
        }

        ownPushableBox = GetComponentInParent<PushableBox>();
    }

    private void Start()
    {
        if (ColorManager.Instance != null)
        {
            ColorManager.Instance.Register(this);
        }
    }

    private void OnDestroy()
    {
        if (ColorManager.Instance != null)
        {
            ColorManager.Instance.Unregister(this);
        }
    }

    public void ApplyState(StageColor backgroundColor)
    {
        bool shouldBeGhost = objectColor == backgroundColor;
        SetGhostState(shouldBeGhost);
    }

    public bool WouldBlockPlayerIfSolid()
    {
        if (!TryGetVisualBounds(out Bounds visualBounds))
        {
            return false;
        }

        Collider[] hits = Physics.OverlapBox(
            visualBounds.center,
            visualBounds.extents + overlapCheckPadding,
            Quaternion.identity,
            playerLayerMask,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            if (hit == null)
            {
                continue;
            }

            if (hit.GetComponentInParent<CharacterController>() != null)
            {
                return true;
            }

            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    // 箱どうしが重なっていても、色変更は許可する。
    // 禁止するのは「プレイヤーが実体化予定の物体の内部にいる」場合だけ。
    public bool WouldBlockAnotherPushableBoxIfSolid()
    {
        return false;
    }

    private bool TryGetVisualBounds(out Bounds combinedBounds)
    {
        combinedBounds = new Bounds();
        bool foundRenderer = false;

        foreach (Renderer targetRenderer in targetRenderers)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            if (!foundRenderer)
            {
                combinedBounds = targetRenderer.bounds;
                foundRenderer = true;
            }
            else
            {
                combinedBounds.Encapsulate(targetRenderer.bounds);
            }
        }

        return foundRenderer;
    }

    private void SetGhostState(bool ghost)
    {
        IsGhost = ghost;

        if (ghost)
        {
            ownPushableBox?.SetGhostState(true);

            foreach (Renderer targetRenderer in targetRenderers)
            {
                if (targetRenderer != null)
                {
                    // 完全透明ではなく、Rendererそのものを停止する。
                    targetRenderer.enabled = false;
                }
            }

            foreach (Collider targetCollider in targetColliders)
            {
                if (targetCollider == null || targetCollider.isTrigger)
                {
                    continue;
                }

                targetCollider.enabled = false;
            }

            return;
        }

        foreach (Renderer targetRenderer in targetRenderers)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            if (solidMaterial != null)
            {
                targetRenderer.sharedMaterial = solidMaterial;
            }

            targetRenderer.enabled = true;
        }

        foreach (Collider targetCollider in targetColliders)
        {
            if (targetCollider == null || targetCollider.isTrigger)
            {
                continue;
            }

            targetCollider.enabled = true;
        }

        ownPushableBox?.SetGhostState(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Renderer[] renderers = targetRenderers;

        if (renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        Bounds visualBounds = new Bounds();
        bool foundRenderer = false;

        foreach (Renderer targetRenderer in renderers)
        {
            if (targetRenderer == null)
            {
                continue;
            }

            if (!foundRenderer)
            {
                visualBounds = targetRenderer.bounds;
                foundRenderer = true;
            }
            else
            {
                visualBounds.Encapsulate(targetRenderer.bounds);
            }
        }

        if (!foundRenderer)
        {
            return;
        }

        Gizmos.DrawWireCube(
            visualBounds.center,
            visualBounds.size + overlapCheckPadding * 2f
        );
    }
#endif
}