#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 强制游戏从 RootScene 启动，即使在编辑器中打开了其他场景
/// </summary>
[InitializeOnLoad]
public static class GameBootstrapper
{
    // ！！注意：这里改成你的 RootScene 实际路径 ！！
    private const string ROOT_SCENE_PATH = "Assets/Scenes/Root Scene.unity";

    static GameBootstrapper()
    {
        // 监听编辑器播放状态变化
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // 关键时机：正在退出编辑模式（即即将开始运行时）
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;

            // 如果当前打开的场景不是 RootScene
            if (currentScenePath != ROOT_SCENE_PATH)
            {
                // 询问是否保存当前场景（防止丢失未保存的修改）
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // 强制打开 RootScene（替换当前场景）
                    EditorSceneManager.OpenScene(ROOT_SCENE_PATH);
                }
                else
                {
                    // 如果用户取消了保存，可以选择不执行 Play（这里保持原样）
                    // 但为了体验，我们可以直接强制打开
                    EditorSceneManager.OpenScene(ROOT_SCENE_PATH);
                }
            }
        }
    }
}
#endif