using UnityEngine;
using System.Collections;

public class FruitRotator : MonoBehaviour
{
	void Update()
    {
        transform.Rotate(new Vector3(15, 30, 15) * Time.deltaTime);
	}
}
