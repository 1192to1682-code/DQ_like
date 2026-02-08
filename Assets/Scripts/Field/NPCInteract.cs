using UnityEngine;
using UnityEngine.Events;
public class NPCInteract : MonoBehaviour,IInteractable
{
    public DialogDate FirstDialogDate;
    public DialogDate AfterDialogDate;
    public DialogDate HasKeyDialogDate;

    /// <summary>
    /// 外からユニティーのイベント処理をする
    /// </summary>
    public UnityEvent NPCEvent;

    public void Interact()
     {
        if (DialogUI.Instance != null &&
            DialogUI.Instance.TryNextIfOpen())
        {
            return;
        
        }


        if (QuestFlag.HasKey)
        {
            DialogUI.Instance.Show(HasKeyDialogDate);

        }

        else if (!QuestFlag.TalkedToVillager)

        {

            DialogUI.Instance.Show(FirstDialogDate);
            QuestFlag.TalkedToVillager = true;

        }

        else
        {
            DialogUI.Instance.Show(AfterDialogDate);

        }

        if (!QuestFlag.HasKey)
        {
            return;
        }

 
        NPCEvent?.Invoke();


    }


}
