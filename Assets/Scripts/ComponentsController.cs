using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ComponentsController : MonoBehaviour
{
    [SerializeField] private RotatableUIElement _rotatableUIElement;
    [SerializeField] private ComponentsList _componentsList;
    [SerializeField] private SystemCamera _systemCamera;
    public Action<ReducerMode> OnModeChanged;
    private Transform _selectedComponentTransform = null;
    private bool _isExploded;
    private Transform _activeComponentTransform;
    private Dictionary<Transform, TransformData> _initialTransforms = new Dictionary<Transform, TransformData>();

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
        _componentsList.OnComponentSelect += ShowComponent;
        _rotatableUIElement.RotatableObject = _componentsList.parentTransform;

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

        Vector3 centerPoint = Vector3.zero;
        for (int i = 0; i < sortedChildren.Count; i++)
        {
            centerPoint += sortedChildren[i].position;
        }
        centerPoint /= sortedChildren.Count;

        Sequence hideSequence = DOTween.Sequence();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            if (child != _selectedComponentTransform)
            {
                Vector3 targetConnectPosition = centerPoint;

                hideSequence.Append(child.DOMove(targetConnectPosition, 0.2f));

                hideSequence.Join(child.DOScale(Vector3.one * 0.2f, 0.2f));
            }
        }

        hideSequence.Append(_selectedComponentTransform.DOScale(Vector3.one, 0.2f))
            .OnComplete(() => StartCoroutine(OnHideComplete(sortedChildren)));
    }

    private IEnumerator OnHideComplete(List<Transform> sortedChildren)
    {
        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            if (child != _selectedComponentTransform)
            {
                child.DOScale(Vector3.zero, 0.2f);
                child.DOLocalRotate(Vector3.zero, 0.2f);
                float offsetY = _isExploded ? i * 0.2f : 0f;
                Vector3 targetPosition = child.localPosition - new Vector3(0f, offsetY, 0f);
                child.DOLocalMove(targetPosition, 0.2f);
            }
        }
        yield return new WaitForSeconds(0.2f);
        _componentsList.ShowListNames();
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

            child.DOLocalRotate(Vector3.zero, 0.5f);

            float offsetY = _isExploded ? i * 0.2f : 0f;
            Vector3 targetPosition = _initialTransforms[child].Position + new Vector3(0f, offsetY, 0f);

            child.gameObject.SetActive(true);

            child.DOLocalMove(targetPosition, 0.2f);
            child.DOScale(Vector3.one, 0.2f);
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
            OnModeChanged?.Invoke(ReducerMode.Recovery);
            Camera.main.transform.DOLookAt(_componentsList.parentTransform.position, 0.5f).OnComplete(() =>
            {
                CollapseDetail();
                _componentsList.ShowListNames();
                _systemCamera.Zoom(_systemCamera.ZoomToDefault);
                Camera.main.transform.DOMove(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -6f), 1f);
                _componentsList.mainTransform.DOLocalRotate(Vector3.zero, 1f);
                _componentsList.parentTransform.DOLocalRotate(Vector3.zero, 1f);
            });
        }
        else
        {
            OnModeChanged?.Invoke(ReducerMode.Exploded);
            Camera.main.transform.DOLookAt(_componentsList.mainTransform.position, 0.5f).OnComplete(() =>
            {
                ShowAll();
                Camera.main.transform.DOMove(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -6f), 1f);
                _componentsList.mainTransform.DOLocalRotate(Vector3.zero, 1f);
                _componentsList.parentTransform.DOLocalRotate(Vector3.zero, 1f);
            });
        }
    }

    private void CollapseDetail()
    {
        Vector3 centerPoint = (_componentsList.mainTransform.position + _componentsList.parentTransform.position) / 2f;

        float distance = Vector3.Distance(_componentsList.mainTransform.position, _componentsList.parentTransform.position);

        Vector3 newCameraPosition = centerPoint + new Vector3(0f, 0f, -distance);

        List<Transform> sortedChildren = SortChildrenByHierarchyOrder();

        for (int i = 0; i < sortedChildren.Count; i++)
        {
            Transform child = sortedChildren[i];

            float offsetY = _isExploded ? i * 0.2f : 0f;
            Vector3 targetPosition = _initialTransforms[child].Position + new Vector3(0f, offsetY, 0f);

            child.DOLocalMove(targetPosition, 0.1f);
            child.DOScale(1, 1f).OnComplete(() => child.gameObject.SetActive(true));
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
