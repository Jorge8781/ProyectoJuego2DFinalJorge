using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnID;

    void Start()
    {
        if (SpawnManager.spawnPointID == spawnID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = transform.position;
        }
    }
}
