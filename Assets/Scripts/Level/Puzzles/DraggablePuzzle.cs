using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class DraggablePuzzle : Puzzle
{
    [SerializeField] private List<DragInteraction> interactableObjects;
    [SerializeField] private float progressSensitivity = 3;
    [SerializeField] private Vector3 interactionAxis = Vector3.up;
    [SerializeField] private float progressReturnRate = 2;
    [SerializeField] private Interactable interactable;
    
    private bool interacting;
    private IInput _input;
    private CameraController _cameraController;
    private float progress;
    private bool completed;
    

    protected void Start()
    {
        _input = GameManager.Instance.ServiceProvider.GetService<IInput>();
        interactable.OnClick.AddListener(() => interacting = true);
        interactable.OnRelease.AddListener(() => interacting = false);
    }

    void Update()
    {
        if (completed)
            return;
        
        if (interacting)
        {
            var mouseDelta = _input.GetMouseDelta();
            mouseDelta.x /= Screen.width;
            mouseDelta.y /= Screen.height;
            
            var progressDelta = new Vector3(mouseDelta.x * interactionAxis.x, mouseDelta.y * interactionAxis.y,
                mouseDelta.z * interactionAxis.z);
            var signedDelta = progressDelta.x + progressDelta.y + progressDelta.z;
            progress = Mathf.Clamp01(progress + signedDelta * progressSensitivity * Time.deltaTime);
            ApplyInteractionProgress(progress);
            if (Math.Abs(progress - 1) < 0.00001f)
            {
                completed = true;
                onCompleted?.Invoke();
            }
        }

        if (!interacting)
        {
            progress = Mathf.MoveTowards(progress, 0, progressReturnRate * Time.deltaTime);
            ApplyInteractionProgress(progress);
        }
    }

    [Button]
    private void ApplyInteractionProgress(float progress)
    {
        foreach (var interactableObject in interactableObjects)
        {
            if (interactableObject.usePosition)
            {
                interactableObject.obj.transform.localPosition = Vector3.Lerp(interactableObject.minValue,
                    interactableObject.maxValue, progress);
            }
            else if (interactableObject.useRotation)
            {
                interactableObject.obj.transform.localRotation = Quaternion.Euler(Vector3.Lerp(
                    interactableObject.minValue,
                    interactableObject.maxValue, progress));
            }
        }
    }

    [Serializable]
    public class DragInteraction
    {
        public Transform obj;
        public bool useRotation;
        public bool usePosition;
        public Vector3 minValue;
        public Vector3 maxValue;
    }
}
