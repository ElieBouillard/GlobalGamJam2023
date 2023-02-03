using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
    private Renderer _renderer;
    public static Vector3 Position;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        Position = transform.position;
    }

    public void TakeDamage()
    {
        _renderer.material.color = Color.red;
        StartCoroutine(ResetColor());
        Debug.Log($"{gameObject.name} : Take Damage");
    }

    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.25f);
        _renderer.material.color = Color.white;
    }
}
