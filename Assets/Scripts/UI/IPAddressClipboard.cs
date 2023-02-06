using DG.Tweening;
using UnityEngine;

public class IPAddressClipboard : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _targetText;
    [SerializeField] private TMPro.TextMeshProUGUI _copiedFeedbackText;

    private Tween _copiedFeedbackTween;
    
    public void CopyToClipboard()
    {
        UnityEngine.GUIUtility.systemCopyBuffer = _targetText.text;
        
        this._copiedFeedbackText.color = Color.white;
        this._copiedFeedbackText.DOColor(new Color(1f, 1f, 1f, 0f), 0.5f).SetDelay(1f);
    }
}
