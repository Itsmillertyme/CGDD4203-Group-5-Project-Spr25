using UnityEngine;

public class pauseScreen : MonoBehaviour
{
    public GameObject gameHUD;
    public GameObject startScreen;

    void Awake()
    {
        gameHUD.SetActive(false);
        startScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        gameHUD.SetActive(true);
        startScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
