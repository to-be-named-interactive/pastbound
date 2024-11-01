using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : InteractBase
{
    void Update()
    {
        CastOverlapCircle();

        if (hasObjectsEntered)
        {
            Collected();
        }   
    }

    private void Collected()
    {
        GameManager.Instance.CollectCoin();
        Destroy(gameObject);
    }
}
