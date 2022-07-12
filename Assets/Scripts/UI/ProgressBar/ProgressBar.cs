using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image filler;
    [HideInInspector] private float maxValue;

    [SerializeField] private float fillDUration;
    [SerializeField] private Ease fillEase;

    private Camera mainCamera;
    private Tween tween;

    private float _value;

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            tween?.Kill();
            tween = FillAnimation(_value).Play();
        }
    }

    public float MaxValue => maxValue;

    public void Initialize(float _maxValue)
    {
        maxValue = _maxValue;
        Value = _maxValue.ToPercent(_maxValue);
        mainCamera = Camera.main;
    }

    private Sequence FillAnimation(float value)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(filler.DOFillAmount(value, fillDUration).SetEase(fillEase));
        return sequence;
    }

    private void OnDestroy()
    {
        tween?.Kill();
    }

    private void Update()
    {
        transform.LookAt(mainCamera.transform.position);
    }
}
