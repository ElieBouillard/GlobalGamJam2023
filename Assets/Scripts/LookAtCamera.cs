using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if(Camera.main == null) return;
        transform.forward = Camera.main.transform.forward;
    }
}
