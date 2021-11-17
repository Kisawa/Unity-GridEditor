using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Mesh GridMesh;
    public Material GridMat;
    public float LinkLineWidth = .5f;
    public float LinkLineBias = .1f;
    public Material LinkMat;

    public Vector2 Step = Vector2.one;

    public List<Grid> Grids = new List<Grid>();
    public List<Link> LinkData = new List<Link>();
    public List<GameObject> LinkObj = new List<GameObject>();

    public List<Grid> FindPathTo(Grid start, Grid end)
    {
        if (!Grids.Contains(start) || !Grids.Contains(end))
            return null;
        HashSet<Vector2Int> discarded = new HashSet<Vector2Int>();
        List<GridInfo> cache = new List<GridInfo>();
        discarded.Add(start.Point);
        return FindPathTo(new GridInfo(start, end), end, cache, discarded);
    }

    List<Grid> FindPathTo(GridInfo start, Grid end, List<GridInfo> cache, HashSet<Vector2Int> discarded, GridInfo current = null)
    {
        if (current == null)
            current = start;
        PushNeighborhood(current, end, cache, discarded);
        if (cache.Count == 0)
            return null;
        current = CheckMinCostInfo(cache, out int index);
        cache.RemoveAt(index);
        discarded.Add(current.Self.Point);
        if (current.Self == end)
        {
            List<Grid> route = new List<Grid>();
            while (current != null)
            {
                if (current.Self != start.Self)
                    route.Insert(0, current.Self);
                current = current.Prev;
            }
            return route;
        }
        return FindPathTo(start, end, cache, discarded, current);
    }

    GridInfo CheckMinCostInfo(List<GridInfo> infos, out int index)
    {
        GridInfo info = null;
        index = -1;
        for (int i = 0; i < infos.Count; i++)
        {
            GridInfo _info = infos[i];
            if (info == null || info.Cost > _info.Cost)
            {
                info = _info;
                index = i;
            }
        }
        return info;
    }

    void PushNeighborhood(GridInfo current, Grid end, List<GridInfo> cache, HashSet<Vector2Int> discarded)
    {
        Vector2Int forwardSeek = current.Self.Point - Vector2Int.down;
        PushGridInfo(forwardSeek, current, end, cache, discarded);
        Vector2Int backSeek = current.Self.Point + Vector2Int.down;
        PushGridInfo(backSeek, current, end, cache, discarded);
        Vector2Int rightSeek = current.Self.Point - Vector2Int.left;
        PushGridInfo(rightSeek, current, end, cache, discarded);
        Vector2Int leftSeek = current.Self.Point + Vector2Int.left;
        PushGridInfo(leftSeek, current, end, cache, discarded);

        Vector2Int forwardRightSeek = current.Self.Point - Vector2Int.down - Vector2Int.left;
        PushGridInfo(forwardRightSeek, current, end, cache, discarded);
        Vector2Int forwardLeftSeek = current.Self.Point - Vector2Int.down + Vector2Int.left;
        PushGridInfo(forwardLeftSeek, current, end, cache, discarded);
        Vector2Int backRightSeek = current.Self.Point + Vector2Int.down - Vector2Int.left;
        PushGridInfo(backRightSeek, current, end, cache, discarded);
        Vector2Int backLeftSeek = current.Self.Point + Vector2Int.down + Vector2Int.left;
        PushGridInfo(backLeftSeek, current, end, cache, discarded);
    }

    void PushGridInfo(Vector2Int point, GridInfo prev, Grid end, List<GridInfo> cache, HashSet<Vector2Int> discarded)
    {
        if (discarded.Contains(point) || !LinkData.Contains(new Link(point, prev.Self.Point)))
            return;
        Grid grid = Grids.FirstOrDefault(x => x != null && x.Point == point);
        if (grid == null)
            return;
        GridInfo info = new GridInfo(grid, prev, end);
        int _index = cache.FindIndex(x => x.Self.Point == point);
        if (_index < 0)
            cache.Add(info);
        else if (info.Cost <= cache[_index].Cost)
            cache[_index] = info;
    }

    static float CalcCost(Vector2Int start, Vector2Int end)
    {
        Vector2Int DIF = end - start;
        int abs_x = Mathf.Abs(DIF.x);
        int abs_y = Mathf.Abs(DIF.y);
        return abs_x < abs_y ? abs_x * 14 + (abs_y - abs_x) * 10 : abs_y * 14 + (abs_x - abs_y) * 10;
    }

    public class GridInfo
    {
        public Grid Self;
        public GridInfo Prev;
        public float G_Cost;
        public float H_Cost;
        public float Cost => G_Cost + H_Cost;

        public GridInfo(Grid self, GridInfo prev, Grid end)
        {
            Self = self;
            Prev = prev;
            G_Cost = CalcCost(prev.Self.Point, self.Point) + prev.G_Cost;
            H_Cost = CalcCost(self.Point, end.Point);
        }

        public GridInfo(Grid self, Grid end)
        {
            Self = self;
            Prev = null;
            H_Cost = CalcCost(self.Point, end.Point);
        }
    }

    [Serializable]
    public struct Link : IEquatable<Link>
    {
        public Vector2Int Point0;
        public Vector2Int Point1;
        public GameObject Obj;

        public Link(Vector2Int point0, Vector2Int point1)
        {
            Point0 = point0;
            Point1 = point1;
            Obj = null;
        }

        public static bool operator ==(Link l0, Link l1)
        {
            return (l0.Point0 == l1.Point0 && l0.Point1 == l1.Point1) || (l0.Point0 == l1.Point1 && l0.Point1 == l1.Point0);
        }

        public static bool operator !=(Link l0, Link l1)
        {
            return !(l0 == l1);
        }

        public override bool Equals(object obj)
        {
            return this == (Link)obj;
        }

        public override int GetHashCode()
        {
            return Point0.GetHashCode() + Point1.GetHashCode();
        }

        public bool Equals(Link other)
        {
            return this == other;
        }
    }
}