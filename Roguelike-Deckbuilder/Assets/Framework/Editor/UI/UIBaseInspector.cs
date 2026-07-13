using UnityEngine;
using UnityEditor;
using System.Linq;
using LitFramework.UI.Core.Window;
using LitFramework.UI.Core.Utility;
using LitFramework.UI.Core;
using System.Collections.Generic;
namespace LitFramework.UI.EditorTools
{
    [CustomEditor(typeof(UIBase), true)]
    public class UIBaseInspector : Editor
    {
        SerializedProperty _pathProp;
        private Vector2 _bindPreviewScroll;

        void OnEnable()
        {
            try
            {
                // 只有 target 存在且不是销毁状态才尝试获取
                if (target != null)
                {
                    _pathProp = serializedObject.FindProperty("_generatedScriptPath");
                }
            }
            catch (System.Exception e)
            {
                // 捕获异常，打印轻度警告，不影响编辑器其他功能
                Debug.LogWarning($"UIBaseInspector 加载序列化数据失败（可能选中了损坏的预制体）: {e.Message}");
                _pathProp = null; // 置空，后续 GUI 逻辑判断跳过显示即可
            }
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

            //  自动生成绑定预览
            DrawBindingsPreview(ui);
            EditorGUILayout.Space();

            //  其余正常字段（排除自动生成字段和 m_Script）
            DrawPropertiesExcluding(serializedObject, new string[] { "m_Script", "_generatedScriptPath" });

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            GUILayout.Label("UI 自动绑定", EditorStyles.boldLabel);

            if (GUILayout.Button("🔧 生成 UI 绑定代码"))
            {
                UIAutoBindGenerator.CollectAndGenerate(ui);
            }
        }
        /// <summary>
        /// 绘制自动绑定字段预览
        /// </summary>
        /// <param name="ui"></param>
        void DrawBindingsPreview(UIBase ui)
        {
            var binds = new List<UIBind>();
            UIAutoBindGenerator.CollectBindsExcludingSubUI(ui.transform, ui, binds);
            if (binds.Count == 0) return;

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
    }
}