using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridEditor
{
    public class GridController : MonoBehaviour
    {
        public Mesh GridMesh;
        public Material GridMat;
        public float LinkLineWidth = .5f;
        public float LinkLineBias = .1f;
        public Material LinkMat;

        public Vector2 Step = Vector2.one;
        public Vector2 Offset;
        public Vector2 StaggeredOffset;

        public List<Grid> Grids = new List<Grid>();
        public List<Link> LinkData = new List<Link>();

        float diagonalStraightRatio = 1.4f;

        public void SetDiagonalStraightRatio(float ratio = 1.4f)
        {
            diagonalStraightRatio = ratio;
        }

        /// <param name="extraPushRule">Used as an extra to compare if the grid is available</param>
        /// <param name="sameCostCompare">priority mode, compare grid info of the same cost. (prev, now)</param>
        public List<Grid> FindPathTo(Grid start, Grid end, Func<GridInfo, bool> extraPushRule = null, Func<GridInfo, GridInfo, GridInfo> sameCostCompare = null)
        {
            if (!Grids.Contains(start) || !Grids.Contains(end))
                return null;
            HashSet<Vector2Int> discarded = new HashSet<Vector2Int>();
            List<GridInfo> cache = new List<GridInfo>();
            discarded.Add(start.Point);
            return FindPathTo(new GridInfo(start, end), end, extraPushRule, sameCostCompare, cache, discarded);
        }

        List<Grid> FindPathTo(GridInfo start, Grid end, Func<GridInfo, bool> extraPushRule, Func<GridInfo, GridInfo, GridInfo> sameCostCompare, List<GridInfo> cache, HashSet<Vector2Int> discarded, GridInfo current = null)
        {
            if (current == null)
                current = start;
            PushNeighborhood(current, end, extraPushRule, sameCostCompare, cache, discarded);
            if (cache.Count == 0)
                return null;
            current = CheckMinCostInfo(end, cache, sameCostCompare, out int index);
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
            return FindPathTo(start, end, extraPushRule, sameCostCompare, cache, discarded, current);
        }

        GridInfo CheckMinCostInfo(Grid end, List<GridInfo> infos, Func<GridInfo, GridInfo, GridInfo> sameCostCompare, out int index)
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
                else if (info.Cost == _info.Cost)
                {
                    if (_info.Self == end || (sameCostCompare != null && sameCostCompare(info, _info) == _info))
                    {
                        info = _info;
                        index = i;
                    }
                }
            }
            return info;
        }

        void PushNeighborhood(GridInfo current, Grid end, Func<GridInfo, bool> extraPushRule, Func<GridInfo, GridInfo, GridInfo> sameCostCompare, List<GridInfo> cache, HashSet<Vector2Int> discarded)
        {
            Vector2Int forwardSeek = current.Self.Point - Vector2Int.down;
            PushGridInfo(forwardSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int backSeek = current.Self.Point + Vector2Int.down;
            PushGridInfo(backSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int rightSeek = current.Self.Point - Vector2Int.left;
            PushGridInfo(rightSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int leftSeek = current.Self.Point + Vector2Int.left;
            PushGridInfo(leftSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);

            Vector2Int forwardRightSeek = current.Self.Point - Vector2Int.down - Vector2Int.left;
            PushGridInfo(forwardRightSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int forwardLeftSeek = current.Self.Point - Vector2Int.down + Vector2Int.left;
            PushGridInfo(forwardLeftSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int backRightSeek = current.Self.Point + Vector2Int.down - Vector2Int.left;
            PushGridInfo(backRightSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
            Vector2Int backLeftSeek = current.Self.Point + Vector2Int.down + Vector2Int.left;
            PushGridInfo(backLeftSeek, current, end, extraPushRule, sameCostCompare, cache, discarded);
        }

        void PushGridInfo(Vector2Int point, GridInfo prev, Grid end, Func<GridInfo, bool> extraPushRule, Func<GridInfo, GridInfo, GridInfo> sameCostCompare, List<GridInfo> cache, HashSet<Vector2Int> discarded)
        {
            if (discarded.Contains(point) || !LinkData.Contains(new Link(point, prev.Self.Point)))
                return;
            Grid grid = Grids.FirstOrDefault(x => x != null && x.Point == point);
            if (grid == null)
                return;
            GridInfo info = new GridInfo(grid, prev, end);
            if (extraPushRule != null && !extraPushRule(info))
                return;
            int _index = cache.FindIndex(x => x.Self.Point == point);
            
            if (_index < 0)
                cache.Add(info);
            else
            {
                GridInfo _info = cache[_index];
                if (info.Cost < _info.Cost)
                    cache[_index] = info;
                else if (info.Cost == _info.Cost && (sameCostCompare == null || sameCostCompare(_info.Prev, prev) == prev))
                    cache[_index] = info;
            }
        }

        static float CalcCost(Vector2Int start, Vector2Int end, float diagonalStraightRatio)
        {
            Vector2Int DIF = end - start;
            int abs_x = Mathf.Abs(DIF.x);
            int abs_y = Mathf.Abs(DIF.y);
            return abs_x < abs_y ? abs_x * 10 * diagonalStraightRatio + (abs_y - abs_x) * 10 : abs_y * 10 * diagonalStraightRatio + (abs_x - abs_y) * 10;
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
                G_Cost = CalcCost(prev.Self.Point, self.Point, self.Controller.diagonalStraightRatio) + prev.G_Cost;
                H_Cost = CalcCost(self.Point, end.Point, self.Controller.diagonalStraightRatio);
            }

            public GridInfo(Grid self, Grid end)
            {
                Self = self;
                Prev = null;
                H_Cost = CalcCost(self.Point, end.Point, self.Controller.diagonalStraightRatio);
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
}