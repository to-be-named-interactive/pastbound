using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundFx : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;

	#region Instance
	// Singleton Instance
	private static SoundFx instance;

	public static SoundFx Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<SoundFx>();
			}
			
			return instance;
		}
	}
	#endregion

	private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

	public void Set(AudioClip clip, float volume = 1f, bool loop = false, float pitch = 1f)
	{
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.loop = loop;
	}

	public void Activate()
	{
		if (!audioSource.isPlaying) audioSource.Play();
	}
}