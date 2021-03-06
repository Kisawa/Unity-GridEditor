using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GridEditor
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : Editor
    {
        Grid self;
        SerializedObject GridControllerObject;
        SerializedProperty GridsProp;

        private void OnEnable()
        {
            self = serializedObject.targetObject as Grid;
            SerializedProperty ControllerProp = serializedObject.FindProperty("Controller");
            if (ControllerProp.objectReferenceValue == null)
                return;
            GridControllerObject = new SerializedObject(ControllerProp.objectReferenceValue);
            GridsProp = GridControllerObject.FindProperty("Grids");
        }

        public override void OnInspectorGUI()
        {
            if (GridControllerObject == null)
            {
                EditorGUILayout.HelpBox("Please add from GridController", MessageType.Warning);
                return;
            }
            GridControllerObject.Update();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Controller:", self.Controller, typeof(GridController), true);
            EditorGUILayout.Vector2IntField("Point:", self.Point);
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Neighborhood:");
            EditorGUI.indentLevel++;

            Grid forward = null, back = null, right = null, left = null, forwardRight = null, forwardLeft = null, backRight = null, backLeft = null;
            Vector2Int forwardSeek = self.Point - Vector2Int.down;
            Vector2Int backSeek = self.Point + Vector2Int.down;
            Vector2Int rightSeek = self.Point - Vector2Int.left;
            Vector2Int leftSeek = self.Point + Vector2Int.left;
            Vector2Int forwardRightSeek = self.Point - Vector2Int.down - Vector2Int.left;
            Vector2Int forwardLeftSeek = self.Point - Vector2Int.down + Vector2Int.left;
            Vector2Int backRightSeek = self.Point + Vector2Int.down - Vector2Int.left;
            Vector2Int backLeftSeek = self.Point + Vector2Int.down + Vector2Int.left;
            for (int i = 0; i < GridsProp.arraySize; i++)
            {
                Grid _grid = GridsProp.GetArrayElementAtIndex(i).objectReferenceValue as Grid;
                if (_grid == null)
                    continue;
                Vector2Int _point = _grid.Point;
                if (_point == forwardSeek)
                    forward = _grid;
                if (_point == backSeek)
                    back = _grid;
                if (_point == rightSeek)
                    right = _grid;
                if (_point == leftSeek)
                    left = _grid;
                if (_point == forwardRightSeek)
                    forwardRight = _grid;
                if (_point == forwardLeftSeek)
                    forwardLeft = _grid;
                if (_point == backRightSeek)
                    backRight = _grid;
                if (_point == backLeftSeek)
                    backLeft = _grid;
            }
            bool forwardLinked = false, backLinked = false, rightLinked = false, leftLinked = false, forwardRightLinked = false, forwardLeftLinked = false, backRightLinked = false, backLeftLinked = false;
            int indexForwardLink = -1, indexBackLink = -1, indexRightLink = -1, indexLeftLink = -1, indexForwardRightLink = -1, indexForwardLeftLink = -1, indexBackRightLink = -1, indexBackLeftLink = -1;
            GridController.Link forwardLink = new GridController.Link(self.Point, forwardSeek);
            GridController.Link backLink = new GridController.Link(self.Point, backSeek);
            GridController.Link rightLink = new GridController.Link(self.Point, rightSeek);
            GridController.Link leftLink = new GridController.Link(self.Point, leftSeek);
            GridController.Link forwardRightLink = new GridController.Link(self.Point, forwardRightSeek);
            GridController.Link forwardLeftLink = new GridController.Link(self.Point, forwardLeftSeek);
            GridController.Link backRightLink = new GridController.Link(self.Point, backRightSeek);
            GridController.Link backLeftLink = new GridController.Link(self.Point, backLeftSeek);
            for (int i = 0; i < self.Controller.LinkData.Count; i++)
            {
                GridController.Link _link = self.Controller.LinkData[i];
                if (_link == forwardLink)
                {
                    forwardLinked = true;
                    indexForwardLink = i;
                }
                if (_link == backLink)
                {
                    backLinked = true;
                    indexBackLink = i;
                }
                if (_link == rightLink)
                {
                    rightLinked = true;
                    indexRightLink = i;
                }
                if (_link == leftLink)
                {
                    leftLinked = true;
                    indexLeftLink = i;
                }
                if (_link == forwardRightLink)
                {
                    forwardRightLinked = true;
                    indexForwardRightLink = i;
                }
                if (_link == forwardLeftLink)
                {
                    forwardLeftLinked = true;
                    indexForwardLeftLink = i;
                }
                if (_link == backRightLink)
                {
                    backRightLinked = true;
                    indexBackRightLink = i;
                }
                if (_link == backLeftLink)
                {
                    backLeftLinked = true;
                    indexBackLeftLink = i;
                }
            }

            if (forward == null)
            {
                if (GUILayout.Button("Forward", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, forwardSeek);
                    forwardLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Forward:", forward, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                forwardLinked = EditorGUILayout.Toggle(forwardLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (back == null)
            {
                if (GUILayout.Button("Back", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, backSeek);
                    backLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Back:", back, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                backLinked = EditorGUILayout.Toggle(backLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (right == null)
            {
                if (GUILayout.Button("Right", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, rightSeek);
                    rightLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Right:", right, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                rightLinked = EditorGUILayout.Toggle(rightLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (left == null)
            {
                if (GUILayout.Button("Left", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, leftSeek);
                    leftLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Left:", left, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                leftLinked = EditorGUILayout.Toggle(leftLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (forwardRight == null)
            {
                if (GUILayout.Button("Forward Right", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, forwardRightSeek);
                    forwardRightLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Forward Right:", forwardRight, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                forwardRightLinked = EditorGUILayout.Toggle(forwardRightLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (forwardLeft == null)
            {
                if (GUILayout.Button("Forward Left", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, forwardLeftSeek);
                    forwardLeftLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Forward Left:", forwardLeft, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                forwardLeftLinked = EditorGUILayout.Toggle(forwardLeftLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (backRight == null)
            {
                if (GUILayout.Button("Back Right", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, backRightSeek);
                    backRightLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Back Right:", backRight, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                backRightLinked = EditorGUILayout.Toggle(backRightLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (backLeft == null)
            {
                if (GUILayout.Button("Back Left", GUILayout.Width(100)))
                {
                    GridControllerEditor.NewGridObj(self.Controller, backLeftSeek);
                    backLeftLinked = true;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField("Back Left:", backLeft, typeof(Grid), true, GUILayout.Width(300));
                EditorGUI.EndDisabledGroup();
                backLeftLinked = EditorGUILayout.Toggle(backLeftLinked, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            if (GUI.changed)
            {
                Vector3 gridPos = self.transform.localPosition;
                if (forwardLinked)
                {
                    if (!self.Controller.LinkData.Contains(forwardLink))
                    {
                        Vector3 pos = forward == null ? GridControllerEditor.CalcPos(self.Controller, forwardSeek) : forward.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, forwardLink, gridPos, pos);
                        forwardLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(forwardLink);
                    }
                }
                else if (indexForwardLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexForwardLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexForwardLink);
                }
                if (backLinked)
                {
                    if (!self.Controller.LinkData.Contains(backLink))
                    {
                        Vector3 pos = back == null ? GridControllerEditor.CalcPos(self.Controller, backSeek) : back.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, backLink, gridPos, pos);
                        backLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(backLink);
                    }
                }
                else if (indexBackLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexBackLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexBackLink);
                }
                if (rightLinked)
                {
                    if (!self.Controller.LinkData.Contains(rightLink))
                    {
                        Vector3 pos = right == null ? GridControllerEditor.CalcPos(self.Controller, rightSeek) : right.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, rightLink, gridPos, pos);
                        rightLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(rightLink);
                    }
                }
                else if (indexRightLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexRightLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexRightLink);
                }
                if (leftLinked)
                {
                    if (!self.Controller.LinkData.Contains(leftLink))
                    {
                        Vector3 pos = left == null ? GridControllerEditor.CalcPos(self.Controller, leftSeek) : left.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, leftLink, gridPos, pos);
                        leftLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(leftLink);
                    }
                }
                else if (indexLeftLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexLeftLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexLeftLink);
                }
                if (forwardRightLinked)
                {
                    if (!self.Controller.LinkData.Contains(forwardRightLink))
                    {
                        Vector3 pos = forwardRight == null ? GridControllerEditor.CalcPos(self.Controller, forwardRightSeek) : forwardRight.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, forwardRightLink, gridPos, pos);
                        forwardRightLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(forwardRightLink);
                    }
                }
                else if (indexForwardRightLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexForwardRightLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexForwardRightLink);
                }
                if (forwardLeftLinked)
                {
                    if (!self.Controller.LinkData.Contains(forwardLeftLink))
                    {
                        Vector3 pos = forwardLeft == null ? GridControllerEditor.CalcPos(self.Controller, forwardLeftSeek) : forwardLeft.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, forwardLeftLink, gridPos, pos);
                        forwardLeftLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(forwardLeftLink);
                    }
                }
                else if (indexForwardLeftLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexForwardLeftLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexForwardLeftLink);
                }
                if (backRightLinked)
                {
                    if (!self.Controller.LinkData.Contains(backRightLink))
                    {
                        Vector3 pos = backRight == null ? GridControllerEditor.CalcPos(self.Controller, backRightSeek) : backRight.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, backRightLink, gridPos, pos);
                        backRightLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(backRightLink);
                    }
                }
                else if (indexBackRightLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexBackRightLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexBackRightLink);
                }
                if (backLeftLinked)
                {
                    if (!self.Controller.LinkData.Contains(backLeftLink))
                    {
                        Vector3 pos = backLeft == null ? GridControllerEditor.CalcPos(self.Controller, backLeftSeek) : backLeft.transform.localPosition;
                        GameObject line = GridControllerEditor.NewLinkObj(self.Controller, backLeftLink, gridPos, pos);
                        backLeftLink.Obj = line;
                        Undo.RecordObject(self.Controller, "Change GridControllLink");
                        self.Controller.LinkData.Add(backLeftLink);
                    }
                }
                else if (indexBackLeftLink > -1)
                {
                    GameObject line = self.Controller.LinkData[indexBackLeftLink].Obj;
                    if (line != null)
                        Undo.DestroyObjectImmediate(line);
                    Undo.RecordObject(self.Controller, "Change GridControllLink");
                    self.Controller.LinkData.RemoveAt(indexBackLeftLink);
                }
                EditorUtility.SetDirty(self.Controller);
            }
        }
    }
}