using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Tavstal.TLibrary
{
    public class MainThreadDispatcher : MonoBehaviour
    {
        private readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
        private static MainThreadDispatcher _instance;

        public static MainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("MainThreadDispatcher");
                    _instance = obj.AddComponent<MainThreadDispatcher>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        private void Update()
        {
            while (_executionQueue.TryDequeue(out var action))
            {
                action();
            }
        }

        public static void RunOnMainThread(Action action)
        {
            if (Application.isPlaying)
            {
                Instance._executionQueue.Enqueue(action);
            }
        }
    }

}