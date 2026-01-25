using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// カメラが追従するターゲット(Player)
    /// </summary>
    public Transform Target;

    /// <summary>
    /// ターゲットをどこから見るか
    /// </summary>
    public Vector3 Offset = new Vector3(0, 4, -6f);

    public float FollowSpeed = 10f;

    public float LookHeight = 1.5f;

    /// <summary>
    /// Updateが実行されたあとに実行される処理を各場所
    /// </summary>
    private void LateUpdate()
    {

        if (Target == null)
        {
            return;
        }

        Vector3 desired = Target.position + Offset;
        
        //カメラのPositionを更新する
        transform.position = Vector3.Lerp(
            transform.position,
            desired,
            FollowSpeed * Time.deltaTime
            );

        //カメラの回転も更新する
        Vector3 lookPos = Target.position + Vector3.up * LookHeight;
        transform.LookAt(lookPos);

    }

    
}
