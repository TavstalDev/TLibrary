using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Tavstal.TLibrary
{
    /// <summary>
    /// A singleton class that dispatches actions to be executed on the main thread in Unity.
    /// </summary>
    /// <remarks>
    /// This class ensures that actions are executed on the main thread, which is important for thread safety in Unity.
    /// It uses a <see cref="ConcurrentQueue{Action}"/> to queue actions and processes them in the <see cref="Update"/> method.
    /// </remarks>
    public class MainThreadDispatcher : MonoBehaviour
    {
        private readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();
        private static MainThreadDispatcher _instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="MainThreadDispatcher"/>. If the instance does not exist, it creates one.
        /// </summary>
        /// <value>
        /// The singleton instance of the <see cref="MainThreadDispatcher"/>.
        /// </value>
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

        /// <summary>
        /// Processes and executes all queued actions on the main thread.
        /// </summary>
        private void Update()
        {
            while (_executionQueue.TryDequeue(out var action))
            {
                action();
            }
        }

        /// <summary>
        /// Enqueues an action to be executed on the main thread.
        /// </summary>
        /// <param name="action">The action to be executed on the main thread.</param>
        /// <remarks>
        /// This method ensures that the action is enqueued and will be executed during the next <see cref="Update"/> call.
        /// The action will only be enqueued if the application is currently playing.
        /// </remarks>
        public static void RunOnMainThread(Action action)
        {
            if (Application.isPlaying)
            {
                Instance._executionQueue.Enqueue(action);
            }
        }
    }

}