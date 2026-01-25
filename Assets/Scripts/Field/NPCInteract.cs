using UnityEngine;
using UnityEngine.Events;
public class NPCInteract : MonoBehaviour,IInteractable
{
    public string NPCName = "村人A";

    [TextArea]
    public string TalkMessage = "こんにちは";

    /// <summary>
    /// 外からユニティーのイベント処理をする
    /// </summary>
    public UnityEvent NPCEvent;

    public void Interact()
    { 
    Debug.Log ($"[NPC]{TalkMessage}私は{NPCName}です");
        
        //NPCEventが設定されていれば（nullじゃなかったら）,
        //設定された処理を発動する。
        NPCEvent?.Invoke();


    }


}
