using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Data class holding data about a camera transition to a camera hotspot/point
/// </summary>
public class CameraTransition : MonoBehaviour
{
    [SerializeField] private CameraPoint point;
    [SerializeField] private float transitionSpeed = 1;
    [SerializeField] private AnimationCurve transitionCurve;
    [SerializeField] private bool blocked;
    [SerializeField] private UnityEvent onDone;

    public CameraPoint Point => point;

    public float TransitionSpeed => transitionSpeed;

    public AnimationCurve TransitionCurve => transitionCurve;

    public bool Blocked => blocked;

    public UnityEvent OnDone => onDone;
    
    public string TransitionName => point != null ? point.gameObject.name : "";//for odin inspector

    public void SetBlocked(bool value)
    {
        blocked = value;
    }
}