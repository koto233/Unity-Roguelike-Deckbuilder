using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : ISceneLoader
{
    private HashSet<string> _loadedScenes = new HashSet<string>();

    /// <summary>
    /// 加载叠加场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="parentContainer"></param>
    /// <returns></returns> <summary>
    public async UniTask<Scene> LoadAdditiveAsync(string sceneName, Transform parentContainer = null)
    {
        if (_loadedScenes.Contains(sceneName))
        {
            Debug.LogWarning($"场景 {sceneName} 已加载，跳过");
            return SceneManager.GetSceneByName(sceneName);
        }

        // 异步加载叠加场景
        var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;
        await loadOp.ToUniTask();

        var scene = SceneManager.GetSceneByName(sceneName);
        _loadedScenes.Add(sceneName);

        // 如果指定了父容器，将场景中的根物体转移到持久化容器下（方便统一管理）
        if (parentContainer != null)
        {
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                root.transform.SetParent(parentContainer, worldPositionStays: false);
            }
        }

        return scene;
    }
    /// <summary>
    /// 卸载叠加场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns> <summary>
    public async UniTask UnloadAdditiveAsync(string sceneName)
    {
        if (!_loadedScenes.Contains(sceneName)) return;

        var scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid())
        {
            await SceneManager.UnloadSceneAsync(scene).ToUniTask();
        }
        _loadedScenes.Remove(sceneName);
    }

    public bool IsSceneLoaded(string sceneName) => _loadedScenes.Contains(sceneName);

    public UniTask<T> LoadSceneWithContext<T>(string sceneName) where T : MonoBehaviour
    {
        throw new System.NotImplementedException();
    }
}