using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Attack : InteractBase
{

    public void PerformAttack(int dir)
    {

        CastOverlapCircle(dir, true);

        if (hasObjectsEntered)
        {
         
            foreach (var collider in colliderObjects)
            {
                if (collider != null)
                {

                    collider.GetComponent<EnemyBase>().GetHurt(4, 10, 5, dir, 0.10f, 0.10f);

                    if (collider.GetComponent<SkeletonAnimation>() != null)
                    {
                        StartCoroutine(collider.GetComponent<EnemyBase>().FreezeAnimationFrame(0.1f, 0.2f));
                    }

                }

            }

        }
       
    }
}
