using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class ReductionSystemCamera : MonoBehaviour
{
    [SerializeField] private float _zoomDuration;
    [SerializeField] private float _zoomAmount;
    [SerializeField] private float _zoomDefault;
    [SerializeField] private ReducerComponentsList _componentsList;
    private Transform _currentComponent;
    private bool _isZoomed;

    private void Awake()
    {
        _componentsList.OnComponentSelect += HandleComponentSelection;
    }

    private void HandleComponentSelection(Transform component)
    {
        if (!_isZoomed)
        {
            Zoom(_zoomAmount);
            _isZoomed = true;
        }
        else if (_isZoomed && component == _currentComponent)
        {            
            Zoom(_zoomDefault);
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