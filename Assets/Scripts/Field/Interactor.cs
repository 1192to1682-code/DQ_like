using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerがInterattion(交流)をするためのclass
/// </summary>
public class Interactor : MonoBehaviour
{

    /// <summary>
    /// Distanceの値m先まで交流が行えるよういするための変数
    /// </summary>
    /// 
    public float Distance = 2.0f;
    public LayerMask InteractLayer;
    public float EyeHeight = 0.5f;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        TryInteract();
        Debug.Log("OK");
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position + Vector3.up * EyeHeight;

        //光線を発射する
        Ray ray = new Ray(origin, transform.forward);

        //光線がどうなってるかのデバッグ用の線を表示する
        Debug.DrawRay(origin, transform.forward * Distance,
            Color.yellow, 0.5f);

        //光線を出してみて、当たるかどうかの情報
        if (Physics.Raycast(ray, out RaycastHit hit,
            Distance, InteractLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            interactable?.Interact();
        }
          

    }

    public void OnMessageNext(InputValue value)
    {

        if (!value.isPressed)
        {
            return;

        }

        if (GameState.IsDialogOpen)
        {
            DialogUI.Instance.Next();

        }

    }
}
