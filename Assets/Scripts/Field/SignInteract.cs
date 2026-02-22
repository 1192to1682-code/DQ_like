using UnityEngine;

public class SignInteract : MonoBehaviour,IInteractable
{
    [TextArea]
    public string Message = "ここは　はじまりの　むら　です";
    public void Interact()
    {
        //Debug. Log($"[Sign]{Message}");
        DialogUI.Instance.ShowSimpleMessage(Message);
    }
    

}
