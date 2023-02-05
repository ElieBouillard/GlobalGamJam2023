using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashScreenPanel : Singleton<DashScreenPanel>
{
    private Canvas _canvas;

    protected override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<Canvas>();
    }

    public void SetCamera(Camera camera)
    {
        _canvas.worldCamera = camera;
    }
}
