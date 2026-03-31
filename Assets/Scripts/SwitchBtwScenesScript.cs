using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.SceneManagement;

public class SwitchBtwScenesScript : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("Button List")]
    Button exitButton;

    void Awake()
    {
        // This prevents the screen from dimming or going to sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    
    // void OnEnable()
    // {
    //     if(exitButton != null)
    //     {
    //         exitButton.onClick.AddListener(ExitApplication);
    //     }
    // }

    // void OnDisable()
    // {
    //     if(exitButton != null)
    //     {
    //         exitButton.onClick.RemoveListener(ExitApplication);
    //     }
    // }

    public void ExitApplication()
    {
        Application.Quit();
        // SceneManager.LoadScene("ARMetaverseScene"); 
    }
}
