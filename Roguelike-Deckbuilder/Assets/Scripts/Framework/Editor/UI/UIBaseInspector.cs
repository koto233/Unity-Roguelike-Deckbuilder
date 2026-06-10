using UnityEngine;
using UnityEditor;
using System.Linq;
using LitFramework.UI.Core.Window;
using LitFramework.UI.Core.Utility;
using LitFramework.UI.Core;
namespace LitFramework.UI.EditorTools
{
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseInspector : Editor
    {
        SerializedProperty _pathProp;
        private Vector2 _bindPreviewScroll;

        void OnEnable()
        {
            _pathProp = serializedObject.FindProperty("_generatedScriptPath");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // ===== 代码生成配置 =====
            EditorGUILayout.LabelField("UI 自动绑定 · 生成设置", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                _pathProp,
                new GUIContent("生成脚本路径")
            );

            EditorGUILayout.HelpBox(
                "示例：UI/Generated\n\n建议放在 Generated / Auto / Bind 等目录下，避免手改。",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            var ui = target as UIBase;
            if (ui == null) return;

            // 1️⃣ 自动生成绑定预览
            DrawBindingsPreview(ui);
            DrawSubUIPreview(ui);
            EditorGUILayout.Space();

            // 2️⃣ 其余正常字段（排除自动生成字段和 m_Script）
            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "_generatedScriptPath" });

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            GUILayout.Label("UI 自动绑定", EditorStyles.boldLabel);

            if (GUILayout.Button("🔧 生成 UI 绑定代码"))
            {
                UIAutoBindGenerator.Generate(ui);
            }
        }

        void DrawBindingsPreview(UIBase ui)
        {
            var binds = ui.GetComponentsInChildren<UIBind>(true);
            if (binds == null || binds.Length == 0) return;

            EditorGUILayout.LabelField("自动绑定预览", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            foreach (var bind in binds)
            {
                var target = bind.Target;

                Color old = GUI.color;

                if (target == null)
                    GUI.color = new Color(1f, 0.5f, 0.5f); // 红色表示绑定失败
                else
                    GUI.color = new Color(0.8f, 1f, 0.8f); // 绿色表示绑定成功

                EditorGUILayout.BeginHorizontal();

                // 字段名显示
                EditorGUILayout.LabelField($"@_{bind.name}", GUILayout.Width(160));

                // 类型/对象显示
                EditorGUILayout.ObjectField(target, typeof(Component), true);
                // 快捷按钮，手动选目标（可选）
                if (GUILayout.Button("查看", GUILayout.Width(40)))
                {
                    Selection.activeObject = bind.gameObject;
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = old;
            }

            EditorGUILayout.EndVertical();
        }
        void DrawSubUIPreview(UIBase ui)
        {
            var subUIs = ui.GetComponentsInChildren<UIBase>(true)
                           .Where(x => x != ui)
                           .ToArray();

            if (subUIs.Length == 0)
                return;

            var referenced = UIBindAutoResolver.CollectReferencedUIs(ui);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("子 UI 模块预览", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box");

            foreach (var sub in subUIs)
            {
                bool used = referenced.TryGetValue(sub, out var fieldName);
                string label = used
                              ? $"已引用 ({fieldName})"
                              : "未引用!";
                Color old = GUI.color;
                GUI.color = used
                    ? new Color(0.7f, 1f, 0.7f)
                    : new Color(1f, 0.85f, 0.5f);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(sub.GetType().Name, GUILayout.Width(160));
                EditorGUILayout.ObjectField(sub, typeof(UIBase), true);

                if (GUILayout.Button("查看", GUILayout.Width(40)))
                {
                    Selection.activeObject = sub.gameObject;
                }

                EditorGUILayout.LabelField(label, GUILayout.Width(120));

                EditorGUILayout.EndHorizontal();
                GUI.color = old;
            }

            EditorGUILayout.EndVertical();
        }

    }
}