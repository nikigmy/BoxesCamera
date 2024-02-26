using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonCombinationPuzzle : Puzzle
{
    [SerializeField] private List<int> buttonSequence;
    [SerializeField] private List<Interactable> buttonInteractables;
    [SerializeField] private List<Renderer> buttonRenderers;
    [SerializeField] private Color rightButtonTint;
    [SerializeField] private Color wrongButtonTint;

    private List<int> currentSequence;
    private bool completed;
    private void Start()
    {
        currentSequence = new List<int>(buttonSequence.Count);
        SubscribeToIndexes();
    }

    private void SubscribeToIndexes()
    {
        for (var index = 0; index < buttonInteractables.Count; index++)
        {
            var buttonIndex = index;
            var button = buttonInteractables[buttonIndex];
            button.OnClick.AddListener(() => OnButtonPress(buttonIndex));
        }
    }

    public void OnButtonPress(int index)
    {
        if (completed) return;
        
        var targetIndex = buttonSequence[currentSequence.Count];
        if (index == targetIndex)
        {
            buttonRenderers[index].material.color = rightButtonTint;
            currentSequence.Add(index);
            if (currentSequence.Count == buttonSequence.Count)
            {
                completed = true;
                onCompleted?.Invoke();
                Invoke(nameof(ResetColors), 0.5f);
            }
        }
        else
        {
            buttonRenderers[index].material.color = wrongButtonTint;
            Invoke(nameof(ResetColors), 0.5f);
            currentSequence.Clear();
        }
    }

    public void ResetColors()
    {
        foreach (var buttonRenderer in buttonRenderers)
        {
            buttonRenderer.material.color = Color.white;
        }
    }
}
