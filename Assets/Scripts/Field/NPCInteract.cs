using UnityEngine;
using UnityEngine.Events;
public class NPCInteract : MonoBehaviour,IInteractable
{

    public enum NPCType
    {
Incalid =-1,
NPC,
Shop

    }

    public NPCType Type=NPCType.NPC;

    public DialogDate FirstDialogDate;
    public DialogDate AfterDialogDate;
    public DialogDate HasKeyDialogDate;

    /// <summary>
    /// �O���烆�j�e�B�[�̃C�x���g����������
    /// </summary>
    public UnityEvent NPCEvent;

    public UnityEvent NPCShopEvent;

    public void Interact()
     {
        if (DialogUI.Instance != null &&
            DialogUI.Instance.TryNextIfOpen())
        {
            return;
        
        }

        //商人だったら、ダイアログと同時にShopCanvasを表示する
if(Type == NPCType.Shop)
{
    DialogUI.Instance.Show(FirstDialogDate);

NPCShopEvent.Invoke();
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
