using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
public class Weapon_AudioConfigScriptableObject : ScriptableObject
{
    [Range(0, 2f)] public float Volume = 2;
    public AudioClip[] FireClips;
    public AudioClip[] FireLayerClip;
    public AudioClip EmptyClip;
    public AudioClip ReloadClip;
    public AudioClip LastBulletClip;
    public AudioClip LastBulletLayerClip;

    public void PlayShootingClip(AudioSource AudioSource, bool IsLastBullet = false)
    {
        if(IsLastBullet && LastBulletClip != null)
        {
            AudioSource.PlayOneShot(LastBulletClip, Volume);
            AudioSource.PlayOneShot(LastBulletLayerClip, Volume * 0.5f);
        }
        else
        {
            AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
            AudioSource.PlayOneShot(FireLayerClip[Random.Range(0, FireLayerClip.Length)], Volume * 0.5f);
        }
    }

    public void PlayOutOfAmmo(AudioSource AudioSource)
    {
        if(EmptyClip != null)
        {
            AudioSource.PlayOneShot(EmptyClip, Volume * 0.5f);
        }
    }
    public void PlayReloadClip(AudioSource AudioSource)
    {
        if (ReloadClip != null)
        {
            AudioSource.PlayOneShot(ReloadClip, Volume * 0.5f);
        }
    }
}
