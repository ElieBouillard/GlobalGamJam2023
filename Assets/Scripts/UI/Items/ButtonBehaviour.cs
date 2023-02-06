using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private TMPro.TextMeshProUGUI _text = null;

    private Color _textInitColor;

    private void Awake()
    {
        this._textInitColor = this._text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.05f);
        this._text.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.08f);
        this._text.color = _textInitColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }
}