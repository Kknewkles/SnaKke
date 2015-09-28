using UnityEngine;
using System.Collections;

public class SnakeKiller : MonoBehaviour
{
    GameObject popupManagerObject;
    PopupManager popupManager;

    void Start()
    {
        popupManagerObject = GameObject.FindWithTag("PopupManager");
        popupManager = popupManagerObject.GetComponent<PopupManager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        popupManager.ShowDeathScreen();
    }
}
