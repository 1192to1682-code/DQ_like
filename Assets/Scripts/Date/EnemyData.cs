using UnityEngine;
[CreateAssetMenu(menuName ="DQ-Like/Battle/EnemyData",
    fileName= "Enemy_")]
public class EnemyData :ScriptableObject
{
    public int EnemyID;//¯•Ê—p(0,1,2)
        public string DisplayName;//“G‚Ì•\¦–¼
    public float MaxHP;//Å‘å‘Ì—Í
    public float AttackMin;//Å¬UŒ‚—Í
    public float AttackMax;//Å‘åUŒ‚—Í

    [Header("battle Visual")]
    public GameObject ModelPrefab;
    public Vector3 ModelPosition = new Vector3(0, 0, 2f);//ˆÊ’u
    public Vector3 ModelRotation = new Vector3(0, 180f, 0);//
    public Vector3 ModelScale = Vector3.one;

    [Header("Reward")]
    public int ExReward = 5;//ŒoŒ±’l



    [TextArea(2, 4)]
    public string Description;//“G‚É‚Â‚¢‚Ä‚Ìà–¾


   
}
