using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStartButtonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");
    }
    
    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
