using UnityEngine.Events;

public interface IInteractable
{
    public void OnMousePress();
    public void OnMouseRelease();
    public bool CanInteract();
    public void SetBlocked(bool value);
    public UnityEvent OnClick { get; }
    public UnityEvent OnRelease { get; }
}