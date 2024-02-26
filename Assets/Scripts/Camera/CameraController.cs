using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CameraPoint initialCameraPoint;
    [SerializeField] private Camera cam;
    [SerializeField] private float cameraRotationSensitivity = 0.085f;
    [SerializeField] private float cameraRotationSpeed = 0.015f;
    [SerializeField] private float baseTransitionTime = 1;
    
    public float BaseTransitionTime => baseTransitionTime;
    public bool IsInTransition => _isInTransition;

    public CameraPoint CurrentPoint => _currentPoint;

    private CameraPoint _currentPoint;
    private Vector3 _storedRotation;
    private bool _isInTransition = false;
    private Transform _trs;
    private Coroutine _transitionRoutine;

    private void Awake()
    {
        _trs = transform;
    }

    private void Start()
    {
        SetInitialPoint(initialCameraPoint);
    }

    
    private void LateUpdate()
    {
        ApplyCameraRotation();
    }

    public void SetInitialPoint(CameraPoint point)
    {
        _trs.position = point.Cam.transform.position;
        _trs.rotation = point.Cam.transform.rotation;
        _currentPoint = point;
        cam.fieldOfView = CurrentPoint.Cam.fieldOfView;
    }
    
    public Coroutine TransitionToPoint(CameraTransition transition, bool customRotation = false, Quaternion rotation = default)
    {
        if (_transitionRoutine != null)
        {
            StopCoroutine(_transitionRoutine);
        }
        _transitionRoutine = StartCoroutine(TransitionToPointRoutine(transition, customRotation, rotation));
        return _transitionRoutine;
    }
    
    public void GoBack()
    {
        if (CanGoBack())
        {
            TransitionToPoint(CurrentPoint.BackTransition, true, GetBackRotation());
        }
    }

    public void DoCameraDrag(Vector3 mouseDelta)
    {
        mouseDelta.x /= Screen.width;
        mouseDelta.y /= Screen.height;
        if(IsInTransition) return;
        
        var rotationVector = mouseDelta * cameraRotationSensitivity * CurrentPoint.RotationMultiplier;
        _storedRotation.y += rotationVector.x;
        _storedRotation.x -= rotationVector.y;
        var currentTargetRot = _trs.rotation.eulerAngles + _storedRotation;
        var referenceRotation = CurrentPoint.Cam.transform.rotation.eulerAngles;
        
        var clampedRotation = VectorHelpers.ClampRotation(currentTargetRot, referenceRotation, CurrentPoint.HorizontalDegreesOfFreedom, CurrentPoint.VerticalDegreesOfFreedom);
        var correction = currentTargetRot - clampedRotation;
        _storedRotation -= correction;
    }

    /// <summary>
    /// Update camera rotation smoothly from stored input
    /// </summary>
    private void ApplyCameraRotation()
    {
        if (IsInTransition) return;
        
        var rotation = Vector3.MoveTowards(Vector3.zero, _storedRotation, cameraRotationSpeed * _storedRotation.magnitude * Time.deltaTime);
        var eulerAngles = transform.rotation.eulerAngles + rotation;
        _trs.rotation = Quaternion.Euler(eulerAngles);
        _storedRotation -= rotation;
        
        _trs.position = CalculateTargetPositionFromRotation(CurrentPoint, _trs.rotation);
    }

    private IEnumerator TransitionToPointRoutine(CameraTransition transition, bool customRotation = false, Quaternion rotation = default)
    {
        _storedRotation = Vector3.zero;
        _isInTransition = true;
        _currentPoint = transition.Point;
        
        Quaternion targetRotation;
        Vector3 targetPosition;
        if (customRotation)
        {
            var res = VectorHelpers.ClampRotation(rotation.eulerAngles, CurrentPoint.Cam.transform.rotation.eulerAngles, 
                CurrentPoint.HorizontalDegreesOfFreedom, CurrentPoint.VerticalDegreesOfFreedom);
            targetRotation = Quaternion.Euler(res);
            
            targetPosition = CalculateTargetPositionFromRotation(CurrentPoint, targetRotation);
        }
        else
        {
            targetRotation = CurrentPoint.Cam.transform.rotation;
            targetPosition = CurrentPoint.Cam.transform.position;
        }
        
        var oldFov = cam.fieldOfView;
        var startPosition = _trs.position;
        var startRotation = _trs.rotation;
        float transitionProgress = 0;

        while (true)
        {
            transitionProgress = Mathf.Clamp01(transitionProgress + (1 / BaseTransitionTime) * transition.TransitionSpeed * Time.deltaTime);
            var transitionLocation = transition.TransitionCurve.Evaluate(transitionProgress);
            
            _trs.position = Vector3.Lerp(startPosition, targetPosition, transitionLocation);
            _trs.rotation = Quaternion.Lerp(startRotation, targetRotation, transitionLocation);
            cam.fieldOfView = Mathf.Lerp(oldFov, CurrentPoint.Cam.fieldOfView, transitionLocation);
            if (Math.Abs(transitionProgress - 1) < 0.000001f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        _isInTransition = false;
        _transitionRoutine = null;
        transition.OnDone?.Invoke();
    }

    /// <summary>
    /// Calculate target camera position for a given point and rotation
    /// </summary>
    /// <param name="cameraPoint"></param>
    /// <param name="transitionRotation"></param>
    /// <returns></returns>
    private Vector3 CalculateTargetPositionFromRotation(CameraPoint cameraPoint, Quaternion transitionRotation)
    {
        return cameraPoint.RotationPivot.position +
               transitionRotation * -(cameraPoint.CameraOffsetFromPivot * Vector3.forward);
    }

    /// <summary>
    /// Calculate rotation when backing camera out
    /// </summary>
    /// <returns></returns>
    private Quaternion GetBackRotation()
    {
        if (CurrentPoint.BackTransition.Point.RotateAroundSelf)
        {
            var newPivotPosition = CurrentPoint.BackTransition.Point.Cam.transform.position;
            var dir = CurrentPoint.RotationPivot.transform.position - newPivotPosition;

            return Quaternion.LookRotation(dir);
        }
        else
        {
            return _trs.rotation;
        }
    }
    
    
    private bool CanGoBack()
    {
        return CurrentPoint.BackTransition != null && CurrentPoint.BackTransition.Point != null && !CurrentPoint.BackTransition.Blocked;
    }

    public bool CanInteractWithPoint(CameraPoint cameraPoint, out CameraTransition transition)
    {
        transition = CurrentPoint.NextPoints.FirstOrDefault(x => x.Point == cameraPoint);
        return transition != null && !transition.Blocked;
    }
}