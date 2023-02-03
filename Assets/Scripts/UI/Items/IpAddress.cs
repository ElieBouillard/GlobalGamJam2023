using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IpAddress : Singleton<IpAddress>
{
    [SerializeField] private TMP_InputField _ipAddressInput;
    [SerializeField] private TMP_Text _ipAddressText;

    public string GetIpAddress()
    {
        return _ipAddressInput.text;
    }
    
    public void SetIpAddress(string ipAddress)
    {
        _ipAddressText.text = ipAddress;
    }
}
