using System.Collections.Generic;
using System.IO;
using DaGenGraph.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace DaGenGraph.Example
{
    public class ExampleGraphWindow: GraphWindow<ExampleGraph>
    {
        private string path;
        internal static ExampleGraphWindow instance
        {
            get
            {
                if (s_Instance != null) return s_Instance;
                var windows = Resources.FindObjectsOfTypeAll<ExampleGraphWindow>();
                s_Instance = windows.Length > 0 ? windows[0] : null;
                if (s_Instance != null) return s_Instance;
                s_Instance = CreateWindow<ExampleGraphWindow>();
                return s_Instance;
            }
        }

        private static ExampleGraphWindow s_Instance;

        [MenuItem("DaGenGraph/ExampleGraphWindow")]
        public static void GetWindow()
        {
            instance.titleContent = new GUIContent("ExampleGraphWindow");
            instance.Show();
            instance.InitGraph();
        }

        protected override void InitGraph()
        {
            path = null;
            base.InitGraph();
        }

        protected override ExampleGraph CreateGraph()
        {
            return new ExampleGraph();
        }

        protected override void OpenGraph()
        {
            string searchPath = EditorUtility.OpenFilePanel($"新建{typeof(ExampleGraph).Name}配置文件", "Assets", "json");
            if (!string.IsNullOrEmpty(searchPath))
            { 
                var jStr = File.ReadAllText(searchPath);
                var obj = JsonConvert.DeserializeObject<ExampleGraph>(jStr, new JsonSerializerSettings()
                {
                    Converters = new List<JsonConverter>()
                    {
                        new UnityJsonConverter()
                    }
                });
                m_Graph = obj;
                path = searchPath;
            }
        }

        protected override void SaveGraph()
        {
            if (string.IsNullOrEmpty(path))
            {
                string searchPath = EditorUtility.SaveFilePanel($"新建{typeof(ExampleGraph).Name}配置文件", "Assets",
                    typeof(ExampleGraph).Name, "json");
                if (!string.IsNullOrEmpty(searchPath))
                {
                    path = searchPath;
                }
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(m_Graph, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new UnityJsonConverter()
                }
            }));
            AssetDatabase.Refresh();
        }
    }
}