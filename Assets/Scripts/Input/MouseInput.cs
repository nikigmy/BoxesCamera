using System;
using System.Collections;
using Sirenix.Utilities;
using UnityEngine;

public class MouseInput : MonoBehaviour, IInput
{
    [SerializeField] private float maxMouseClickDelta = 5;
    [SerializeField] private LayerMask mouseClickLayerMask;
    
    public Action<int, RaycastHit[]> OnPointerDown { get; set; }
    public Action<bool, int, RaycastHit[]> OnPointerUp { get; set; }
    public Action OnBack { get; set; }
    public Action<Vector3> OnDrag { get; set; }
    
    private Vector3 _clickMousePosition;
    private Vector3 _lastMousePosition;
    private Vector3 _mouseDelta;
    private bool _inputEnabled = true;
    private RaycastHit[] _raycasts;
    
    private void Start()
    {
        _raycasts = new RaycastHit[10];
    }

    private void Update()
    {
        if (!_inputEnabled) return;
        
        HandleInput();
    }

    public Vector3 GetMouseDelta()
    {
        return _mouseDelta;
    }

    public void ToggleInput(bool enabled)
    {
        _inputEnabled = enabled;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnBack?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _clickMousePosition = Input.mousePosition;
            _lastMousePosition = _clickMousePosition;
            
            var hits = DoMouseRaycast(_raycasts);
            if (hits > 0)
            {
                OnPointerDown?.Invoke(hits, _raycasts);
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var totalMouseDelta = _clickMousePosition - Input.mousePosition;
            if (totalMouseDelta.magnitude <= maxMouseClickDelta)
            {
                var hits = DoMouseRaycast(_raycasts);
                if (hits > 0)
                {
                    OnPointerUp?.Invoke(true, hits, _raycasts);
                }
            }
            else
            {
                OnPointerUp?.Invoke(false, 0, _raycasts);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _mouseDelta = _lastMousePosition - Input.mousePosition;
            _lastMousePosition = Input.mousePosition;
            OnDrag?.Invoke(_mouseDelta);
        }
        else
        {
            _mouseDelta = Vector3.zero;
            _lastMousePosition = Vector3.zero;
        }
    }

    private int DoMouseRaycast(RaycastHit[] raycasts)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var rayHits = Physics.RaycastNonAlloc(ray, raycasts, 25, mouseClickLayerMask);
        Array.Sort(raycasts, 0, rayHits, new RaycastDistanceComparer());
        return rayHits;
    }
    
}