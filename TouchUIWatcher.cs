using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Syy.Tool
{
    public class TouchUIWatcher : EditorWindow
    {
        [MenuItem("Window/TouchUIWatcher")]
        public static void Open()
        {
            GetWindow<TouchUIWatcher>("TouchUIWatcher");
        }

        List<RaycastResult> results = new List<RaycastResult>();
        Vector2 scrollPosition;

        void OnEnable()
        {
           EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            if (Application.isPlaying && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;
                EventSystem.current.RaycastAll(pointer, results);
                scrollPosition = Vector2.zero;
                if (results.Any())
                {
                    if (Data.instance.type == HitActionType.Highlight)
                    {
                        EditorGUIUtility.PingObject(results.First().gameObject);
                    }
                    else if (Data.instance.type == HitActionType.Inspector)
                    {
                        var first = results.First().gameObject;
                        EditorGUIUtility.PingObject(first);
                        Selection.activeGameObject = first;
                    }
                }
                Repaint();
            }
        }

        void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Mode", GUILayout.Width(40));
                DrawBehaviourButton(HitActionType.None, EditorStyles.miniButtonLeft);
                DrawBehaviourButton(HitActionType.Highlight, EditorStyles.miniButtonMid);
                DrawBehaviourButton(HitActionType.Inspector, EditorStyles.miniButtonRight);
            }

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Playing Only\n・None: Nothing when touch\n・Highlight: Ping when touch\n・Inspector: Ping and select when touch", MessageType.Info);
                return;
            }

            EditorGUILayout.LabelField($"Hit {results.Count}");
            if (results.Count > 0)
            {
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition, false, true))
                    {
                        scrollPosition = scroll.scrollPosition;
                        foreach (var result in results)
                        {
                            EditorGUILayout.ObjectField(result.gameObject, typeof(GameObject), true);
                        }
                    }
                }
            }
        }

        void DrawBehaviourButton(HitActionType type, GUIStyle style)
        {
            var prev = Data.instance.type == type;
            var next = GUILayout.Toggle(prev, type.ToString(), style);
            if (prev != next && next)
            {
                Data.instance.type = type;
            }
        }

        class Data : ScriptableSingleton<Data>
        {
            public HitActionType type = HitActionType.Highlight;
        }

        enum HitActionType
        {
            None,
            Highlight,
            Inspector,
        }
    }
}
