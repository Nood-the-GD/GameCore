using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffectCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector3 _scaleEffect = new Vector3(-0.1f, -0.1f, -0.1f);
    private Vector3 _originalScale;

    void Awake()
    {
        _originalScale = this.transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.transform.DOScale(this.transform.localScale + _scaleEffect, 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.transform.DOScale(_originalScale, 0.2f);
    }
}
