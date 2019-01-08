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
                if (Data.instance.highlight && results.Any())
                {
                    EditorGUIUtility.PingObject(results.First().gameObject);
                }
                Repaint();
            }
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Playing only");
                return;
            }

            Data.instance.highlight = EditorGUILayout.ToggleLeft($"Highlight     Hit {results.Count}", Data.instance.highlight);
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

        class Data : ScriptableSingleton<Data>
        {
            public bool highlight = true;
        }
    }
}
