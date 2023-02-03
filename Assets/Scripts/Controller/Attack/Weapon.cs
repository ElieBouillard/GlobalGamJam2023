using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _radius = 1f;
    [SerializeField, Min(0f)] private float _distance = 1f;
    [SerializeField, Min(0f)] private float _damage = 10f;
    [SerializeField, Min(0f)] private float _cooldown = 0.33f;
}
