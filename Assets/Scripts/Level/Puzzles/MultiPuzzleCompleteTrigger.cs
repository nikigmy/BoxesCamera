using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiPuzzleCompleteTrigger : Puzzle
{
    [SerializeField] protected List<Puzzle> puzzles;

    private int puzzlesComplete;

    private void Start()
    {
        foreach (var puzzle in puzzles)
        {
            puzzle.OnCompleted.AddListener(() => OnPuzzleComplete());
        }
    }

    private void OnPuzzleComplete()
    {
        puzzlesComplete++;
        if (puzzlesComplete == puzzles.Count)
        {
            onCompleted?.Invoke();
        }
    }
}
