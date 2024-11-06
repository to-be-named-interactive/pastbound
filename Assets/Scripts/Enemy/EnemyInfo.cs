using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;

    [Space(5)]

    [Header("Enemy Health")]
    public float enemyMaxHealth;

    [Space(5)]

    [Header("Enemy Stamina")]
    public float enemyMaxStamina;

    [Space(5)]

    [Header("Enemy Defence")]
    public float defencePower;
    public bool hasShield;
}