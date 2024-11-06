using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimation : MonoBehaviour
{
    public Animator transitionAnimator;

	void Start()
    {
        transitionAnimator = GetComponent<Animator>();  
    }

	public void PlayFadeInAnimation()
    {
		transitionAnimator.SetTrigger("TriggerFadeIn");
	}

	public void PlayFadeOutAnimation()
	{
		transitionAnimator.SetTrigger("TriggerFadeOut");
	}
}