using UnityEngine;

public class SwordButton : MonoBehaviour
{
    [SerializeField] private Door door;

    private Bala espadaActual;

    public void SetSword(Bala espada)
    {
        if (espadaActual != null) return;

        espadaActual = espada;
        door.OpenDoor();
    }

    public void RemoveSword(Bala espada)
    {
        if (espadaActual != espada) return;

        espadaActual = null;
        door.CloseDoor();
    }
}



