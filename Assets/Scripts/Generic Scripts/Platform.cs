using UnityEngine;

public class Platform : MonoBehaviour, IShootable
{

    public Sprite redTileSprite;
    public Sprite blueTileSprite;

    public enum PlatformState
    {
        Frozen,
        Fire,
        Default
    };

    public PlatformState myState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        RenderPlatform();
    }


    public void OnShat(Bullet b)
    {

        if (b.shotType == Bullet.ShotType.ICE_SHOT &&
            myState == PlatformState.Fire)
        {
            // Freeze platform 
            Freeze();
        }
        else if (b.shotType == Bullet.ShotType.FIRE_SHOT &&
                myState == PlatformState.Frozen)
        {
            // Melt platform
            Melt();
        }
        else if (myState == PlatformState.Default)
        {
            if (b.shotType == Bullet.ShotType.FIRE_SHOT)
            {
                Melt();
            }
            else
            {
                Freeze();
            }
        }

        //TODO: Freeze platform + ice shot and fire platform + fire...

    }


    public void Freeze()
    {
        // Make it blue?
        SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
        s.sprite = blueTileSprite;

    }


    public void Melt()
    {
        // Make it red?
        SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
        s.sprite = redTileSprite;
    }


    public void RenderPlatform()
    {
        //TODO render based on state
    }

}
