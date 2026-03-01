using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string portalID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnManager.spawnPointID = portalID;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}


