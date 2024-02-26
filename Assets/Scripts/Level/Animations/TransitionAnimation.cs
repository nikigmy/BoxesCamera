using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class TransitionAnimation : MonoBehaviour
{
    [SerializeField] private bool preserveLastRotation;

    // [InlineEditor()]
    [SerializeField] private List<CameraTransition> transitions;

    private CameraController _cameraController;
    private void Start()
    {
        _cameraController = GameManager.Instance.ServiceProvider.GetService<CameraController>();
    }

    [Button]
    public void PlayAnimation()
    {
        StartCoroutine(DoTransitionAnimation());
    }
    
    private IEnumerator DoTransitionAnimation()
    {
        for (var index = 0; index < transitions.Count; index++)
        {
            var transition = transitions[index];
            if (index >= transitions.Count - 1 && preserveLastRotation)
            {
                _cameraController.TransitionToPoint(transition, true, _cameraController.transform.rotation);
            }
            else
            {
                _cameraController.TransitionToPoint(transition);
            }

            yield return new WaitForSeconds(_cameraController.BaseTransitionTime * (1 / transition.TransitionSpeed));
        }
    }
}