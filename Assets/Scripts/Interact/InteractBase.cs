using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBase : MonoBehaviour
{
    //resetingPosition bool will switch on and off the ability to see the overlap circle
    public bool resetingPosition = false;

    //setting OverlapCircle's position and radius
    [SerializeField] private Vector3 circlePosition; 
    [SerializeField] float interactCirleRadius;

    //The layer that the OverlapCircle must detect gameobjects
    [SerializeField] public LayerMask layerMask;

    //Has the OverlapCircle detected a object in itself
    [HideInInspector] public bool hasObjectsEntered;

    //The Collider or (if is used for Attack purposes) the colliders of OverlapCircle detection 
    [HideInInspector] public Collider2D colliderObject;
    [HideInInspector] public Collider2D[] colliderObjects;

    //The calculated position of position, radius, and if must we change the sign of the value 
    Vector3 newCirclePosition = Vector3.zero;

    //The method that does the OverlapCircle casting  
    public void CastOverlapCircle(int XaxisSign = 0, bool usedForAttack = false)
    {

        if(usedForAttack)
        {
            newCirclePosition = new Vector3(transform.position.x + circlePosition.x * XaxisSign, transform.position.y + circlePosition.y, 0);
            colliderObjects = Physics2D.OverlapCircleAll(newCirclePosition, interactCirleRadius, layerMask);
 
            if (!resetingPosition) newCirclePosition = Vector3.zero;
        }
        else
        {
            newCirclePosition = new Vector3(transform.position.x + circlePosition.x, transform.position.y + circlePosition.y, 0);
            colliderObject = Physics2D.OverlapCircle(newCirclePosition, interactCirleRadius, layerMask);
        }


        if (usedForAttack)
        {
            if (colliderObjects != null)
            {
                if (hasObjectsEntered == false) hasObjectsEntered = true;

            }
            else
            {
                if (hasObjectsEntered == true) hasObjectsEntered = false;
            }
        }
        else
        {
            if (colliderObject != null)
            {
                if (hasObjectsEntered == false) hasObjectsEntered = true;

            }
            else
            {
                if (hasObjectsEntered == true) hasObjectsEntered = false;
            }
        }
        

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if(resetingPosition) newCirclePosition = transform.position + circlePosition;
        Gizmos.DrawWireSphere(newCirclePosition, interactCirleRadius);
       
    }
}
