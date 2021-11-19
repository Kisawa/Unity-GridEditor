using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GridEditor;
using Grid = GridEditor.Grid;

public class test : MonoBehaviour
{
    public Grid Start;
    public Grid End;

    private void OnGUI()
    {
        if (GUILayout.Button("Find Path"))
        {
            if (Start == null || End == null)
                return;
            clear();
            Start.Controller.SetDiagonalStraightRatio(1);
            List<Grid> route = Start.Controller.FindPathTo(Start, End);
            if (route == null)
                return;
            Grid prev = Start;
            Start.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            for (int i = 0; i < route.Count; i++)
            {
                Grid _grid = route[i];
                _grid.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                Start.Controller.LinkData[Start.Controller.LinkData.FindIndex(x => x == new GridController.Link(prev.Point, _grid.Point))].Obj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                prev = _grid;
            }
        }
        if (GUILayout.Button("Find Path Near Enemy"))
        {
            if (Start == null || End == null)
                return;
            clear();
            Start.Controller.SetDiagonalStraightRatio(1);
            List<Grid> route = Start.Controller.FindPathTo(Start, End, (x, y) =>
            {
                if (testEnemy.EnemyStandGrids.Contains(x.Self))
                    return x;
                if (testEnemy.EnemyStandGrids.Contains(y.Self))
                    return y;
                return x;
            });
            if (route == null)
                return;
            Grid prev = Start;
            Start.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            for (int i = 0; i < route.Count; i++)
            {
                Grid _grid = route[i];
                _grid.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                Start.Controller.LinkData[Start.Controller.LinkData.FindIndex(x => x == new GridController.Link(prev.Point, _grid.Point))].Obj.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                prev = _grid;
            }
        }
    }

    void clear()
    {
        if (Start == null)
            return;
        for (int i = 0; i < Start.Controller.Grids.Count; i++)
            Start.Controller.Grids[i].GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        for (int i = 0; i < Start.Controller.LinkData.Count; i++)
            Start.Controller.LinkData[i].Obj.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
    }
}