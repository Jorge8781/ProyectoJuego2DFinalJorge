using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToBoss : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("ENTRÉ AL PORTAL");
            SceneManager.LoadScene("BossScene");
        }
    }
}


