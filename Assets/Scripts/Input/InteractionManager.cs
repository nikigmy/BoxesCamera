using System.Linq;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayerMask;
    
    private IInput _input;
    private CameraController _cameraController;
    private IInteractable _currentInteractable;
    
    void Start()
    {
        _input = GameManager.Instance.ServiceProvider.GetService<IInput>();
        _cameraController = GameManager.Instance.ServiceProvider.GetService<CameraController>();
        _input.OnDrag += OnCameraDrag;
        _input.OnPointerUp += OnClickConfirm;
        _input.OnPointerDown += OnClick;
        _input.OnBack += OnBack;
    }

    private void OnClick(int hitCount, RaycastHit[] rayHits)
    {
        for (int i = 0; i < hitCount; i++)
        {
            if (!interactableLayerMask.Contains(rayHits[i].collider.gameObject.layer))//blocked by un-interactable layer
            {
                return;
            }
            var interactable = rayHits[i].transform.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                _currentInteractable = interactable;
                interactable.OnMousePress();
                return;
            }
        }
    }

    private void OnClickConfirm(bool isConfirm, int hitCount, RaycastHit[] rayHits)
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.OnMouseRelease();
            _currentInteractable = null;
            return;
        }
        if(_cameraController.IsInTransition || !isConfirm) return;

        for (int i = 0; i < hitCount; i++)
        {
            if (!interactableLayerMask.Contains(rayHits[i].collider.gameObject.layer))//blocked by un-interactable layer
            {
                return;
            }
            var cameraPoint = rayHits[i].transform.GetComponent<CameraPointTrigger>()?.CameraPoint;
            if (cameraPoint != null && _cameraController.CanInteractWithPoint(cameraPoint, out var transition))
            {
                _cameraController.TransitionToPoint(transition);
                break;
            }
        }
    }

    private void OnBack()
    {
        _cameraController.GoBack();
    }

    private void OnCameraDrag(Vector3 mouseDelta)
    {
        if (_currentInteractable == null)
        {
            _cameraController.DoCameraDrag(mouseDelta);
        }
    }
}
