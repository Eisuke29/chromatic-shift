using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PushableBox))]
public sealed class SnapBoxWhenLanding : MonoBehaviour
{
    [Header("Landing Surface")]
    [Tooltip("箱がここへ着地したときだけ、中央へ自動調整します。")]
    [SerializeField]
    private Collider landingSurface;

    [Header("Snap Target")]
    [Tooltip("最終的に箱を固定する位置です。空のGameObjectを指定します。")]
    [SerializeField]
    private Transform snapPoint;

    [Header("Timing")]
    [Tooltip("落下して接地した様子を少し見せてから中央へ寄せます。")]
    [SerializeField]
    private float snapDelay = 0.08f;

    [SerializeField]
    private bool snapOnlyOnce = true;

    private PushableBox pushableBox;
    private bool hasSnapped;
    private Coroutine snapCoroutine;

    private void Awake()
    {
        pushableBox = GetComponent<PushableBox>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TrySnapAfterLanding(collision.collider);
    }

    private void OnCollisionStay(Collision collision)
    {
        TrySnapAfterLanding(collision.collider);
    }

    private void TrySnapAfterLanding(Collider other)
    {
        if (hasSnapped && snapOnlyOnce)
        {
            return;
        }

        if (landingSurface == null || snapPoint == null)
        {
            return;
        }

        if (other != landingSurface)
        {
            return;
        }

        if (snapCoroutine != null)
        {
            return;
        }

        snapCoroutine = StartCoroutine(SnapAfterDelay());
    }

    private IEnumerator SnapAfterDelay()
    {
        yield return new WaitForSeconds(snapDelay);

        if (pushableBox == null || snapPoint == null)
        {
            yield break;
        }

        pushableBox.SnapAndLock(
            snapPoint.position,
            snapPoint.rotation
        );

        hasSnapped = true;
        snapCoroutine = null;
    }
}