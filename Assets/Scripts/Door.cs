using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedPos;
    private bool isOpen = false;

    void Start()
    {
        closedPos = transform.position;
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        StartCoroutine(OpenRoutine());
    }


    IEnumerator OpenRoutine()
    {
        Vector3 targetPos = closedPos + Vector3.up * openHeight;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                openSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void CloseDoor()
    {
        if (!isOpen) return;

        isOpen = false; // 🔥 CLAVE
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }


    IEnumerator CloseRoutine()
    {
        while (Vector3.Distance(transform.position, closedPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                closedPos,
                openSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

}


