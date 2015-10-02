using UnityEngine;
using System.Collections;

public class FruitCompassController : MonoBehaviour
{
    public Transform Target;
    public float RotationSpeed;
    
    private Quaternion lookRotation;
    private Vector3 direction;

    // Update is called once per frame
    void Update()
    {
        transform.position = SnakeController.instance.SnakeHead.transform.position + SnakeController.instance.forV;
        
        direction = (Target.transform.position - transform.position);
        lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10000);
    }
}
