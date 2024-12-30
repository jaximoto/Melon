using UnityEngine;
using UnityEngine.UI;

public class AnimatedBackground : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Array of sprites for the animation.")]
    public Sprite[] spriteClips;

    [Tooltip("Time (in seconds) between each frame.")]
    public float frameRate = 0.1f;

    [Tooltip("Should the animation loop?")]
    public bool loop = true;

    [Tooltip("Ease type for smooth transitions.")]
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    // Reference to the UI Image component
    private Image uiImage;

    // Current frame index
    private int currentFrame = 0;

    private void Start()
    {
        // Get the Image component attached to this GameObject
        uiImage = GetComponent<Image>();

        if (uiImage == null)
        {
            Debug.LogError("No Image component found on this GameObject.");
            return;
        }

        if (spriteClips == null || spriteClips.Length == 0)
        {
            Debug.LogError("No sprites assigned to spriteClips array.");
            return;
        }

        // Start the animation
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        // Use LeanTween to animate through the sprite array
        LeanTween.value(gameObject, 0, spriteClips.Length - 1, spriteClips.Length * frameRate)
            .setOnUpdate(UpdateFrame)
            .setOnComplete(OnAnimationComplete)
            .setEase(easeType); // Apply easing for smoother transitions

    }

    private void UpdateFrame(float value)
    {
        // Calculate the current frame index
        currentFrame = Mathf.FloorToInt(value);

        // Update the Image's sprite
        uiImage.sprite = spriteClips[currentFrame];
    }

    private void OnAnimationComplete()
    {
        if (loop)
        {
            PlayAnimation(); // Restart the animation if looping is enabled
        }
    }
}
