using UnityEngine;
using UnityEngine.Events;

public class Puzzle : MonoBehaviour
{
    [SerializeField] protected UnityEvent onCompleted;
    public UnityEvent OnCompleted => onCompleted;
}