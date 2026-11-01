using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerBoxPusher : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField]
    [Tooltip("歩いて箱へぶつかったときの押す強さです。")]
    private float pushForce = 12f;

    [SerializeField]
    [Tooltip("上方向へ押して箱が浮くのを防ぎます。")]
    private float verticalPushLimit = 0.15f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PushableBox pushableBox =
            hit.collider.GetComponentInParent<PushableBox>();

        if (pushableBox == null)
        {
            return;
        }

        Rigidbody targetRigidbody =
            hit.collider.GetComponentInParent<Rigidbody>();

        if (targetRigidbody == null || targetRigidbody.isKinematic)
        {
            return;
        }

        Vector3 pushDirection = hit.moveDirection;
        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        pushDirection.Normalize();

        if (hit.moveDirection.y < -verticalPushLimit)
        {
            return;
        }

        targetRigidbody.AddForce(
            pushDirection * pushForce,
            ForceMode.Acceleration
        );
    }
}