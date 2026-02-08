using UnityEngine;
using System.Collections;

public class ChestInteract : MonoBehaviour,IInteractable
{
    [Header("íÜêg")]
    public ItemDate RewardItem;

    [Header("ââèo")]
    public Animator ChestAnimator;
    public string OpenTriggerName = "Open";

    [Header("èÛë‘")]
    public bool IsOpened = false;

    [Header("åÆÇ™Ç»Ç¢Ç∆Ç†Ç©Ç»Ç¢")]
    public bool IsLocked = false;

    [Header("ìñÇΩÇËîªíË")]
    public Collider InteractCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    if (ChestAnimator == null)
        ChestAnimator = this.transform.GetComponentInChildren<Animator>();

    if(InteractCollider == null)
        {
                InteractCollider = GetComponent<Collider>();
        }
    }

    public void Interact()
    {

        if (IsOpened)
        {
            return;
        }

        if (IsLocked)
        {
            if (!QuestFlag.HasKey)
            {
                DialogUI.Instance.ShowSimpleMessage("åÆÇ™Ç»Ç¢Ç≈Ç∑");
                return;
            }
        }

        StartCoroutine(OpenRoutine());

    }


        private IEnumerator OpenRoutine()
               
            {
            IsOpened = true;

        if (RewardItem.ItemName != null &&
            RewardItem.ItemName == "ñúî\åÆ")
        {
            QuestFlag.OpenedChestA = true;
            QuestFlag.HasKey = true;
        }
        else
        {
            QuestFlag.OpenedChestA = true;

        }


        if(InteractCollider !=null)
            {
            InteractCollider.enabled = false;
        
        }

        if (ChestAnimator != null)
        {
            ChestAnimator.ResetTrigger(OpenTriggerName);
            ChestAnimator.SetTrigger(OpenTriggerName);
        }

        yield return new WaitForSeconds(1f);
        if (InventryManager.Instance != null)
        {
            InventryManager.Instance.Add(RewardItem);

        }
        
        if(DialogUI .Instance != null)
        {
            DialogUI.Instance.ShowItemDialog(RewardItem.ItemName);
         }
        

        }

       
}
