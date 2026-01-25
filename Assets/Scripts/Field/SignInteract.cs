using UnityEngine;

public class SignInteract : MonoBehaviour,IInteractable
{
    [TextArea]
    public string Message = "‚±‚±‚Í@‚Í‚¶‚Ü‚è‚Ì@‚Ş‚ç@‚Å‚·";
    public void Interact()
    {
        Debug. Log($"[Sign]{Message}");
    }
    

}
