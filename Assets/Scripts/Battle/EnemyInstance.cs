using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class EnemyInstance
{
    public EnemyData Data;
    public float CurrentHP;
    public GameObject ModelInstance;
    public Animator Animator;
    public GameObject UIInstance; // 頭上のUI

    public EnemyInstance(EnemyData data)
    {
        Data = data;
        CurrentHP = data.MaxHP;
    }

    public bool IsDead => CurrentHP <= 0;

    public void DestroyModel()
    {
        if (ModelInstance != null)
        {
            UnityEngine.Object.Destroy(ModelInstance);
        }
    }
}
