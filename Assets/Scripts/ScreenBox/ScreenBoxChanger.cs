using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenBoxChanger : MonoBehaviour
{
	[SerializeField] CinemachineConfiner2D cameraConfiner;
	[SerializeField] TransitionAnimation transitionAnimation;


	GameObject[] screenBoxes;

	
    private void Update()
    {
		screenBoxes = Player.Instance.ScreenBoxDetection();

		ChangeScreenBox();
	}

    public void ChangeScreenBox()
    {
		if (screenBoxes != null)
		{
			//Fade out when we leave the previous screen box
			transitionAnimation.PlayFadeOutAnimation();


			//Get the ScreenBoxes gameobjects
			GameObject previousScreenBox = screenBoxes[0];
			GameObject nextScreenBox = screenBoxes[1];
			
			//Get the ScreenBoxesAssets gameobjects from both screenboxes
			GameObject nextScreenBoxAssets = nextScreenBox.transform.GetChild(0).gameObject;
			GameObject previousScreenBoxAssets = previousScreenBox.transform.GetChild(0).gameObject;


			//Deactivate previous and activate next ScreenBox
			if (!nextScreenBoxAssets.activeSelf) nextScreenBoxAssets.SetActive(true);
			if (previousScreenBoxAssets.activeSelf) previousScreenBoxAssets.SetActive(false);

			//Change camrea confiner to the new screenBox
			cameraConfiner.m_BoundingShape2D = nextScreenBox.GetComponent<PolygonCollider2D>();

			//Fade in when we enter the next screen box
			transitionAnimation.PlayFadeInAnimation();

		}
	}
}
