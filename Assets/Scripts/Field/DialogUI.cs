using UnityEngine;
using TMPro;
using System.Collections;

public class DialogUI : MonoBehaviour
{
    /// <summary>
    /// どこからでもアクアセスできるようにstaticで宣言する。
    /// </summary>
    public static DialogUI Instance;

    public GameObject Panel;

    public TextMeshProUGUI Nametext;

    public TextMeshProUGUI Messagetext;


    [Header("▼などのNextHint(Optional)")]
    public GameObject NextHint;

    public GameObject YesNoButtonBG;


    /// <summary>
    /// 1文字を待つ時間
    /// </summary>
    private float charInterval= 0.2f;

    private string[] currentLines;

    /// <summary>
    /// 何行目か？
    /// </summary>
    private int lineIndex;


    private Coroutine typingCoroutine;

    private bool isTyping = false;


    private void Awake()

    {

        Instance = this;
        Panel.SetActive(false);

        if (NextHint != null)
        {

            NextHint.SetActive(false);
        }


    }


    /// <summary>
    /// <>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="message"></param>

    public void Show(DialogDate dialogDate)
     {

        if (dialogDate == null ||
            dialogDate.MessageLines == null ||
            dialogDate.MessageLines.Length == 0)
        {
            Debug.LogWarning("DialogDateが不正です");
            return;
        
        }


        //boolの値を直接GameObjectのActiveの値に変更する
        YesNoButtonBG.SetActive(dialogDate.ShowYesNo);
        GameState.IsDialogOpen = true;
        Nametext.text = dialogDate.Speaker;

        currentLines = dialogDate.MessageLines;
        lineIndex = 0;
        
        Panel.SetActive(true);

        ShowLine(lineIndex);

    }


    /// <summary>
    /// 外部からアイテム取得のダイアログを見せる
    /// </summary>
    /// <param name="dialogDate"></param>
    public void ShowItemDialog(string dialogMessage)
    {

        if (dialogMessage == string.Empty)
        {
            Debug.LogWarning("DialogMessageがないです");
            return;

        }


        //boolの値を直接GameObjectのActiveの値に変更する
        YesNoButtonBG.SetActive(false);
        GameState.IsDialogOpen = true;
        Nametext.text = string.Empty;

        string[] itemLines = new string[1];
        itemLines[0] =$"{dialogMessage}を手に入れた";

        currentLines =itemLines;
        lineIndex = 0;


        Panel.SetActive(true);

        ShowLine(lineIndex);

    }


    /// <summary>
    /// UIのcloseから呼び出し
    /// </summary>

    public void Close()
    {
        StopTypingIfNeeded  ();
        GameState.IsDialogOpen = false;
        Panel.SetActive(false);

        if (NextHint != null)
        {

            NextHint.SetActive(false);

        }
        currentLines = null;
        lineIndex = 0;
    }

    public void Next()
    
    { 
    
        if (!Panel.activeSelf)
        {
            return;

        }

        if (isTyping)
        {
            FinishCurrentLineInstant();
            return;
        
        }
        lineIndex++;//インクリメント(n+lすること)

        if (currentLines != null &&
                lineIndex < currentLines.Length)
        {
            ShowLine(lineIndex);


        }

        else
        {
            Close();
        
        }

    }

    private void ShowLine(int index) 
    
    {
        StopTypingIfNeeded();

        //今表示されているものを空にする
    Messagetext.text = string.Empty;
    if(NextHint !=null)
        {
            NextHint.SetActive(true);

        }

        typingCoroutine = StartCoroutine(TypeLine(currentLines[index]));
    
    }

    private IEnumerator TypeLine(string line)
    {

        isTyping = true;
        //引数の行の中の1文字を取り出していく
        foreach (char c in line)
        {
            Messagetext.text += c;
            yield return new WaitForSeconds(charInterval);

        }

        isTyping = false;

        if (NextHint != null)
        {
            NextHint.SetActive(true);
        
        }

        //Next();
        typingCoroutine = null;
    }

    private void FinishCurrentLineInstant()
    {
        if (currentLines == null)
        {

            return;
        }

        StopTypingIfNeeded ();
        Messagetext.text = currentLines[lineIndex];

        isTyping = false;

        if (NextHint != null)
        {
            NextHint.SetActive(true);
        
        }
            
    }

    public void ShowSimpleMessage(string message)
    {
        if (message== string.Empty)
        {
            Debug.LogWarning("messageが未取得です");
            return;
        }

        StopTypingIfNeeded();

        // Yes / No は表示しない
        YesNoButtonBG.SetActive(false);

        GameState.IsDialogOpen = true;

        // 名前欄は空
        Nametext.text = string.Empty;

        // 1行だけの配列として扱う
        currentLines = new string[] { message };
        lineIndex = 0;

        Panel.SetActive(true);

        ShowLine(lineIndex);
    }

    private void StopTypingIfNeeded()
    {
        if (typingCoroutine != null)
        { StopCoroutine(typingCoroutine);

            isTyping = false;
        }

            }
    public void OnYes()
    {
        Close();

    }
    public void OnNo()
    {
        Close();

    }

}
