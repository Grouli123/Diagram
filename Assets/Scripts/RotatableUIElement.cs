using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class RotatableUIElement : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform _rotatableObject;
    [SerializeField] private float _rotationSpeed = 1f;    
    public Transform RotatableObject {get=>  _rotatableObject; set => _rotatableObject = value;}
    private Vector2 _mouseOrigin;

    public void SetRotatableObjectParent(Transform parent)
    {
        _rotatableObject = parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _mouseOrigin = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_rotatableObject == null)
            return;

        var delta = eventData.position - _mouseOrigin;
        _mouseOrigin = eventData.position;

        var axis = Quaternion.AngleAxis(-90f, Vector3.forward) * delta;
        _rotatableObject.rotation = Quaternion.AngleAxis(delta.magnitude * _rotationSpeed, axis) * _rotatableObject.rotation;
    }
}