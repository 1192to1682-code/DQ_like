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

    [TextArea(2, 4)]
    public string Description;//“G‚É‚Â‚¢‚Ä‚Ìà–¾


   
}
