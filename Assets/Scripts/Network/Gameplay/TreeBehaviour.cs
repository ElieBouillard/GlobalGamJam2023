using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TreeBehaviour : MonoBehaviour, IHittable
{
    [Header("References")]
    [SerializeField] private TreeView _treeView = null;

    [Header("Parameters")]
    [SerializeField] private int _initialLife;

    private Renderer _renderer;
    public static Vector3 Position;

    private int _currLife;
    
    private void Awake()
    {
        Position = transform.position;
        _currLife = _initialLife;

        if (_treeView == null)
            _treeView = FindObjectOfType<TreeView>();
    }

    public void OnHit(HitData hitData)
    {
        if (hitData.Team != Team.Enemy)
            return;
        
        _currLife = Mathf.Max(0, _currLife - hitData.Damage);
        _treeView.SetHealthPercentage(_currLife / (float)_initialLife);

        if (_currLife <= 0)
        {
            if (!NetworkManager.Instance.Server.IsRunning)
                return;
            
            ServerMessages.SendGameOver(false);
        }
    }
}