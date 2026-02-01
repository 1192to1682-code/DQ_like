using UnityEngine;

[CreateAssetMenu(menuName ="DQ-Like/Dialog/DiakogDate",
    fileName = "DialogDate_")
    
    ]

public class DialogDate:ScriptableObject
{

    public string Speaker;

    /// <summary>
    /// ダイアグに表示するテキスト
    /// </summary>

    [TextArea(2, 4)]
    public string[] MessageLines;


    /// <summary>
    /// 選択しを出すかどうかのフラグ
    /// </summary>
    public bool ShowYesNo = false;


    /// <summary>
    /// イエスを押した時のゲーム進攻上のフラグ
    /// </summary>
    public int SetFlagOnYes;

}
