using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class SystemCamera : MonoBehaviour
{
    [SerializeField] private float _zoomDuration;
    [SerializeField] private float _zoomAmount;
    [SerializeField] private float zoomDefault;
    public float ZoomToDefault { get => zoomDefault; set => zoomDefault = value; }

    [SerializeField] private ComponentsList _componentsList;
    private Transform _currentComponent;
    private bool _isZoomed;

    private void Awake() => 
        _componentsList.OnComponentSelect += HandleComponentSelection;

    private void HandleComponentSelection(Transform component)
    {
        if (!_isZoomed)
        {
            Zoom(_zoomAmount);
            _isZoomed = true;
        }
        else if (_isZoomed && component == _currentComponent)
        {            
            Zoom(zoomDefault);
            _isZoomed = false;
        }

        _currentComponent = component;
    }

    public void Zoom(float amount)
    {
        transform.DOLocalMoveZ(amount, _zoomDuration);
    }

    private void OnDestroy()
    {
        _componentsList.OnComponentSelect -= HandleComponentSelection;
    }    
}

