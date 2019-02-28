using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowGraph.Editor.GUIExtensions
{

    public static class GUIExtensions
    {

        #region Label Editable

        public static string LabelEditable(this GUIStyle style, Rect rect, GUIContent text, params GUILayoutOption[] options)
        {
            return LabelEditable(style, rect, text, "textfield", options);
        }
        public static string LabelEditable(this GUIStyle style, Rect rect, GUIContent text, GUIStyle textStyle, params GUILayoutOption[] options)
        {
            int ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            var state = (EditableLabelState)GUIUtility.GetStateObject(typeof(EditableLabelState), ctrlId);
            var evt = Event.current;


            if (state.isEdit)
            {
                if (evt.type == EventType.KeyDown)
                {

                    if (evt.keyCode == KeyCode.Return)
                    {
                        state.isEdit = false;
                        GUIUtility.keyboardControl = 0;
                        evt.Use();
                    }
                    else if (evt.keyCode == KeyCode.Escape)
                    {
                        state.isEdit = false;
                        state.text = state.old;
                        GUIUtility.keyboardControl = 0;
                        evt.Use();
                    }

                }


                state.text = UnityEditor.EditorGUI.TextField(rect, state.text, textStyle);

                if (evt.type == EventType.MouseDown)
                {
                    if (!rect.Contains(evt.mousePosition))
                    {
                        state.isEdit = false;
                        GUIUtility.keyboardControl = 0;
                        evt.Use();
                    }
                }


                if (!state.isEdit)
                {
                    if (!string.Equals(text.text, state.text))
                    {
                        GUI.changed = true;
                        text.text = state.text;
                    }
                }
            }
            else
            {
                if (GUI.Button(rect, text, style))
                {
                    state.isEdit = true;
                    state.text = text.text;
                    state.old = text.text;
                    // GUIUtility.keyboardControl = ctrlId;
                }

            }
            return text.text;
        }
        public static string LabelEditable(this GUIStyle style, GUIContent text, params GUILayoutOption[] options)
        {
            return LabelEditable(style, text, "textfield", options);
        }
        public static string LabelEditable(this GUIStyle style, GUIContent text, GUIStyle textStyle, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(text, style, options);
            return LabelEditable(style, rect, text, textStyle);
        }


        private class EditableLabelState
        {
            public bool isEdit;
            public string text;
            public string old;
        }

        #endregion


        #region Drag


        public static bool GUIDragControl(this Rect rect)
        {
            var evt = Event.current;
            var ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            DragControlState state = (DragControlState)GUIUtility.GetStateObject(typeof(DragControlState), ctrlId);
            bool dragStart = false;
            switch (evt.type)
            {
                case EventType.MouseDown:
                    if (rect.Contains(evt.mousePosition))
                    {
                        state.state = 1;
                    }
                    break;
                case EventType.MouseDrag:
                    if (state.state == 1 && rect.Contains(evt.mousePosition))
                    {
                        state.state = 2;
                        dragStart = true;
                        // evt.Use();
                    }
                    break;
                case EventType.MouseUp:

                    if (state.state == 2)
                    {
                        // evt.Use();
                    }
                    state.state = 0;
                    break;
            }
            return dragStart;
        }

        private class DragControlState
        {
            public int state;
        }

        #endregion


    }

}