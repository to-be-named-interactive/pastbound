using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;

    [SerializeField]
    private float addedWidth;

    float virtualCameraHeight;
    float virtualCameraWidth;

    float OrthographicSize = 10;
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void LateUpdate()
    {
        float distanceFromCenter = Player.Instance.PlayerDistanceFromScreenBoxCenter();
        
        // Calculating the cameras height and width from OrthographicSize (we need the height to get the width)
        virtualCameraHeight = 2f * OrthographicSize;
        virtualCameraWidth = virtualCameraHeight * virtualCamera.m_Lens.Aspect;

        // Sometimes virtualCameraWidth would go to 20f for a few times and 35.5555556f for most times, which made some twitching
        // Weird fix but it works "__"
        if (virtualCameraWidth == 20f) virtualCameraWidth = 35.5555556f;
        
        if (distanceFromCenter != float.NaN)
        {
            if(Player.Instance.screenBoxEntireWidth != float.NaN)
            {
                // Slowly move infront or behind the player
                float floatValue = (Player.Instance.screenBoxEntireWidth + addedWidth) / virtualCameraWidth;
                float floatValue2 = distanceFromCenter / floatValue;
                transposer.m_TrackedObjectOffset.x = -floatValue2;
            }
        }
    }
}