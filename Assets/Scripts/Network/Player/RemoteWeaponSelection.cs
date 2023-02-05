using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemoteWeaponSelection : MonoBehaviour
{
    [SerializeField] private GameObject[] _weapons;

    public void SetWeapon(int id)
    {
        if (id == 0)
        {
            _weapons[0].SetActive(false);
            _weapons[1].SetActive(false);
        }
        else if(id == 1)
        {
            _weapons[0].SetActive(true);
            _weapons[1].SetActive(false);
        }
        else if(id == 2)
        {
            _weapons[0].SetActive(false);
            _weapons[1].SetActive(true);
        }
    }
}
