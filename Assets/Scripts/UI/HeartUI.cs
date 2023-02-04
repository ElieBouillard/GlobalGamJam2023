using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Image _image = null;
    [SerializeField] private Sprite _onSprite = null;
    [SerializeField] private Sprite _offSprite = null;

    private bool _state = true;

    public void Toggle(bool state, bool force = false)
    {
        if (!force && state == _state)
            return;

        _state = state;
        _image.sprite = _state ? _onSprite : _offSprite;
        // TODO: Anim.
    }
}
