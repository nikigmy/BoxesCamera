using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected CameraPoint targetCameraPoint;
    [SerializeField] protected bool blocked;
    [SerializeField] protected UnityEvent onClick;
    [SerializeField] protected UnityEvent onRelease;
    public UnityEvent OnClick => onClick;
    public UnityEvent OnRelease => onRelease;
    
    protected CameraController _cameraController;
    
    protected virtual void Start()
    {
        _cameraController = GameManager.Instance.ServiceProvider.GetService<CameraController>();
    }

    public virtual bool CanInteract()
    {
        return !blocked && _cameraController.CurrentPoint == targetCameraPoint;
    }
    
    public virtual void SetBlocked(bool value)
    {
        blocked = value;
    }

    public virtual void OnMousePress()
    {
        if (!blocked)
        {
            onClick?.Invoke();
        }
    }

    public virtual void OnMouseRelease()
    {
        if (!blocked)
        {
            onRelease?.Invoke();
        }
    }
}