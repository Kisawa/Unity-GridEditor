using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridEditor;
using Grid = GridEditor.Grid;

public class testEnemy : MonoBehaviour
{
    public static List<Grid> EnemyStandGrids = new List<Grid>();

    public Grid StandGrid;

    private void Start()
    {
        EnemyStandGrids.Add(StandGrid);
    }
}