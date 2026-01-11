using UnityEngine;

public class CrowFollower : MonoBehaviour
{
    public Transform player;
    public Vector2 offset = new Vector2(-1.2f, 1f);
    public float followSpeed = 5f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (player == null) return;

        float dir = Mathf.Sign(player.localScale.x);

        // 🔹 offset dinámico según hacia dónde mira el player
        Vector3 targetPos = player.position + new Vector3(offset.x * dir, offset.y, 0);

        // 🔹 seguir suavemente
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );

        // 🔹 girar el cuervo
        transform.localScale = new Vector3(
            originalScale.x * dir,
            originalScale.y,
            originalScale.z
        );
    }

}

