using UnityEngine;

public class pauseScreen : MonoBehaviour
{
    public GameObject gameHUD;
    public GameObject startScreen;

    void Awake()
    {
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        gameHUD.SetActive(true);
        startScreen.SetActive(false);
        Time.timeScale = 1;
    }
}
