using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shootsound : MonoBehaviour
{
    public AudioSource source;

    private void Start()
    {
        source.volume = GlobalAudio.Instance.globalVolume;
        IPlayerController _player = GetComponentInParent<IPlayerController>();
        if (_player == null) Debug.Log("Player is null for anim");
        _player.OnShoot += OnShoot;
    }

    public void OnShoot()
    {
        source.Play();
    }

}
