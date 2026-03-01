using UnityEngine;

[CreateAssetMenu(menuName = "DQ-Like/Status/ExpTable",
    fileName= "ExpTable_")]
public class ExpTable : ScriptableObject
{

    [Tooltip("index = Level(1始まり想定)")]
    public int[] NeedExp;
    /// <summary>
    /// 現在のレベルを引数に与えると
    /// 次のレベルに必要なExpを返してくれる
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetNeedExpToNext(int currentLevel) { 

        if(NeedExp ==null ||NeedExp.Length ==0)
        {
            return 999999;

        }

        if (currentLevel < 0)
        {
            currentLevel = 0;

        }

        if(currentLevel >=NeedExp.Length)
        {
            return NeedExp[NeedExp.Length -1];

        }
        return NeedExp[currentLevel];

    }


}
