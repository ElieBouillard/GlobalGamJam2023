using UnityEngine;

public class DebugHittable : MonoBehaviour, IHittable
{
    public void OnHit(HitData hitData)
    {
        gameObject.SetActive(false);
    }
}
