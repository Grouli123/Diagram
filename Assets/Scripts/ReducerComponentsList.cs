using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
public class ReducerComponentsList : MonoBehaviour
{
    public Action<Transform> OnComponentSelect;

    public Transform mainTransform;
    public Transform parentTransform;
    [SerializeField] private Transform _componentsParent;
    [SerializeField] private ReducerComponentElement _componentElementPrefab;
    [SerializeField] private ReducerController _reducerController; 
    [SerializeField] private RotatableUIElement _rotatableUIElement;
    private List<ReducerComponentElement> _componentsList = new();

    private void Awake()
    {
        if (_componentsParent != null)
        {
            for (int i = 0; i < _componentsParent.childCount; i++)
            {
                ReducerComponentElement newElement = Instantiate(_componentElementPrefab, transform);
                newElement.SetComponent(_componentsParent.GetChild(i));
                newElement.OnComponentSelect += OnComponentSelectHandling;
                _componentsList.Add(newElement);
            }
        }

        _reducerController.OnModeChanged += OnReducerModeChanged;
    }

    private void OnComponentSelectHandling(Transform component)
    {
        _rotatableUIElement.RotatableObject = component;
        OnComponentSelect?.Invoke(component);
    }

    private void OnReducerModeChanged(ReducerMode mode)
    {
        switch (mode)
        {
            case ReducerMode.Exploded:
                ShowComponents(false);
                break;

            case ReducerMode.Recovery:
                ShowComponents(true);
                break;
        }
    }

    private void ShowComponents(bool show)
    {
        for (int i = 0; i < _componentsList.Count; i++)
            _componentsList[i].gameObject.SetActive(show);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _componentsList.Count; i++)
            _componentsList[i].OnComponentSelect -= OnComponentSelectHandling;

        _reducerController.OnModeChanged -= OnReducerModeChanged;
    }
}
