using UnityEngine;
using UnityEngine.UI;
public class AudioSlider : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider; // Drag your slider here

    private void Start()
    {
        // Ensure the slider matches the current global volume
        volumeSlider.value = GlobalAudio.Instance.GlobalVolume;

        // Add a listener to handle changes in slider value
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        // Set the global volume based on slider value
        GlobalAudio.Instance.GlobalVolume = Mathf.RoundToInt(value);
    }

    private void OnDestroy()
    {
        // Remove the listener to prevent memory leaks
        volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
