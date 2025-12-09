using UnityEngine;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] private Transform ControladorDisparo;
    [SerializeField] private GameObject bala;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1")){
            //disparar
            Disparar();
        }
    }

    private void Disparar ()
    {
        Instantiate(bala, ControladorDisparo.position, ControladorDisparo.rotation);
    }
}
