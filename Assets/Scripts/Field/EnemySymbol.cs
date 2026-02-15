using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemySymbol : MonoBehaviour
{
   private string BattleSceneName = "BattleScene";

    private void OnTriggerEnter(Collider other)
    {
        //Player‚ÌTagˆÈŠO‚ÌGameObject‚ªN“ü‚µ‚Ä‚«‚½‚ç‰½‚à‚µ‚È‚¢
        if (!other.CompareTag("Player"))
        {
            return;

        }

        SceneManager. LoadScene(BattleSceneName);
        
    }
}
