using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public void PlaySound(AudioClip clip, float volume = 1f, bool loop = false, float pitch = 1f)
    {
        SoundFx sfx = SoundFx.Instance;
        
        if(sfx != null)
        {
            sfx.Set(clip, volume, loop, pitch);
            sfx.Activate();
        }
        
    }
}
