using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoader
{
    // 加载叠加场景，并挂载到指定父级下（可选）
    UniTask<Scene> LoadAdditiveAsync(string sceneName, Transform parentContainer = null);
    UniTask UnloadAdditiveAsync(string sceneName);
    bool IsSceneLoaded(string sceneName);
}