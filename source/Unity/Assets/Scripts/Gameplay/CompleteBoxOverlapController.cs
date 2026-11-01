using UnityEngine;

public sealed class CompleteBoxOverlapController : MonoBehaviour
{
    [Header("Color Objects")]
    [SerializeField]
    private ColorObject blueColorObject;

    [SerializeField]
    private ColorObject yellowColorObject;

    [Header("Boxes")]
    [SerializeField]
    private PushableBox blueBox;

    [SerializeField]
    private PushableBox yellowBox;

    [Header("Mixed Visual Only")]
    [Tooltip("完全に重なっているときだけ表示する、見た目専用の緑Cubeです。")]
    [SerializeField]
    private GameObject mergedGreenVisual;

    [Header("Complete Overlap Detection")]
    [Tooltip("X/Z方向の位置差がこの値以内なら一致とみなします。")]
    [SerializeField]
    private float horizontalTolerance = 0.03f;

    [Tooltip("Y方向の位置差がこの値以内なら一致とみなします。")]
    [SerializeField]
    private float verticalTolerance = 0.03f;

    private void Awake()
    {
        if (mergedGreenVisual != null)
        {
            mergedGreenVisual.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (blueColorObject == null ||
            yellowColorObject == null ||
            blueBox == null ||
            yellowBox == null ||
            mergedGreenVisual == null)
        {
            return;
        }

        bool bothAreSolid =
            !blueColorObject.IsGhost &&
            !yellowColorObject.IsGhost;

        bool areAtExactlySamePosition =
            IsAtSamePosition();

        bool shouldShowGreen =
            bothAreSolid &&
            areAtExactlySamePosition;

        if (shouldShowGreen)
        {
            mergedGreenVisual.transform.SetPositionAndRotation(
                blueBox.transform.position,
                blueBox.transform.rotation
            );
        }

        mergedGreenVisual.SetActive(shouldShowGreen);
    }

    private bool IsAtSamePosition()
    {
        Vector3 difference =
            blueBox.transform.position -
            yellowBox.transform.position;

        return Mathf.Abs(difference.x) <= horizontalTolerance &&
               Mathf.Abs(difference.y) <= verticalTolerance &&
               Mathf.Abs(difference.z) <= horizontalTolerance;
    }
}