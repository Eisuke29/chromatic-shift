using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class BackgroundMusicPlayer : MonoBehaviour
{
    public static BackgroundMusicPlayer Instance { get; private set; }

    [Header("Main BGM")]
    [SerializeField]
    private AudioClip mainBgmClip;

    [Range(0f, 1f)]
    [SerializeField]
    private float mainBgmVolume = 0.10f;

    [Header("Clear BGM")]
    [SerializeField]
    private AudioClip clearBgmClip;

    [Range(0f, 1f)]
    [SerializeField]
    private float clearBgmVolume = 0.22f;

    [SerializeField]
    private bool loopClearBgm = true;

    private AudioSource audioSource;
    private bool isPlayingClearBgm;

    public bool IsPlayingClearBgm => isPlayingClearBgm;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        PlayMainBgm(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayMainBgm(bool restartFromBeginning)
    {
        if (audioSource == null || mainBgmClip == null)
        {
            return;
        }

        bool isAlreadyPlayingMain =
            !isPlayingClearBgm &&
            audioSource.isPlaying &&
            audioSource.clip == mainBgmClip;

        if (isAlreadyPlayingMain && !restartFromBeginning)
        {
            return;
        }

        isPlayingClearBgm = false;

        audioSource.Stop();
        audioSource.clip = mainBgmClip;
        audioSource.loop = true;
        audioSource.volume = mainBgmVolume;
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void PlayClearBgm()
    {
        if (audioSource == null || clearBgmClip == null)
        {
            return;
        }

        isPlayingClearBgm = true;

        audioSource.Stop();
        audioSource.clip = clearBgmClip;
        audioSource.loop = loopClearBgm;
        audioSource.volume = clearBgmVolume;
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void ResumeMainBgmAfterClear()
    {
        if (!isPlayingClearBgm)
        {
            return;
        }

        PlayMainBgm(true);
    }

    public void SetMainBgmVolume(float newVolume)
    {
        mainBgmVolume = Mathf.Clamp01(newVolume);

        if (!isPlayingClearBgm && audioSource != null)
        {
            audioSource.volume = mainBgmVolume;
        }
    }

    public void SetClearBgmVolume(float newVolume)
    {
        clearBgmVolume = Mathf.Clamp01(newVolume);

        if (isPlayingClearBgm && audioSource != null)
        {
            audioSource.volume = clearBgmVolume;
        }
    }
}