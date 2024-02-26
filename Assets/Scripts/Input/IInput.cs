using System;
using UnityEngine;

public interface IInput
{
    Action<int, RaycastHit[]> OnPointerDown { get; set; }
    Action<bool, int, RaycastHit[]> OnPointerUp { get; set; }
    Action OnBack { get; set; }
    Action<Vector3> OnDrag { get; set; }
    
    Vector3 GetMouseDelta();
    void ToggleInput(bool enabled);

}