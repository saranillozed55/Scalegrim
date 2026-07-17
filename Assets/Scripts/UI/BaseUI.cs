using UnityEngine;
using UnityEngine.UIElements;

public class BaseUI : MonoBehaviour, IUIToolkit
{

    protected UIDocument _document;
    protected VisualElement Container;

    protected virtual void Awake()
    {
        _document = GetComponent<UIDocument>();
        Container = _document.rootVisualElement.Q<VisualElement>("Container");
    }

    protected virtual void Start()
    {
        Container.AddToClassList("hidden");
    }
    protected virtual void PushToStack()
    {
        UIManager.Instance.Push(this);
    }

    public virtual void OnOpen()
    {
        Container.RemoveFromClassList("hidden");
    }
    public virtual void OnClose()
    {
        Container.AddToClassList("hidden");
    }
    public virtual void OnFocus()
    {
        //implement
    }
    public virtual void OnLoseFocus()
    {
        //implement
    }

}
