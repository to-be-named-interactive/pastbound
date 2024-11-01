using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
    public float bounceForce = 2;
	public float bounceForceAdditive;
	


	[Space(5)]

	public float bounceStartTime = 0.5f;
	public float bounceEndTime = 0.5f;

	int bouncesCount = 0;
	bool isBouncing;
	float currentbounceForce;

	public IEnumerator Bounce()
	{
		isBouncing = true;
		currentbounceForce = bounceForce;

		if (bouncesCount <= 3) bouncesCount++;

		if(bouncesCount == 2) currentbounceForce += bounceForceAdditive;

		if (bouncesCount == 3) currentbounceForce += bounceForceAdditive;
	



		float startTime = Time.time;

		while (Time.time - startTime <= bounceStartTime)
		{
			Player.Instance.velocity.y = currentbounceForce;
			yield return null;
		}

		startTime = Time.time;

		Player.Instance.velocity.y = currentbounceForce;

		while (Time.time - startTime <= bounceEndTime)
		{
			yield return null;
		}

		isBouncing = false;
	}

	public void ResetBounceForce()
    {
		currentbounceForce = bounceForce;
	}
}
