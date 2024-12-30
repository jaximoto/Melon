using UnityEngine;

public class OneWayCollisionFix : MonoBehaviour
{
    public LayerMask playerLayer; // Assign the Player Layer in the Inspector
    public LayerMask platformLayer; // Assign the Platform Layer in the Inspector

    private PlatformEffector2D platformEffector;

    void Start()
    {
        platformEffector = GetComponent<PlatformEffector2D>();
        if (!platformEffector)
        {
            Debug.LogError("No PlatformEffector2D found on this GameObject.");
        }
    }

    void Update()
    {
        if (ShouldIgnoreCollision()) // Replace with your custom condition
        {
            platformEffector.useColliderMask = false;
            Physics2D.IgnoreLayerCollision(
                LayerMaskToLayerNumber(platformLayer),
                LayerMaskToLayerNumber(playerLayer),
                true
            );
        }
        else
        {
            platformEffector.useColliderMask = true;
            Physics2D.IgnoreLayerCollision(
                LayerMaskToLayerNumber(platformLayer),
                LayerMaskToLayerNumber(playerLayer),
                false
            );
        }
    }

    private bool ShouldIgnoreCollision()
    {
        // Replace with your logic (e.g., player position relative to platform)
        return Input.GetKey(KeyCode.S); // Example: Pressing 'S' to drop through
    }

    private int LayerMaskToLayerNumber(LayerMask layerMask)
    {
        return Mathf.RoundToInt(Mathf.Log(layerMask.value, 2));
    }
}
