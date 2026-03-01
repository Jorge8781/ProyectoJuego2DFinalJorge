using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform cam;
    private Vector3 startPos;
    private float startCamPosX;
    private float startCamPosY;

    public float distanceX = 0.5f;
    public float distanceY = 0.5f;

    void Start()
    {
        cam = Camera.main.transform;

        startPos = transform.position;
        startCamPosX = cam.position.x;
        startCamPosY = cam.position.y;
    }

    void LateUpdate()
    {
        float movementX = (cam.position.x - startCamPosX) * distanceX;
        float movementY = (cam.position.y - startCamPosY) * distanceY;

        transform.position = new Vector3(
            startPos.x + movementX,
            startPos.y + movementY,
            startPos.z
        );
    }
}