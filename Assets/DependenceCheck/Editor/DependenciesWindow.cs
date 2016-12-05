using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dependend.Check
{
    public class DependenciesWindow : EditorWindow
    {

        private int currentSelectInstanceID;
        private string currentSelectPath;
        private UnityEngine.Object currentObject;
        private string[] dependencies;
        private UnityEngine.Object[] dependObjects;

        private bool checkRecursive;
        private bool serchedRecursive;

        private bool locked = false;

        private Vector2 scrollPos;

        [MenuItem("Tools/DependenciesCheck")]
        public static void Create()
        {
            EditorWindow.GetWindow<DependenciesWindow>();
        }

        void OnGUI()
        {
            if (string.IsNullOrEmpty(currentSelectPath)) { return; }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select Assets");
            if (locked)
            {
                locked = GUILayout.Button("Lock", GUILayout.Width(100));
            }
            else
            {
                locked = !GUILayout.Button("unlock", GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(" " + currentSelectPath);
            EditorGUILayout.ObjectField(currentObject, typeof(UnityEngine.Object), GUILayout.Width(150));
            EditorGUILayout.EndHorizontal();
            this.checkRecursive = EditorGUILayout.Toggle("Check recursive dependencies", this.checkRecursive);

            EditorGUILayout.LabelField("Dependencies");
            if (this.dependencies == null)
            {
                EditorGUILayout.LabelField(" No Dependencies");
                return;
            }
            this.scrollPos = EditorGUILayout.BeginScrollView(this.scrollPos);
            int length = dependencies.Length;
            for (int i = 0; i < length; ++ i )
            {
                string depend = this.dependencies[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  " + depend);
                EditorGUILayout.ObjectField(dependObjects[i], typeof(UnityEngine.Object), GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        void Update()
        {
            if (locked) { return; }
            if (currentSelectInstanceID != Selection.activeInstanceID || this.serchedRecursive != this.checkRecursive)
            {
                this.currentSelectInstanceID = Selection.activeInstanceID;
                this.currentSelectPath = AssetDatabase.GetAssetPath(this.currentSelectInstanceID);
                this.currentObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(this.currentSelectPath);
                this.dependencies = AssetDatabase.GetDependencies(this.currentSelectPath, this.checkRecursive);
                this.serchedRecursive = this.checkRecursive;
                int length = dependencies.Length;
                this.dependObjects = new UnityEngine.Object[length];
                for (int i = 0; i < length; ++i)
                {
                    dependObjects[i] = AssetDatabase.LoadAssetAtPath<Object>(dependencies[i]);
                }
                this.Repaint();
            }
        }
    }
}