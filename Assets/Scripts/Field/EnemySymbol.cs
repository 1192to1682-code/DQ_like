using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemySymbol : MonoBehaviour
{
   private string BattleSceneName = "BattleScene";

   [Header("Movement Settings")]
   public float Speed = 3.0f;
   public float DetectionRange = 10.0f;
   public float StopDistance = 1.5f;

   private Transform playerTransform;

   private void Start()
   {
       GameObject player = GameObject.FindGameObjectWithTag("Player");
       if (player != null)
       {
           playerTransform = player.transform;
       }
   }

   private void Update()
   {
       // If dialog is open, do not move
       if (GameState.IsDialogOpen)
       {
           return;
       }

       if (playerTransform == null)
       {
           return;
       }

       float distance = Vector3.Distance(transform.position, playerTransform.position);

       if (distance <= DetectionRange && distance > StopDistance)
       {
           // Rotate towards player
           Vector3 direction = (playerTransform.position - transform.position).normalized;
           direction.y = 0; // Keep rotation on Y axis only
           
           if (direction != Vector3.zero)
           {
               Quaternion lookRotation = Quaternion.LookRotation(direction);
               transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
           }

           // Move towards player
           transform.position += transform.forward * Speed * Time.deltaTime;
       }
   }

    private void OnTriggerEnter(Collider other)
    {
        //PlayerのTag以外のGameObjectが侵入してきたら何もしない
        if (!other.CompareTag("Player"))
        {
            return;

        }

        SceneManager. LoadScene(BattleSceneName);
        
    }
}
