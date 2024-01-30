using UnityEngine;
using TMPro;
using System;

public class ReducerComponentElement : MonoBehaviour
{
    public Action<Transform> OnComponentSelect;

    [SerializeField] private TMP_Text _text;
    [SerializeField] private Transform _component;

    public void SetComponent(Transform component)
    {
        _component = component;
        _text.text = _component.name;
    }

    public void OnClick()
    {
        OnComponentSelect?.Invoke(_component);
    }
}
