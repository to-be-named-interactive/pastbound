using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Attack : InteractBase
{
    public void PerformAttack(int dir)
    {
        // Cast to detect objects in the attack range
        /*CastOverlapCircle(dir, true);

        // Check if there are any objects within range
        if (hasObjectsEntered)
        {
            foreach (var collider in colliderObjects)
            {
                /Skip if collider or required components are missing
                if (collider == null) continue;

                var enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.GetHurt(4, 10, 5, dir, 0.10f, 0.10f);

                    // Check and run freeze animation if SkeletonAnimation is present
                    var skeletonAnimation = collider.GetComponent<SkeletonAnimation>();
                    if (skeletonAnimation != null)
                    {
                        StartCoroutine(enemy.FreezeAnimationFrame(0.1f, 0.2f));
                    }
                }
            }
        }*/
    }
}