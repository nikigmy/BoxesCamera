using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ChestAnimation : MonoBehaviour
{
    [SerializeField] private GameObject chestLid;
    [SerializeField] private Vector3 chestLidTargetRotation;
    [SerializeField] private float chestLidRotationDuration;
    
    [SerializeField] private Material chestMaterial;
    [SerializeField] private float chestMaterialStartDissolve;
    [SerializeField] private float chestMaterialEndDissolve;
    [SerializeField] private float dissolveDuration;
    
    [SerializeField] private TransitionAnimation firstCameraAnimation;
    [SerializeField] private TransitionAnimation secondCameraAnimation;
    [SerializeField] private UnityEvent onComplete;
    
    private float currentDisolve;
    
    [Button]
    public void PlayAnimation()
    {
        currentDisolve = chestMaterialStartDissolve;
        var sequence = DOTween.Sequence();
        sequence.AppendCallback(firstCameraAnimation.PlayAnimation);
        sequence.Join(chestLid.transform.DOLocalRotate(chestLidTargetRotation, chestLidRotationDuration));
        sequence.AppendCallback(secondCameraAnimation.PlayAnimation);
        sequence.Join(DOTween
            .To(() => currentDisolve, (value) => currentDisolve = value, chestMaterialEndDissolve, dissolveDuration)
            .OnUpdate(() => chestMaterial.SetFloat("_DissolveAmount", currentDisolve)));
        sequence.onComplete += () =>
        {
            onComplete?.Invoke();
            chestMaterial.SetFloat("_DissolveAmount", chestMaterialStartDissolve);
            gameObject.SetActive(false);
        };
    }
}
