using UnityEngine;
using UnityEngine.UI;
public class Background : MonoBehaviour
{
    public Image[] backgrounds; // Assign your backgrounds here in order
    public Transform player; // Reference to the player
    public float heightThreshold1 = 100f; // Height to trigger the background change
    public float heightThreshold2 = 200f; // Height to trigger the background change
    private bool transition1Triggered = false;
    private bool transition2Triggered = false;
    private int currentBackgroundIndex = 0;

    public float fadeDuration = 2f; // Time for the fade animation


    private void Update()
    {
        if (!transition1Triggered && player.position.y > heightThreshold1)
        {
            TransitionToNextBackground();
            transition1Triggered = true; // Prevent multiple triggers
        }

        else if (!transition1Triggered && player.position.y > heightThreshold2)
        {
            TransitionToNextBackground();
            transition2Triggered = true; // Prevent multiple triggers
        }
    }
    public void TransitionToNextBackground()
    {
        // Ensure we don't go out of bounds
        if (currentBackgroundIndex >= backgrounds.Length - 1) return;

        // Current and next background
        Image currentBackground = backgrounds[currentBackgroundIndex];
        Image nextBackground = backgrounds[currentBackgroundIndex + 1];

        // Fade out the current background
        LeanTween.value(currentBackground.gameObject, 1f, 0f, fadeDuration)
            .setOnUpdate((float value) => UpdateAlpha(currentBackground, value)); // Use lambda here

        // Fade in the next background
        LeanTween.value(nextBackground.gameObject, 0f, 1f, fadeDuration)
            .setOnUpdate((float value) => UpdateAlpha(nextBackground, value)) // Use lambda here
            .setOnComplete(() => currentBackgroundIndex++);
    }

    // Helper function to update alpha
    private void UpdateAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
