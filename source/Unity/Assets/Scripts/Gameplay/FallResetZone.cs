using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class FallResetZone : MonoBehaviour
{
    [Header("Reset Target")]
    [SerializeField]
    private StageResetController stageResetController;

    [Header("Debug")]
    [SerializeField]
    private bool logFallReset = true;

    private void Awake()
    {
        Collider zoneCollider = GetComponent<Collider>();
        zoneCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterController playerController =
            other.GetComponentInParent<CharacterController>();

        if (playerController == null)
        {
            return;
        }

        if (logFallReset)
        {
            Debug.Log("Fall Reset");
        }

        stageResetController?.ResetStage();
    }
}