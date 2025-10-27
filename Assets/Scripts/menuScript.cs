using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   // public GameObject menuPanel; 
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");

    }

}
