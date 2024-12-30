using UnityEngine;
using UnityEngine.UI;
public class FadeInImage : MonoBehaviour
{
    [SerializeField] private Image image;             // Drag your Image here
    [SerializeField] private float fadeDuration = 2.0f; // Duration of fade-in
    [SerializeField] private float delay = 3.0f;       // Delay before fade-in

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }

        // Ensure the Image starts fully transparent
        Color startColor = image.color;
        startColor.a = 0;
        image.color = startColor;

        // Use LeanTween to fade in the alpha with a delay
        LeanTween.value(gameObject, 0, 1, fadeDuration)
                 .setOnUpdate((float alpha) =>
                 {
                     Color currentColor = image.color;
                     currentColor.a = alpha;
                     image.color = currentColor;
                 })
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setDelay(delay);
    }
}
