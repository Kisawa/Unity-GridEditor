using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GridEditor
{
    [CustomEditor(typeof(GridController))]
    public class GridControllerEditor : Editor
    {
        GridController self;
        SerializedProperty GridMeshProp;
        SerializedProperty GridMatProp;
        SerializedProperty LinkLineWidthProp;
        SerializedProperty LinkLineBiasProp;
        SerializedProperty LinkMatProp;
        SerializedProperty StepProp;
        SerializedProperty OffsetProp;
        SerializedProperty StaggeredOffsetProp;
        SerializedProperty GridsProp;
        SerializedProperty LinkDataProp;

        [SerializeField] Vector2Int matrix = Vector2Int.one;
        [SerializeField] bool linkStraight = true;
        [SerializeField] bool linkDiagonal = false;

        private void OnEnable()
        {
            self = serializedObject.targetObject as GridController;
            GridMeshProp = serializedObject.FindProperty("GridMesh");
            GridMatProp = serializedObject.FindProperty("GridMat");
            LinkLineWidthProp = serializedObject.FindProperty("LinkLineWidth");
            LinkLineBiasProp = serializedObject.FindProperty("LinkLineBias");
            LinkMatProp = serializedObject.FindProperty("LinkMat");
            StepProp = serializedObject.FindProperty("Step");
            OffsetProp = serializedObject.FindProperty("Offset");
            StaggeredOffsetProp = serializedObject.FindProperty("StaggeredOffset");
            GridsProp = serializedObject.FindProperty("Grids");
            LinkDataProp = serializedObject.FindProperty("LinkData");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Grid:");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(GridMeshProp);
            EditorGUILayout.PropertyField(GridMatProp);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Link Line:");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(LinkLineWidthProp);
            EditorGUILayout.PropertyField(LinkLineBiasProp);
            EditorGUILayout.PropertyField(LinkMatProp);
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(StepProp);
            EditorGUILayout.PropertyField(OffsetProp);
            EditorGUILayout.PropertyField(StaggeredOffsetProp);
            GUILayout.Space(10);

            if (GridsProp.arraySize == 0)
            {
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Initialize:");
                EditorGUI.indentLevel++;
                Vector2Int _matrix = EditorGUILayout.Vector2IntField("Matrix", matrix);
                _matrix.x = _matrix.x <= 1 ? 1 : _matrix.x;
                _matrix.y = _matrix.y <= 1 ? 1 : _matrix.y;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Link Straight", GUILayout.Width(100));
                bool _linkStraight = EditorGUILayout.Toggle(linkStraight, GUILayout.Width(50));
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Link Diagonal", GUILayout.Width(100));
                bool _linkDiagonal = EditorGUILayout.Toggle(linkDiagonal, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Create", GUILayout.Width(EditorGUIUtility.currentViewWidth - 30)))
                    initialize();
                EditorGUI.indentLevel--;
                if (GUI.changed)
                {
                    Undo.RecordObject(this, "Initialize Config Changed");
                    matrix = _matrix;
                    linkStraight = _linkStraight;
                    linkDiagonal = _linkDiagonal;
                }
            }
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(EditorGUIUtility.currentViewWidth * .65f)))
                refresh();
            if (GUILayout.Button("Refresh Link", GUILayout.Width(EditorGUIUtility.currentViewWidth * .34f - 29)))
                refreshLink();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Data View:");
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(GridsProp);
            EditorGUILayout.PropertyField(LinkDataProp);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
            if (GUILayout.Button("Clear", GUILayout.Width(EditorGUIUtility.currentViewWidth - 30)))
                clear();
            serializedObject.ApplyModifiedProperties();
        }

        void initialize()
        {
            refresh();
            serializedObject.Update();
            for (int i = 0; i < matrix.x; i++)
            {
                for (int j = 0; j < matrix.y; j++)
                {
                    int hor = i - matrix.x / 2;
                    int ver = j - matrix.y / 2;
                    Vector2Int point = new Vector2Int(hor, ver);
                    NewGridObj(self, point);
                }
            }
            if (linkStraight || linkDiagonal)
            {
                Undo.RecordObject(self, "Initialize Grids");
                for (int i = 0; i < self.Grids.Count; i++)
                {
                    Grid _grid = self.Grids[i];
                    if (_grid == null)
                        continue;
                    Vector3 _gridPos = _grid.transform.localPosition;
                    if (linkStraight)
                    {
                        Vector2Int forwardSeek = _grid.Point - Vector2Int.down;
                        Vector2Int backSeek = _grid.Point + Vector2Int.down;
                        Vector2Int rightSeek = _grid.Point - Vector2Int.left;
                        Vector2Int leftSeek = _grid.Point + Vector2Int.left;
                        GridController.Link forwardLink = new GridController.Link(_grid.Point, forwardSeek);
                        GridController.Link backLink = new GridController.Link(_grid.Point, backSeek);
                        GridController.Link rightLink = new GridController.Link(_grid.Point, rightSeek);
                        GridController.Link leftLink = new GridController.Link(_grid.Point, leftSeek);
                        Grid _forward = self.Grids.FirstOrDefault(x => x != null && x.Point == forwardSeek);
                        if (_forward != null)
                        {
                            if (!self.LinkData.Contains(forwardLink))
                            {
                                GameObject _line = NewLinkObj(self, forwardLink, _gridPos, _forward.transform.localPosition);
                                forwardLink.Obj = _line;
                                self.LinkData.Add(forwardLink);
                            }
                        }
                        Grid _back = self.Grids.FirstOrDefault(x => x != null && x.Point == backSeek);
                        if (_back != null)
                        {
                            if (!self.LinkData.Contains(backLink))
                            {
                                GameObject _line = NewLinkObj(self, backLink, _gridPos, _back.transform.localPosition);
                                backLink.Obj = _line;
                                self.LinkData.Add(backLink);
                            }
                        }
                        Grid _right = self.Grids.FirstOrDefault(x => x != null && x.Point == rightSeek);
                        if (_right != null)
                        {
                            if (!self.LinkData.Contains(rightLink))
                            {
                                GameObject _line = NewLinkObj(self, rightLink, _gridPos, _right.transform.localPosition);
                                rightLink.Obj = _line;
                                self.LinkData.Add(rightLink);
                            }
                        }
                        Grid _left = self.Grids.FirstOrDefault(x => x != null && x.Point == leftSeek);
                        if (_left != null)
                        {
                            if (!self.LinkData.Contains(leftLink))
                            {
                                GameObject _line = NewLinkObj(self, leftLink, _gridPos, _left.transform.localPosition);
                                leftLink.Obj = _line;
                                self.LinkData.Add(leftLink);
                            }
                        }
                    }
                    if (linkDiagonal)
                    {
                        Vector2Int forwardRightSeek = _grid.Point - Vector2Int.down - Vector2Int.left;
                        Vector2Int forwardLeftSeek = _grid.Point - Vector2Int.down + Vector2Int.left;
                        Vector2Int backRightSeek = _grid.Point + Vector2Int.down - Vector2Int.left;
                        Vector2Int backLeftSeek = _grid.Point + Vector2Int.down + Vector2Int.left;
                        GridController.Link forwardRightLink = new GridController.Link(_grid.Point, forwardRightSeek);
                        GridController.Link forwardLeftLink = new GridController.Link(_grid.Point, forwardLeftSeek);
                        GridController.Link backRightLink = new GridController.Link(_grid.Point, backRightSeek);
                        GridController.Link backLeftLink = new GridController.Link(_grid.Point, backLeftSeek);
                        Grid _forwardRight = self.Grids.FirstOrDefault(x => x != null && x.Point == forwardRightSeek);
                        if (_forwardRight != null)
                        {
                            if (!self.LinkData.Contains(forwardRightLink))
                            {
                                GameObject _line = NewLinkObj(self, forwardRightLink, _gridPos, _forwardRight.transform.localPosition);
                                forwardRightLink.Obj = _line;
                                self.LinkData.Add(forwardRightLink);
                            }
                        }
                        Grid _forwardLeft = self.Grids.FirstOrDefault(x => x != null && x.Point == forwardLeftSeek);
                        if (_forwardLeft != null)
                        {
                            if (!self.LinkData.Contains(forwardLeftLink))
                            {
                                GameObject _line = NewLinkObj(self, forwardLeftLink, _gridPos, _forwardLeft.transform.localPosition);
                                forwardLeftLink.Obj = _line;
                                self.LinkData.Add(forwardLeftLink);
                            }
                        }
                        Grid _backRight = self.Grids.FirstOrDefault(x => x != null && x.Point == backRightSeek);
                        if (_backRight != null)
                        {
                            if (!self.LinkData.Contains(backRightLink))
                            {
                                GameObject _line = NewLinkObj(self, backRightLink, _gridPos, _backRight.transform.localPosition);
                                backRightLink.Obj = _line;
                                self.LinkData.Add(backRightLink);
                            }
                        }
                        Grid _backLeft = self.Grids.FirstOrDefault(x => x != null && x.Point == backLeftSeek);
                        if (_backLeft != null)
                        {
                            if (!self.LinkData.Contains(backLeftLink))
                            {
                                GameObject _line = NewLinkObj(self, backLeftLink, _gridPos, _backLeft.transform.localPosition);
                                backLeftLink.Obj = _line;
                                self.LinkData.Add(backLeftLink);
                            }
                        }
                    }
                }
                EditorUtility.SetDirty(self);
            }
        }

        void refresh()
        {
            List<Grid> grids = new List<Grid>();
            for (int i = 0; i < GridsProp.arraySize; i++)
            {
                SerializedProperty _gridProp = GridsProp.GetArrayElementAtIndex(i);
                if (_gridProp.objectReferenceValue == null)
                {
                    GridsProp.DeleteArrayElementAtIndex(i);
                    i--;
                }
                else
                {
                    Grid _grid = _gridProp.objectReferenceValue as Grid;
                    grids.Add(_grid);
                    Transform _gridTrans = _grid.transform;
                    MeshFilter _gridMeshFilter = _grid.GetComponent<MeshFilter>();
                    MeshRenderer _gridMeshRenderer = _grid.GetComponent<MeshRenderer>();
                    Undo.RecordObject(_gridTrans, "Refresh Grid");
                    Undo.RecordObject(_gridMeshFilter, "Refresh Grid");
                    Undo.RecordObject(_gridMeshRenderer, "Refresh Grid");
                    _gridTrans.localPosition = CalcPos(self, _grid.Point);
                    _gridMeshFilter.sharedMesh = self.GridMesh;
                    _gridMeshRenderer.sharedMaterial = self.GridMat;
                    EditorUtility.SetDirty(_gridTrans);
                    EditorUtility.SetDirty(_gridMeshFilter);
                    EditorUtility.SetDirty(_gridMeshRenderer);
                }
            }
            refreshLink(grids);
            for (int i = 0; i < self.transform.childCount; i++)
            {
                Transform _child = self.transform.GetChild(i);
                Grid _grid = _child.GetComponent<Grid>();
                if (_grid != null && _grid.Controller == self && !self.Grids.Contains(_grid))
                    DestroyImmediate(_child.gameObject);
            }
        }

        void refreshLink()
        {
            List<Grid> grids = new List<Grid>();
            for (int i = 0; i < GridsProp.arraySize; i++)
            {
                SerializedProperty _gridProp = GridsProp.GetArrayElementAtIndex(i);
                if (_gridProp.objectReferenceValue != null)
                {
                    Grid _grid = _gridProp.objectReferenceValue as Grid;
                    grids.Add(_grid);
                }
            }
            refreshLink(grids);
        }

        void refreshLink(List<Grid> grids)
        {
            for (int i = 0; i < LinkDataProp.arraySize; i++)
            {
                SerializedProperty _linkDataProp = LinkDataProp.GetArrayElementAtIndex(i);
                Vector2Int point0 = _linkDataProp.FindPropertyRelative("Point0").vector2IntValue;
                Vector2Int point1 = _linkDataProp.FindPropertyRelative("Point1").vector2IntValue;
                GameObject lineObj = _linkDataProp.FindPropertyRelative("Obj").objectReferenceValue as GameObject;
                Grid grid0 = grids.FirstOrDefault(x => x.Point == point0);
                Grid grid1 = grids.FirstOrDefault(x => x.Point == point1);
                if (grid0 != null && grid1 != null)
                {
                    if (lineObj == null)
                    {
                        lineObj = NewLinkObj(self, point0, point1, grid0.transform.localPosition, grid1.transform.localPosition);
                        _linkDataProp.FindPropertyRelative("Obj").objectReferenceValue = lineObj;
                    }
                    else
                    {
                        LinkRenderer _linkRenderer = lineObj.GetComponent<LinkRenderer>();
                        Undo.RecordObject(_linkRenderer, "Refresh LinkLine");
                        _linkRenderer.SetGlobalMaterial(self.LinkMat);
                        Vector3 pos0 = grid0.transform.localPosition;
                        Vector3 pos1 = grid1.transform.localPosition;
                        Vector3 dir = Vector3.Normalize(pos1 - pos0);
                        pos0 += dir * self.LinkLineBias;
                        pos1 -= dir * self.LinkLineBias;
                        _linkRenderer.SetPosition(pos0, pos1);
                        EditorUtility.SetDirty(_linkRenderer);
                    }
                }
                else
                {
                    if (lineObj != null)
                        Undo.DestroyObjectImmediate(lineObj);
                    LinkDataProp.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }
        }

        void clear()
        {
            for (int i = 0; i < self.Grids.Count; i++)
            {
                Grid _grid = self.Grids[i];
                Undo.RecordObject(self, "Clear Grids");
                self.Grids.RemoveAt(i);
                i--;
                if (_grid != null)
                    Undo.DestroyObjectImmediate(_grid.gameObject);
            }
            for (int i = 0; i < self.LinkData.Count; i++)
            {
                GridController.Link _link = self.LinkData[i];
                if (_link.Obj != null)
                    Undo.DestroyObjectImmediate(_link.Obj);
            }
            Undo.RecordObject(self, "Clear LinkData");
            self.LinkData.Clear();
            EditorUtility.SetDirty(self);
        }

        public static Vector3 CalcPos(GridController controller, Vector2Int point)
        {
            return new Vector3(controller.Step.x * point.x, 0, controller.Step.y * point.y) + new Vector3(controller.Offset.x * point.y, 0, controller.Offset.y * point.x) + new Vector3(controller.StaggeredOffset.x * (point.y % 2), 0, controller.StaggeredOffset.y * (point.x % 2));
        }

        public static Grid NewGridObj(GridController controller, Vector2Int point)
        {
            GameObject obj = new GameObject(point.ToString());
            obj.transform.SetParent(controller.transform);
            obj.transform.localPosition = CalcPos(controller, point);
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.AddComponent<MeshFilter>().sharedMesh = controller.GridMesh;
            obj.AddComponent<MeshRenderer>().sharedMaterial = controller.GridMat;
            Grid grid = obj.AddComponent<Grid>();
            grid.Controller = controller;
            grid.Point = point;
            Undo.RegisterCreatedObjectUndo(obj, "Create Grid");
            Undo.RecordObject(controller, "Create Grid");
            controller.Grids.Add(grid);
            EditorUtility.SetDirty(controller);
            return grid;
        }

        public static GameObject NewLinkObj(GridController controller, GridController.Link link, Vector3 pos0, Vector3 pos1)
        {
            return NewLinkObj(controller, link.Point0, link.Point1, pos0, pos1);
        }

        public static GameObject NewLinkObj(GridController controller, Vector2Int point0, Vector2Int point1, Vector3 pos0, Vector3 pos1)
        {
            GameObject obj = new GameObject($"link: {point0.ToString()}-{point1.ToString()}");
            obj.transform.SetParent(controller.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localEulerAngles = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            LinkRenderer link = obj.AddComponent<LinkRenderer>();
            link.SetGlobalMaterial(controller.LinkMat);
            link.Width = controller.LinkLineWidth;
            Vector3 dir = Vector3.Normalize(pos1 - pos0);
            pos0 += dir * controller.LinkLineBias;
            pos1 -= dir * controller.LinkLineBias;
            link.SetPosition(pos0, pos1);
            Undo.RegisterCreatedObjectUndo(obj, "Create Line");
            return obj;
        }
    }
}