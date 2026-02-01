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

        GameState.IsDialogOpen = true;
        Nametext.text = name;

        currentLines = dialogDate.MessageLines;
        lineIndex = 0;
        
        Panel.SetActive(true);

        ShowLine(lineIndex);

    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    /// <summary>
    /// UIのcloseから呼び出し
    /// </summary>

    public void Close()
    {
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

        Next();
        typingCoroutine = null;
    }

    private void FinishCurrentLineInstant()
    {
        if (currentLines == null)
        {

            return;
        }

        Messagetext.text = currentLines[lineIndex];

        isTyping = false;

        if (NextHint != null)
        {
            NextHint.SetActive(true);
        
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
