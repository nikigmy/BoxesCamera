using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Data class holding data about a camera point/hotspot. Attributes from Odin inspector added for easy editing
/// </summary>
public class CameraPoint : MonoBehaviour
{
    [BoxGroup("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private bool rotateAroundSelf;
    [BoxGroup("Camera")] [DisableIf("rotateAroundSelf")]
    [SerializeField] private Transform rotationPivot;
    [BoxGroup("Camera")][Range(0, 360)]
    [SerializeField] private  float horizontalDegreesOfFreedom = 45;
    [BoxGroup("Camera")][Range(0, 360)]
    [SerializeField] private  float verticalDegreesOfFreedom = 45;
    [BoxGroup("Camera")][Range(-1, 1)]
    [SerializeField] private  float rotationMultiplier = 1;
    
    [InlineEditor] 
    [BoxGroup("Transitions")] [LabelText("@\"Back Transition : \" + backTransition?.TransitionName")]
    [SerializeField] private  CameraTransition backTransition;
    [InlineEditor]
    [BoxGroup("Transitions")] [ListDrawerSettings(HideAddButton = true, ListElementLabelName = "TransitionName", ShowFoldout = false)]
    [SerializeField] private  List<CameraTransition> nextPoints;

    public Camera Cam => cam;

    public bool RotateAroundSelf => rotateAroundSelf;

    public float HorizontalDegreesOfFreedom => horizontalDegreesOfFreedom;

    public float VerticalDegreesOfFreedom => verticalDegreesOfFreedom;

    public float RotationMultiplier => rotationMultiplier;

    public CameraTransition BackTransition => backTransition;

    public List<CameraTransition> NextPoints => nextPoints;

    public float CameraOffsetFromPivot => _cameraOffset;

    public Transform RotationPivot => rotateAroundSelf ? cam.transform : rotationPivot;
    
    private float _cameraOffset;

    private void Awake()
    {
        _cameraOffset = (cam.transform.position - RotationPivot.position).magnitude;
    }

#if UNITY_EDITOR
    
    public float projectionMultiplier = 2;
    /// <summary>
    /// Draw camera constraints gizmos and forward ray
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        var pivotPosition = rotateAroundSelf ? cam.transform.position : rotationPivot.position;
        var rotLeft = Quaternion.AngleAxis(horizontalDegreesOfFreedom / 2,-cam.transform.up);
        var rotRight = Quaternion.AngleAxis(horizontalDegreesOfFreedom / 2,cam.transform.up);
        var rotUp = Quaternion.AngleAxis(verticalDegreesOfFreedom / 2,-cam.transform.right);
        var rotDown = Quaternion.AngleAxis(verticalDegreesOfFreedom / 2,cam.transform.right);

        var topRight = rotRight * rotUp * -cam.transform.forward;
        var topLeft = rotLeft * rotUp * -cam.transform.forward;
        var botRight = rotRight * rotDown * -cam.transform.forward;
        var botLeft = rotLeft * rotDown * -cam.transform.forward;

        topRight *= projectionMultiplier;
        topLeft *= projectionMultiplier;
        botRight *= projectionMultiplier;
        botLeft *= projectionMultiplier;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pivotPosition, topRight);
        Gizmos.DrawRay(pivotPosition, topLeft);
        Gizmos.DrawRay(pivotPosition, botLeft);
        Gizmos.DrawRay(pivotPosition,  botRight);
        
        Gizmos.DrawLine(pivotPosition + botRight,pivotPosition + botLeft);
        Gizmos.DrawLine(pivotPosition + topRight,pivotPosition + topLeft);
        Gizmos.DrawLine(pivotPosition + topRight,pivotPosition + botRight);
        Gizmos.DrawLine(pivotPosition + botLeft,pivotPosition + topLeft);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(cam.transform.position, cam.transform.forward * projectionMultiplier);
    }
#endif
}