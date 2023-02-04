using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour, IHittable
{
    [Header("Parameters")]
    [SerializeField] private int _initialLife;

    private Renderer _renderer;
    public static Vector3 Position;

    private int _currLife;
    
    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
        Position = transform.position;
    }

    public void OnHit(HitData hitData)
    {
        _renderer.material.color = Color.red;
        StartCoroutine(ResetColor());
        _currLife -= _initialLife;

        if (_currLife <= 0)
        {
            
        }
    }
    
    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.25f);
        _renderer.material.color = Color.white;
    }
}