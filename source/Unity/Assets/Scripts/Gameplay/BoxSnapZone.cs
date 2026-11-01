using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class BoxSnapZone : MonoBehaviour
{
    private enum SnapBehavior
    {
        LockCompletely,
        LockHorizontalOnly,
        DoNotLock
    }

    [Header("Target")]
    [SerializeField]
    private PushableBox targetBox;

    [SerializeField]
    private Transform snapPoint;

    [Header("Snap Condition")]
    [Tooltip("箱の中心がSnap Pointにこの距離まで近づいたときだけ位置調整します。")]
    [SerializeField]
    private float maxHorizontalDistance = 0.35f;

    [Tooltip("箱の中心YがSnap Pointに十分近いときだけ位置調整します。")]
    [SerializeField]
    private float maxVerticalDistance = 0.5f;

    [Header("After Snap")]
    [SerializeField]
    private SnapBehavior snapBehavior =
        SnapBehavior.LockCompletely;

    [SerializeField]
    private bool disableZoneAfterSnap = true;

    private BoxCollider triggerCollider;
    private bool hasSnapped;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        TrySnap(other);
    }

    private void TrySnap(Collider other)
    {
        if (hasSnapped || targetBox == null || snapPoint == null)
        {
            return;
        }

        PushableBox enteredBox =
            other.GetComponentInParent<PushableBox>();

        if (enteredBox != targetBox)
        {
            return;
        }

        Vector3 boxPosition = targetBox.transform.position;
        Vector3 targetPosition = snapPoint.position;

        Vector2 horizontalDifference = new Vector2(
            boxPosition.x - targetPosition.x,
            boxPosition.z - targetPosition.z
        );

        float verticalDifference = Mathf.Abs(
            boxPosition.y - targetPosition.y
        );

        if (horizontalDifference.magnitude > maxHorizontalDistance)
        {
            return;
        }

        if (verticalDifference > maxVerticalDistance)
        {
            return;
        }

        switch (snapBehavior)
        {
            case SnapBehavior.LockCompletely:
                targetBox.SnapAndLock(
                    snapPoint.position,
                    snapPoint.rotation
                );
                break;

            case SnapBehavior.LockHorizontalOnly:
                targetBox.SnapAndFreezeHorizontal(
                    snapPoint.position,
                    snapPoint.rotation
                );
                break;

            case SnapBehavior.DoNotLock:
                targetBox.SnapWithoutLock(
                    snapPoint.position,
                    snapPoint.rotation
                );
                break;
        }

        hasSnapped = true;

        if (disableZoneAfterSnap)
        {
            triggerCollider.enabled = false;
        }
    }
}