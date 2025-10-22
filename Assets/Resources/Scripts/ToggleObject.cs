using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    [SerializeField] private GameObject objeto; // El objeto que quieres activar/desactivar

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            // Cambia el estado activo del objeto
            objeto.SetActive(!objeto.activeSelf);
        }
    }
}