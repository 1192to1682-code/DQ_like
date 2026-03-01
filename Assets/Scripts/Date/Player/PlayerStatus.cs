using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Status/PlayerStatus",
    fileName= "PlayerStatus_")]
public class PlayerStatus : ScriptableObject
{
    [Header("プレイヤーの状態")]
    public int Level = 1;
    public int Exp = 0;

    [Header("プレイヤーのステータス")]
    public float MaxHP = 30;
    public float AttackMin = 3;
    public float AttackMax = 6;
    
    /// <summary>
    ///プレイヤーステータスの初期化 
    /// </summary>
    public void ResetToDefault()
    {

        Level = 1;
        Exp = 0;
        MaxHP = 30;
        AttackMin = 3;
        AttackMax = 6;


    }

}
