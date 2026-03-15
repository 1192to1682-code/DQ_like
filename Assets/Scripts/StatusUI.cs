using UnityEngine;
using TMPro;
public class StatusUI : MonoBehaviour
{
    public GameObject Root;

    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI DefenceText;
    public TextMeshProUGUI WeaponText;
    public TextMeshProUGUI ArmorText;

        
    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Field_01")
        {
            Close();
        }
    }
    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(UnityEngine.SceneManagement.Scene current, UnityEngine.SceneManagement.Scene next)
    {
        if (next.name != "Field_01")
        {
            Close();
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (PlayerState.Instance == null)
        {
            return;

        }

        LevelText.text =
            $"LV{PlayerState.Instance.PlayerStatus.Level}";

        HPText.text =
            $"HP{PlayerState.Instance.PlayerStatus.MaxHP}";

        AttackText.text =
            $"‚±‚¤‚°‚«:{PlayerState.Instance.AttackMax}" +
            $"/{PlayerState.Instance.AttackMin}";

        DefenceText.text =
                    $"‚Ú‚¤‚¬‚å:{PlayerState.Instance.Defence}";

        if (EquipmentManager.Instance != null)
        {
            WeaponText.text =
                $"‚Ô‚«:{EquipmentManager.Instance.EquipmentWeapon.DisplayName}";

            ArmorText.text =
                $"‚Ú‚¤‚®:{EquipmentManager.Instance.EquipmentArmor.DisplayName}";
        }

    }

    public void Open()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Field_01") return;
        gameObject.SetActive(true);
        Root.SetActive(true);
        Refresh();
      }

    public void Close()
    {
        Root.SetActive(false);
        gameObject.SetActive(false);
    }

}


