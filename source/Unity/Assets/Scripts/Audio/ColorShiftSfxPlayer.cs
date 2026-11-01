using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class ColorShiftSfxPlayer : MonoBehaviour
{
    public static ColorShiftSfxPlayer Instance { get; private set; }

    [Header("Clips")]
    [SerializeField]
    private AudioClip successClip;

    [SerializeField]
    private AudioClip blockedClip;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField]
    private float successVolume = 0.45f;

    [Range(0f, 1f)]
    [SerializeField]
    private float blockedVolume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlaySuccess()
    {
        if (successClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(successClip, successVolume);
    }

    public void PlayBlocked()
    {
        if (blockedClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(blockedClip, blockedVolume);
    }
}