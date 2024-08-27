using UnityEngine;


public class RespawnButton : MonoBehaviour
{
    public void Respawn()
    {
        Debug.Log("Respawn");
        //Reload the current Scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}