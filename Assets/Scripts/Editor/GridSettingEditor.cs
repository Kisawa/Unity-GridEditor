using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(GridController))]
public class GridSettingEditor : Editor
{
    GridController self;
    SerializedProperty GridMeshProp;
    SerializedProperty GridMatProp;
    SerializedProperty LinkLineWidthProp;
    SerializedProperty LinkLineBiasProp;
    SerializedProperty LinkMatProp;
    SerializedProperty StepProp;
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
        GUILayout.Space(10);

        bool isEmpty = true;
        for (int i = 0; i < GridsProp.arraySize; i++)
        {
            if (GridsProp.GetArrayElementAtIndex(i).objectReferenceValue != null)
                isEmpty = false;
        }

        if (isEmpty)
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
        if (GUILayout.Button("Refresh", GUILayout.Width(EditorGUIUtility.currentViewWidth - 30)))
            refresh();
        EditorGUILayout.LabelField("Data View:");
        EditorGUI.indentLevel++;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(GridsProp);
        EditorGUILayout.PropertyField(LinkDataProp);
        EditorGUI.EndDisabledGroup();
        EditorGUI.indentLevel--;
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
                    if (self.Grids.Any(x => x != null && x.Point == forwardSeek))
                    {
                        if (!self.LinkData.Contains(forwardLink))
                        {
                            GameObject _line = NewLinkObj(self, forwardLink);
                            forwardLink.Obj = _line;
                            self.LinkData.Add(forwardLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == backSeek))
                    {
                        if (!self.LinkData.Contains(backLink))
                        {
                            GameObject _line = NewLinkObj(self, backLink);
                            backLink.Obj = _line;
                            self.LinkData.Add(backLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == rightSeek))
                    {
                        if (!self.LinkData.Contains(rightLink))
                        {
                            GameObject _line = NewLinkObj(self, rightLink);
                            rightLink.Obj = _line;
                            self.LinkData.Add(rightLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == leftSeek))
                    {
                        if (!self.LinkData.Contains(leftLink))
                        {
                            GameObject _line = NewLinkObj(self, leftLink);
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
                    if (self.Grids.Any(x => x != null && x.Point == forwardRightSeek))
                    {
                        if (!self.LinkData.Contains(forwardRightLink))
                        {
                            GameObject _line = NewLinkObj(self, forwardRightLink);
                            forwardRightLink.Obj = _line;
                            self.LinkData.Add(forwardRightLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == forwardLeftSeek))
                    {
                        if (!self.LinkData.Contains(forwardLeftLink))
                        {
                            GameObject _line = NewLinkObj(self, forwardLeftLink);
                            forwardLeftLink.Obj = _line;
                            self.LinkData.Add(forwardLeftLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == backRightSeek))
                    {
                        if (!self.LinkData.Contains(backRightLink))
                        {
                            GameObject _line = NewLinkObj(self, backRightLink);
                            backRightLink.Obj = _line;
                            self.LinkData.Add(backRightLink);
                        }
                    }
                    if (self.Grids.Any(x => x != null && x.Point == backLeftSeek))
                    {
                        if (!self.LinkData.Contains(backLeftLink))
                        {
                            GameObject _line = NewLinkObj(self, backLeftLink);
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
        List<Vector2Int> gridPoints = new List<Vector2Int>();
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
                gridPoints.Add(_grid.Point);
                Transform _gridTrans = _grid.transform;
                MeshFilter _gridMeshFilter = _grid.GetComponent<MeshFilter>();
                MeshRenderer _gridMeshRenderer = _grid.GetComponent<MeshRenderer>();
                Undo.RecordObject(_gridTrans, "Refresh Grid");
                Undo.RecordObject(_gridMeshFilter, "Refresh Grid");
                Undo.RecordObject(_gridMeshRenderer, "Refresh Grid");
                _gridTrans.localPosition = new Vector3(self.Step.x * _grid.Point.x, 0, self.Step.y * _grid.Point.y);
                _gridMeshFilter.sharedMesh = self.GridMesh;
                _gridMeshRenderer.sharedMaterial = self.GridMat;
                EditorUtility.SetDirty(_gridTrans);
                EditorUtility.SetDirty(_gridMeshFilter);
                EditorUtility.SetDirty(_gridMeshRenderer);
            }
        }
        for (int i = 0; i < LinkDataProp.arraySize; i++)
        {
            SerializedProperty _linkDataProp = LinkDataProp.GetArrayElementAtIndex(i);
            Vector2Int point0 = _linkDataProp.FindPropertyRelative("Point0").vector2IntValue;
            Vector2Int point1 = _linkDataProp.FindPropertyRelative("Point1").vector2IntValue;
            GameObject lineObj = _linkDataProp.FindPropertyRelative("Obj").objectReferenceValue as GameObject;
            if (gridPoints.Any(x => x == point0) && gridPoints.Any(x => x == point1))
            {
                if (lineObj == null)
                {
                    lineObj = NewLinkObj(self, point0, point1);
                    _linkDataProp.FindPropertyRelative("Obj").objectReferenceValue = lineObj;
                }
                LineRenderer _lineRenderer = lineObj.GetComponent<LineRenderer>();
                Undo.RecordObject(_lineRenderer, "Refresh LinkLine");
                _lineRenderer.sharedMaterial = self.LinkMat;
                Vector3 pos0 = new Vector3(self.Step.x * point0.x, 0, self.Step.y * point0.y);
                Vector3 pos1 = new Vector3(self.Step.x * point1.x, 0, self.Step.y * point1.y);
                Vector3 dir = Vector3.Normalize(pos1 - pos0);
                pos0 += dir * self.LinkLineBias;
                pos1 -= dir * self.LinkLineBias;
                pos1 += Vector3.up * .00001f;
                _lineRenderer.SetPositions(new Vector3[] { pos0, pos1 });
                EditorUtility.SetDirty(_lineRenderer);
            }
            else
            {
                if (lineObj != null)
                    Undo.DestroyObjectImmediate(lineObj);
                LinkDataProp.DeleteArrayElementAtIndex(i);
                i--;
            }
        }
        for (int i = 0; i < self.transform.childCount; i++)
        {
            Transform _child = self.transform.GetChild(i);
            Grid _grid = _child.GetComponent<Grid>();
            if (_grid != null && _grid.Controller == self && !self.Grids.Contains(_grid))
                DestroyImmediate(_child.gameObject);
        }
    }

    public static Grid NewGridObj(GridController controller, Vector2Int point)
    {
        GameObject obj = new GameObject(point.ToString());
        obj.transform.SetParent(controller.transform);
        obj.transform.localPosition = new Vector3(controller.Step.x * point.x, 0, controller.Step.y * point.y);
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

    public static GameObject NewLinkObj(GridController controller, GridController.Link link)
    {
        return NewLinkObj(controller, link.Point0, link.Point1);
    }

    public static GameObject NewLinkObj(GridController controller, Vector2Int point0, Vector2Int point1)
    {
        GameObject obj = new GameObject($"link: {point0.ToString()}-{point1.ToString()}");
        obj.transform.SetParent(controller.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.sharedMaterial = controller.LinkMat;
        line.widthMultiplier = controller.LinkLineWidth;
        Vector3 pos0 = new Vector3(controller.Step.x * point0.x, 0, controller.Step.y * point0.y);
        Vector3 pos1 = new Vector3(controller.Step.x * point1.x, 0, controller.Step.y * point1.y);
        Vector3 dir = Vector3.Normalize(pos1 - pos0);
        pos0 += dir * controller.LinkLineBias;
        pos1 -= dir * controller.LinkLineBias;
        pos1 += Vector3.up * .00001f;
        line.SetPositions(new Vector3[] { pos0, pos1 });
        Undo.RegisterCreatedObjectUndo(obj, "Create Line");
        return obj;
    }
}