using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public PanelType PanelType;

    protected virtual void Awake()
    {
        AssignButtonsReference();
    }

    protected virtual void AssignButtonsReference()
    {
        
    }
}