using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public class AnimationFunctions : MonoBehaviour
{
    [HideInInspector] public SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    private void Start()
    {
        //Traversing
        skeletonAnimation.state.Data.SetMix("Traversing/Idle", "Traversing/Run", 0.1f);
        skeletonAnimation.state.Data.SetMix("Traversing/Idle", "Traversing/Jump", 0.1f);
        skeletonAnimation.state.Data.SetMix("Traversing/Idle", "Traversing/Land", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Idle", "Traversing/Roll", 0.05f);
        skeletonAnimation.state.Data.SetMix("Traversing/Run", "Traversing/Idle", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Run", "Traversing/Jump", 0.1f);
        skeletonAnimation.state.Data.SetMix("Traversing/Run", "Traversing/Land", 0.05f);
        skeletonAnimation.state.Data.SetMix("Traversing/Run", "Traversing/Roll", 0.05f);
        skeletonAnimation.state.Data.SetMix("Traversing/Jump", "Traversing/Land", 0.04f);
        skeletonAnimation.state.Data.SetMix("Traversing/Jump", "Traversing/Fall", 0.03f);
        skeletonAnimation.state.Data.SetMix("Traversing/Fall", "Traversing/Land", 0.02f);
        skeletonAnimation.state.Data.SetMix("Traversing/Land", "Traversing/Run", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Land", "Traversing/Idle", 0.3f);
        skeletonAnimation.state.Data.SetMix("Traversing/Land", "Traversing/Jump", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Land", "Traversing/Roll", 0.05f);
        skeletonAnimation.state.Data.SetMix("Traversing/Roll", "Traversing/Jump", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Roll", "Traversing/Idle", 0.3f);
        skeletonAnimation.state.Data.SetMix("Traversing/Roll", "Traversing/Run", 0.3f);
        skeletonAnimation.state.Data.SetMix("Traversing/Roll", "Traversing/Land", 0.2f);
        skeletonAnimation.state.Data.SetMix("Traversing/Roll", "Traversing/Fall", 0.2f);

        //Combat
        skeletonAnimation.state.Data.SetMix("Combat/UlfSummoningStart", "Combat/UlfSummoningHold", 0.05f);
        skeletonAnimation.state.Data.SetMix("Combat/UlfSummoningHold", "Combat/UlfSummoningEnd", 0.03f);

    }

    public bool PlayAnimation(int track, string name, bool loop = false, float timeScale = 1f)
    {
        
        if (!skeletonAnimation) skeletonAnimation = GetComponent<SkeletonAnimation>();
      
       if (!skeletonAnimation.AnimationName.Equals(name) || name.Equals("Land"))
       {
            try
            {
                skeletonAnimation.state.SetAnimation(track, name, loop);
                skeletonAnimation.AnimationState.TimeScale = timeScale;
            }
            catch (System.ArgumentException message)
            {
                Debug.LogError(message);
               return false;
            }
            return true;
     }
     return false;
    }

    
    public bool PlayNextAnimation(int track, string name, string nextName)
    {
        if (PlayAnimation(track, name))
        {
            skeletonAnimation.state.AddAnimation(track, nextName, loop: true, 0f);
            return true;
        }
        return false;
    }
   
}
