using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Invoke("DelayStartGame", 1.1f);

        Player.life = 3;
    }

    private void DelayStartGame()
    {
        SceneManager.LoadScene("遊戲畫面");

      //  Player.life = 3;
    }

    public void QuitGame()
    {
        Invoke("DelayStaitGame", 1.1f);
    }

    public void DelayStaitGame()
    {
       Application.Quit();
    }
}
