// Scripts/Framework/CoroutineRunner.cs
using System.Collections;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 全局协程宿主，用于非 MonoBehaviour 类启动协程
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("[CoroutineRunner]");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }


        // public void StopCoroutine(Coroutine coroutine)
        // {
        //     if (coroutine != null)
        //         ((MonoBehaviour)this).StopCoroutine(coroutine);
        // }
    }
}