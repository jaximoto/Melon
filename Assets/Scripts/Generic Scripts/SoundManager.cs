using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        music.volume = AudioListener.volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
