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
                Renderer renderer = _grid.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", Color.red);
                renderer.material.renderQueue = 3001;
                LinkRenderer linkRenderer = Start.Controller.LinkData[Start.Controller.LinkData.FindIndex(x => x == new GridController.Link(prev.Point, _grid.Point))].Obj.GetComponent<LinkRenderer>();
                linkRenderer.material.SetColor("_Color", Color.red);
                linkRenderer.material.renderQueue = 3001;
                prev = _grid;
            }
        }
        if (GUILayout.Button("Find Path Near Enemy"))
        {
            if (Start == null || End == null)
                return;
            clear();
            Start.Controller.SetDiagonalStraightRatio(1);
            List<Grid> route = Start.Controller.FindPathTo(Start, End, null, (x, y) =>
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
                Renderer renderer = _grid.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", Color.red);
                renderer.material.renderQueue = 3001;
                LinkRenderer linkRenderer = Start.Controller.LinkData[Start.Controller.LinkData.FindIndex(x => x == new GridController.Link(prev.Point, _grid.Point))].Obj.GetComponent<LinkRenderer>();
                linkRenderer.material.SetColor("_Color", Color.red);
                linkRenderer.material.renderQueue = 3001;
                prev = _grid;
            }
        }
    }

    void clear()
    {
        if (Start == null)
            return;
        for (int i = 0; i < Start.Controller.Grids.Count; i++)
        {
            Renderer renderer = Start.Controller.Grids[i].GetComponent<Renderer>();
            renderer.material.SetColor("_Color", Color.black);
            renderer.material.renderQueue = 3000;
        }
        for (int i = 0; i < Start.Controller.LinkData.Count; i++)
        {
            LinkRenderer renderer = Start.Controller.LinkData[i].Obj.GetComponent<LinkRenderer>();
            renderer.material.SetColor("_Color", Color.black);
            renderer.material.renderQueue = 3000;
        }
    }
}