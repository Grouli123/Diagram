using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReducerController : MonoBehaviour
{
    [SerializeField] private RotatableUIElement _rotatableUIElement;
    [SerializeField] private ReducerComponentsList _componentsList;
    public Action<ReducerMode> OnModeChanged;
    private Transform _selectedComponentTransform = null;
    private bool _isExploded;
    private Transform _activeComponentTransform;
    private Dictionary<Transform, TransformData> _initialTransforms = new Dictionary<Transform, TransformData>();

    private void Awake()
    {
        _componentsList.OnComponentSelect += ShowComponent;
        SaveInitialTransforms();
    }
    private void Update()
    {
        if (_rotatableUIElement != null && _rotatableUIElement.RotatableObject != null)
        {
            Camera.main.transform.DOLookAt(_rotatableUIElement.RotatableObject.position, 0.5f);
        }
    }
    private void SaveInitialTransforms()
    {
        _initialTransforms.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            _initialTransforms[child] = new TransformData(child);
        }
    }

    private void ShowComponent(Transform component)
    {
        if (_selectedComponentTransform != component)
        {
            if (_activeComponentTransform != null)
            {
                _activeComponentTransform.gameObject.SetActive(false);
            }

            _selectedComponentTransform = component;
            HideAllExceptSelected();
        }
        else
        {
            _selectedComponentTransform = null;
            ShowAll();
        }
    }

    private void HideAllExceptSelected()
    {
        List<Transform> sortedChildren = SortChildrenByHierarchyOrder();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            if (child != _selectedComponentTransform)
            {
                child.DOLocalRotate(Vector3.zero, 1f);

                float offsetY = _isExploded ? i * 0.5f : 0f;
                Vector3 targetPosition = child.localPosition - new Vector3(0f, offsetY, 0f);

                // Добавлено: анимация уменьшения скейла перед отключением объекта
                child.DOScale(Vector3.zero, 1f).OnComplete(() => child.gameObject.SetActive(false));

                child.DOLocalMove(targetPosition, 1f);
            }
        }

        _selectedComponentTransform.DOScale(Vector3.one, 1f);
        _selectedComponentTransform.gameObject.SetActive(true);
        _activeComponentTransform = _selectedComponentTransform;

        if (_isExploded)
        {
            _rotatableUIElement.RotatableObject = _componentsList.mainTransform;
        }
    }

    private List<Transform> SortChildrenByHierarchyOrder()
    {
        List<Transform> sortedChildren = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            sortedChildren.Add(transform.GetChild(i));
        }

        sortedChildren.Sort((t1, t2) => t1.GetSiblingIndex().CompareTo(t2.GetSiblingIndex()));

        return sortedChildren;
    }

    private void ShowAll()
    {
        List<Transform> sortedChildren = SortChildrenByHierarchyOrder();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            child.DOLocalRotate(Vector3.zero, 1f);

            float offsetY = _isExploded ? i * 0.5f : 0f;
            Vector3 targetPosition = _initialTransforms[child].Position + new Vector3(0f, offsetY, 0f);

            child.gameObject.SetActive(true);

            child.DOLocalMove(targetPosition, 1f);
            child.DOScale(Vector3.one, 1f);
        }

        if (_isExploded)
        {
            _rotatableUIElement.RotatableObject = _componentsList.mainTransform;
        }
        else
        {
            _rotatableUIElement.RotatableObject = _componentsList.parentTransform;
        }
    }

    public void ExplodeOrRecovery()
    {
        _isExploded = !_isExploded;

        if (_isExploded)
        {
            OnModeChanged?.Invoke(ReducerMode.Exploded);
            Camera.main.transform.DOLookAt(_componentsList.mainTransform.position, 1f).OnComplete(() =>
            {
                ShowAll();

                Camera.main.DOFieldOfView(80f, 1f);
            });
        }
        else
        {
            OnModeChanged?.Invoke(ReducerMode.Recovery);
            Camera.main.transform.DOLookAt(_componentsList.parentTransform.position, 1f).OnComplete(() =>
            {
                CollapseDetail();
                Camera.main.DOFieldOfView(60f, 1f);
            });
        }
    }

    private void CollapseDetail()
    {
        Vector3 centerPoint = (_componentsList.mainTransform.position + _componentsList.parentTransform.position) / 2f;

        float distance = Vector3.Distance(_componentsList.mainTransform.position, _componentsList.parentTransform.position);

        Vector3 newCameraPosition = centerPoint + new Vector3(0f, 0f, -distance);

        Camera.main.transform.DOMove(newCameraPosition, 1f);

        List<Transform> sortedChildren = SortChildrenByHierarchyOrder();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            float offsetY = _isExploded ? i * 0.5f : 0f;
            Vector3 targetPosition = _initialTransforms[child].Position + new Vector3(0f, offsetY, 0f);

            child.DOLocalMove(targetPosition, 1f);
            child.DOScale(1, 1f).OnComplete(() => child.gameObject.SetActive(true));
        }

        if (_isExploded)
        {
            _rotatableUIElement.RotatableObject = _componentsList.mainTransform;

            Camera.main.transform.DOMove(_componentsList.mainTransform.position + new Vector3(0f, 0f, -distance), 1f);
        }
        else
        {
            _rotatableUIElement.RotatableObject = _componentsList.parentTransform;

            Camera.main.transform.DOMove(_componentsList.parentTransform.position + new Vector3(0f, 0f, -distance), 1f);
        }
    }

    private void OnDestroy()
    {
        _componentsList.OnComponentSelect -= ShowComponent;
    }

    private class TransformData
    {
        public Vector3 Position { get; private set; }

        public TransformData(Transform transform)
        {
            Position = transform.localPosition;
        }
    }
}

public enum ReducerMode
{
    Exploded,
    Recovery,
    Default
}
