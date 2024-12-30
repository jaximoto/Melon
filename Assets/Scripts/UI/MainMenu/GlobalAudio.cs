using UnityEngine;

public class GlobalAudio : MonoBehaviour
{
    public static GlobalAudio Instance { get; private set; }

    public int globalVolume = 100; // Default to 100%
    public int GlobalVolume
    {
        get => globalVolume;
        set
        {
            globalVolume = Mathf.Clamp(value, 0, 100); // Clamp between 0 and 100
            AdjustVolume();
        }
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    private void AdjustVolume()
    {
        // Scale AudioListener volume to 0.0 - 1.0 based on globalVolume
        AudioListener.volume = globalVolume / 100f;
        Debug.Log($"Global Volume set to: {globalVolume}%");
    }
}
