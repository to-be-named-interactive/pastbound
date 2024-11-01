using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/*Finds all of the gameObjects that have a ParallaxLayer.cs script, and moves them!*/

public class ParallaxController : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float cameraPositionChangeX, float cameraPositionChangeY);
    private ParallaxCameraDelegate onCameraMove;
    Vector2 oldCameraPosition;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();
    Camera cam;
    [SerializeField] GameObject SCB;

    private void Awake()
    {
        cam = Camera.main;
       cam.transform.position = new Vector2(SCB.transform.position.x, cam.transform.position.y);
        onCameraMove += MoveLayer;
        FindLayers();
        

        oldCameraPosition.x = cam.transform.position.x;
        oldCameraPosition.y = cam.transform.position.y;
    }

    private void Update()
    {
        if (cam.transform.position.x != oldCameraPosition.x || (cam.transform.position.y) != oldCameraPosition.y)
        {
   
            if (onCameraMove != null)
            {
                Vector2 cameraPositionChange;
                cameraPositionChange = new Vector2(oldCameraPosition.x - cam.transform.position.x, oldCameraPosition.y - cam.transform.position.y);
                onCameraMove(cameraPositionChange.x, cameraPositionChange.y);
            }
            
            oldCameraPosition = new Vector2(cam.transform.position.x, cam.transform.position.y);
        }
        
    }

    //Finds all the objects that have a ParallaxLayer component, and adds them to the parallaxLayers list.
    void FindLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < SCB.transform.childCount; i++)
        {
            ParallaxLayer layer = SCB.transform.GetChild(i).GetComponent<ParallaxLayer>();
        
            if (layer != null)
            {
                parallaxLayers.Add(layer);
            }
        }
    }

    //Move each layer based on each layers position. This is being used via the ParallaxLayer script
    void MoveLayer(float positionChangeX, float positionChangeY)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.MoveLayer(positionChangeX, positionChangeY);

            
        }
    }
}