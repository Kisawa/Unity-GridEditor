using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridEditor
{
    public class Grid : MonoBehaviour
    {
        public GridController Controller;
        public Vector2Int Point;

        public bool IsLinked(Grid grid)
        {
            if (Controller == null || grid.Controller != Controller)
                return false;
            return Controller.LinkData.Contains(new GridController.Link(Point, grid.Point));
        }

        public bool IsLinked(Grid grid, out GridController.Link link)
        {
            link = default;
            if (Controller == null || grid.Controller != Controller)
                return false;
            int index = grid.Controller.LinkData.IndexOf(new GridController.Link(Point, grid.Point));
            if (index > -1)
            {
                link = grid.Controller.LinkData[index];
                return true;
            }
            return false;
        }

        public Grid GetForward(bool linked = false)
        {
            Vector2Int forwardSeek = Point - Vector2Int.down;
            return GetGrid(forwardSeek, linked);
        }

        public Grid GetBack(bool linked = false)
        {
            Vector2Int backSeek = Point + Vector2Int.down;
            return GetGrid(backSeek, linked);
        }

        public Grid GetRight(bool linked = false)
        {
            Vector2Int rightSeek = Point - Vector2Int.left;
            return GetGrid(rightSeek, linked);
        }

        public Grid GetLeft(bool linked = false)
        {
            Vector2Int leftSeek = Point + Vector2Int.left;
            return GetGrid(leftSeek, linked);
        }

        public Grid GetForwardRight(bool linked = false)
        {
            Vector2Int forwardRightSeek = Point - Vector2Int.down - Vector2Int.left;
            return GetGrid(forwardRightSeek, linked);
        }

        public Grid GetForwardLeft(bool linked = false)
        {
            Vector2Int forwardLeftSeek = Point - Vector2Int.down + Vector2Int.left;
            return GetGrid(forwardLeftSeek, linked);
        }

        public Grid GetBackRight(bool linked = false)
        {
            Vector2Int backRightSeek = Point + Vector2Int.down - Vector2Int.left;
            return GetGrid(backRightSeek, linked);
        }

        public Grid GetBackLeft(bool linked = false)
        {
            Vector2Int backLeftSeek = Point + Vector2Int.down + Vector2Int.left;
            return GetGrid(backLeftSeek, linked);
        }

        Grid GetGrid(Vector2Int point, bool linked)
        {
            if (Controller == null)
                return null;
            return Controller.Grids.FirstOrDefault(x => x != null && x.Point == point && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, point))));
        }

        public Grid GetForwardWithLink(out GridController.Link link)
        {
            Vector2Int forwardSeek = Point - Vector2Int.down;
            return GetGridWithLink(forwardSeek, out link);
        }

        public Grid GetBackWithLink(out GridController.Link link)
        {
            Vector2Int backSeek = Point + Vector2Int.down;
            return GetGridWithLink(backSeek, out link);
        }

        public Grid GetRightWithLink(out GridController.Link link)
        {
            Vector2Int rightSeek = Point - Vector2Int.left;
            return GetGridWithLink(rightSeek, out link);
        }

        public Grid GetLeftWithLink(out GridController.Link link)
        {
            Vector2Int leftSeek = Point + Vector2Int.left;
            return GetGridWithLink(leftSeek, out link);
        }

        public Grid GetForwardRightWithLink(out GridController.Link link)
        {
            Vector2Int forwardRightSeek = Point - Vector2Int.down - Vector2Int.left;
            return GetGridWithLink(forwardRightSeek, out link);
        }

        public Grid GetForwardLeftWithLink(out GridController.Link link)
        {
            Vector2Int forwardLeftSeek = Point - Vector2Int.down + Vector2Int.left;
            return GetGridWithLink(forwardLeftSeek, out link);
        }

        public Grid GetBackRightWithLink(out GridController.Link link)
        {
            Vector2Int backRightSeek = Point + Vector2Int.down - Vector2Int.left;
            return GetGridWithLink(backRightSeek, out link);
        }

        public Grid GetBackLeftWithLink(out GridController.Link link)
        {
            Vector2Int backRightSeek = Point + Vector2Int.down + Vector2Int.left;
            return GetGridWithLink(backRightSeek, out link);
        }

        Grid GetGridWithLink(Vector2Int point, out GridController.Link link)
        {
            link = default;
            if (Controller == null)
                return null;
            Grid _grid = Controller.Grids.FirstOrDefault(x => x != null && x.Point == point);
            if (_grid != null)
            {
                int index = Controller.LinkData.IndexOf(new GridController.Link(Point, point));
                if (index > -1)
                    link = Controller.LinkData[index];
            }
            return _grid;
        }

        public List<Grid> GetNeighborhood(bool linked = false)
        {
            if (Controller == null)
                return null;
            List<Grid> neighborhood = new List<Grid>();
            GetNeighborhood(neighborhood, linked);
            return neighborhood;
        }

        public void GetNeighborhood(List<Grid> list, bool linked = false)
        {
            if (Controller == null)
                return;
            if (list == null)
                list = new List<Grid>();
            Vector2Int forwardSeek = Point - Vector2Int.down;
            Vector2Int backSeek = Point + Vector2Int.down;
            Vector2Int rightSeek = Point - Vector2Int.left;
            Vector2Int leftSeek = Point + Vector2Int.left;
            Vector2Int forwardRightSeek = Point - Vector2Int.down - Vector2Int.left;
            Vector2Int forwardLeftSeek = Point - Vector2Int.down + Vector2Int.left;
            Vector2Int backRightSeek = Point + Vector2Int.down - Vector2Int.left;
            Vector2Int backLeftSeek = Point + Vector2Int.down + Vector2Int.left;
            for (int i = 0; i < Controller.Grids.Count; i++)
            {
                Grid _grid = Controller.Grids[i];
                if (_grid == null)
                    continue;
                if (_grid.Point == forwardSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, forwardSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == backSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, backSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == rightSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, rightSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == leftSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, leftSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == forwardRightSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, forwardRightSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == forwardLeftSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, forwardLeftSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == backRightSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, backRightSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
                if (_grid.Point == backLeftSeek && (!linked || Controller.LinkData.Contains(new GridController.Link(Point, backLeftSeek))) && !list.Contains(_grid))
                    list.Add(_grid);
            }
        }

        public List<(Grid, GridController.Link)> GetNeighborhoodWithLink()
        {
            if (Controller == null)
                return null;
            List<(Grid, GridController.Link)> neighborhood = new List<(Grid, GridController.Link)>();
            GetNeighborhoodWithLink(neighborhood);
            return neighborhood;
        }

        public void GetNeighborhoodWithLink(List<(Grid, GridController.Link)> list)
        {
            if (Controller == null)
                return;
            if (list == null)
                list = new List<(Grid, GridController.Link)>();
            Vector2Int forwardSeek = Point - Vector2Int.down;
            Vector2Int backSeek = Point + Vector2Int.down;
            Vector2Int rightSeek = Point - Vector2Int.left;
            Vector2Int leftSeek = Point + Vector2Int.left;
            Vector2Int forwardRightSeek = Point - Vector2Int.down - Vector2Int.left;
            Vector2Int forwardLeftSeek = Point - Vector2Int.down + Vector2Int.left;
            Vector2Int backRightSeek = Point + Vector2Int.down - Vector2Int.left;
            Vector2Int backLeftSeek = Point + Vector2Int.down + Vector2Int.left;
            for (int i = 0; i < Controller.Grids.Count; i++)
            {
                Grid _grid = Controller.Grids[i];
                if (_grid == null)
                    continue;
                if (_grid.Point == forwardSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, forwardSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == backSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, backSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == rightSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, rightSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == leftSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, leftSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == forwardRightSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, forwardRightSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == forwardLeftSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, forwardLeftSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == backRightSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, backRightSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
                if (_grid.Point == backLeftSeek)
                {
                    int index = Controller.LinkData.IndexOf(new GridController.Link(Point, backLeftSeek));
                    if (index > -1)
                    {
                        GridController.Link link = Controller.LinkData[index];
                        (Grid, GridController.Link) _gridWithLink = (_grid, link);
                        if (!list.Contains(_gridWithLink))
                            list.Add(_gridWithLink);
                    }
                }
            }
        }
    }
}