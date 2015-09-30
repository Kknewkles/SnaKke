using UnityEngine;
using System.Collections;

public class SnakeKiller : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PopupManager.instance.DeathScreen_Show();
    }
}
