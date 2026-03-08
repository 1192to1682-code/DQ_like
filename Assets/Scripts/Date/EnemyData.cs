using UnityEngine;
[CreateAssetMenu(menuName ="DQ-Like/Battle/EnemyData",
    fileName= "Enemy_")]
public class EnemyData :ScriptableObject
{
    public int EnemyID;//識別用(0,1,2)
    public string DisplayName;//敵の表示名
    public float MaxHP;//最大体力
    public float AttackMin;//最小攻撃力
    public float AttackMax;//最大攻撃力

    [Header("battle Visual")]
    public GameObject ModelPrefab;
    public Vector3 ModelPosition = new Vector3(0, 0, 2f);//位置
    public Vector3 ModelRotation = new Vector3(0, 180f, 0);//
    public Vector3 ModelScale = Vector3.one;

    public Vector3 UIOffset = new Vector3(0, 2.5f, 0); // 頭上UIのオフセット

    [Header("Reward")]
    public int ExpReward = 5;//経験値



    [TextArea(2, 4)]
    public string Description;//敵についての説明


   
}
