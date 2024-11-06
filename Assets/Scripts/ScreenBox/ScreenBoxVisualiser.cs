using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoxVisualiser : MonoBehaviour
{
    [SerializeField] private List<GameObject> allScreenBoxes = new List<GameObject>();
    List<PolygonCollider2D> screenBoxColliders = new List<PolygonCollider2D>();

    private bool screenBoxesSorted = false;

    private void Start()
    {
        if (allScreenBoxes == null) return;

        // if there are screenboxes get every single ones their colliders
        for (int i = 0; i < allScreenBoxes.Count; i++)
        {
            if(allScreenBoxes[i] != null)
            {
                if (allScreenBoxes[i].GetComponent<PolygonCollider2D>() != null)
                {
                    screenBoxColliders.Add(allScreenBoxes[i].GetComponent<PolygonCollider2D>());
                }
            }
        }   
    }

    void Update()
    {
        // Locate in which screenbox the player is
        if (Player.Instance.nextScreenBox != null && !screenBoxesSorted)
        {
            for (int i = 0; i < allScreenBoxes.Count; i++)
            {
                if (allScreenBoxes[i] != Player.Instance.nextScreenBox)
                {
                    if (allScreenBoxes[i].transform.GetChild(0).gameObject.activeSelf)
                    {
                        allScreenBoxes[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
            
            screenBoxesSorted = true;
        }
        
        if (screenBoxColliders != null)
        {
            // For each sc collider draw its dimensons so we can see them at all times
            for (int i = 0; i < screenBoxColliders.Count; i++)
            {
                DrawScreenBoxVisualiser(screenBoxColliders[i]);
            }
        } 
    }

    private void DrawScreenBoxVisualiser(PolygonCollider2D collider)
    {
        Bounds bounds = collider.bounds;

        // Width line down
        Vector3 widthLineDownStart = new Vector3(bounds.min.x, bounds.min.y, 0);
        Vector3 widthLineDownEnd = new Vector3(bounds.max.x, bounds.min.y, 0);

        // Width Line Up
        Vector3 widthLineUpStart = new Vector3(bounds.min.x, bounds.max.y, 0);
        Vector3 widthLineUpEnd = new Vector3(bounds.max.x, bounds.max.y, 0);

        // Height Line left
        Vector3 heightLineLeftStart = new Vector3(bounds.min.x, bounds.min.y, 0);
        Vector3 heightLineLeftUpEnd = new Vector3(bounds.min.x, bounds.max.y, 0);

        // Height Line right
        Vector3 heightLineRightStart = new Vector3(bounds.max.x, bounds.min.y, 0);
        Vector3 heightLineRightUpEnd = new Vector3(bounds.max.x, bounds.max.y, 0);
        
        Color color = Color.magenta;

        // Draw width Lines 
        Debug.DrawLine(widthLineDownStart, widthLineDownEnd, color);
        Debug.DrawLine(widthLineUpStart, widthLineUpEnd, color);

        // Draw height Lines
        Debug.DrawLine(heightLineLeftStart, heightLineLeftUpEnd, color);
        Debug.DrawLine(heightLineRightStart, heightLineRightUpEnd, color);
    }
}