using UnityEngine;
using UnityEngine.Events;
public class NPCInteract : MonoBehaviour,IInteractable
{
    public DialogDate FirstDialogDate;
    public DialogDate AfterDialogDate;
    /// <summary>
    /// 外からユニティーのイベント処理をする
    /// </summary>
    public UnityEvent NPCEvent;

    public void Interact()
    {
        if (!QuestFlag.TalkedToVillager)
        {

            DialogUI.Instance.Show(FirstDialogDate);
            QuestFlag.TalkedToVillager = true;

        }

        else
        {
            DialogUI.Instance.Show(AfterDialogDate);

        }

 
        NPCEvent?.Invoke();


    }


}
