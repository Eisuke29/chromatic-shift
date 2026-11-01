using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class PushableBox : MonoBehaviour
{
    [Header("Movement Limits")]
    [Tooltip("ONなら箱は倒れません。")]
    [SerializeField]
    private bool lockRotation = true;

    [Tooltip("ONなら箱は上下に動けません。通常の箱はOFFにします。")]
    [SerializeField]
    private bool lockVerticalMovement = false;

    [Header("Physics")]
    [SerializeField]
    private float mass = 18f;

    [SerializeField]
    private float linearDamping = 1.5f;

    [SerializeField]
    private float angularDamping = 8f;

    private Rigidbody boxRigidbody;

    private bool isGhost;
    private bool isLockedAtSnapPoint;
    private bool isHorizontalPositionLocked;

    public bool IsGhost => isGhost;
    public bool IsLockedAtSnapPoint => isLockedAtSnapPoint;
    public bool IsHorizontalPositionLocked => isHorizontalPositionLocked;

    private void Awake()
    {
        boxRigidbody = GetComponent<Rigidbody>();
        ApplyPhysicsSettings();
    }

    public void SetGhostState(bool ghost)
    {
        if (boxRigidbody == null)
        {
            return;
        }

        isGhost = ghost;

        boxRigidbody.linearVelocity = Vector3.zero;
        boxRigidbody.angularVelocity = Vector3.zero;

        if (ghost || isLockedAtSnapPoint)
        {
            boxRigidbody.isKinematic = true;
            return;
        }

        ApplyPhysicsSettings();
    }

    // 青箱用：
    // 穴の中央で完全に固定する。
    public void SnapAndLock(
        Vector3 worldPosition,
        Quaternion worldRotation
    )
    {
        if (boxRigidbody == null)
        {
            return;
        }

        boxRigidbody.linearVelocity = Vector3.zero;
        boxRigidbody.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(
            worldPosition,
            worldRotation
        );

        isLockedAtSnapPoint = true;
        isHorizontalPositionLocked = true;

        boxRigidbody.isKinematic = true;
    }

    // 黄箱用：
    // 指定位置へ寄せるが、Y方向には落下できる。
    public void SnapAndFreezeHorizontal(
        Vector3 worldPosition,
        Quaternion worldRotation
    )
    {
        if (boxRigidbody == null)
        {
            return;
        }

        boxRigidbody.linearVelocity = Vector3.zero;
        boxRigidbody.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(
            worldPosition,
            worldRotation
        );

        isLockedAtSnapPoint = false;
        isHorizontalPositionLocked = true;

        if (!isGhost)
        {
            ApplyPhysicsSettings();
        }
    }

    // 通常の位置調整だけを行う場合用。
    public void SnapWithoutLock(
        Vector3 worldPosition,
        Quaternion worldRotation
    )
    {
        if (boxRigidbody == null)
        {
            return;
        }

        boxRigidbody.linearVelocity = Vector3.zero;
        boxRigidbody.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(
            worldPosition,
            worldRotation
        );

        isLockedAtSnapPoint = false;
        isHorizontalPositionLocked = false;

        if (!isGhost)
        {
            ApplyPhysicsSettings();
        }
    }

    public void UnlockFromSnapPoint()
    {
        if (boxRigidbody == null)
        {
            return;
        }

        isLockedAtSnapPoint = false;
        isHorizontalPositionLocked = false;

        if (!isGhost)
        {
            ApplyPhysicsSettings();
        }
    }

    private void ApplyPhysicsSettings()
    {
        boxRigidbody.mass = mass;
        boxRigidbody.linearDamping = linearDamping;
        boxRigidbody.angularDamping = angularDamping;

        boxRigidbody.useGravity = true;
        boxRigidbody.isKinematic = false;

        RigidbodyConstraints constraints = RigidbodyConstraints.None;

        if (lockVerticalMovement)
        {
            constraints |= RigidbodyConstraints.FreezePositionY;
        }

        if (isHorizontalPositionLocked)
        {
            constraints |= RigidbodyConstraints.FreezePositionX;
            constraints |= RigidbodyConstraints.FreezePositionZ;
        }

        if (lockRotation)
        {
            constraints |= RigidbodyConstraints.FreezeRotation;
        }

        boxRigidbody.constraints = constraints;
    }
}