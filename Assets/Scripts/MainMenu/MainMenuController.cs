using UnityEngine;

public class MainMenuController : Singleton<MainMenuController>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStartButtonPressed()
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
        PlayerController.Instance.IsDead = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
    }
    
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
