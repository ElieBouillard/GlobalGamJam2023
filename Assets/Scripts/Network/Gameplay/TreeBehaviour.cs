using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (hitData.Team != Team.Enemy) return;
        
        _renderer.material.color = Color.red;
        StartCoroutine(ResetColor());
        _currLife -= _initialLife;

        Debug.Log($"Tree Take Damage : Current Life -> {_currLife}");
        
        if (_currLife <= 0)
        {
            Debug.Log("Tree Is DEAD / GAME OVER");
        }
    }
    
    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.25f);
        _renderer.material.color = Color.white;
    }
}