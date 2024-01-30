using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ComponentElement : MonoBehaviour
{
    public Action<Transform> OnComponentSelect;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _button;
    private Transform _component;

    public Transform Component
    {
        get => _component;
        set => _component = value;
    }

    private void Start() =>
        _button.onClick.AddListener(SelectComponent);

    public void SetComponent(Transform component)
    {
        if (_component)
            return;
        _component = component;
        _text.text = _component.name;
    }

    private void SelectComponent() =>
            OnComponentSelect?.Invoke(_component);
}
