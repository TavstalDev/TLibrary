using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers.General;
using UnityEngine;
using Action = System.Action;

namespace Tavstal.TLibrary.Threading
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
        private readonly ConcurrentQueue<Action> ExecutionQueue = new ConcurrentQueue<Action>();
        private static readonly object _lock = new object();
        private static MainThreadDispatcher _instance = null!;
        private static int _mainThreadId;

        /// <summary>
        /// Saves the ID of the main thread when this object is created.
        /// </summary>
        private void Awake()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
        
        /// <summary>
        /// Processes and executes all queued actions on the main thread.
        /// </summary>
        private void Update()
        {
            while (ExecutionQueue.TryDequeue(out var action))
                action();
        }
        
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
                    lock (_lock)
                    {
                        var obj = new GameObject("MainThreadDispatcher");
                        _instance = obj.AddComponent<MainThreadDispatcher>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Checks if the current code is running on the main thread.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the current thread is the main thread; otherwise, <see langword="false"/>.
        /// </value>
        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _mainThreadId;

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
            if (!Application.isPlaying)
                return;
            if (IsMainThread)
            {
                action();
                return;
            }

            Instance.ExecutionQueue.Enqueue(action);
        }

        /// <summary>
        /// Executes an action on the main thread asynchronously.
        /// </summary>
        /// <param name="action">The action to be executed on the main thread.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing 
        /// a <see cref="bool"/> that indicates whether the action was successfully executed.
        /// </returns>
        public static Task<bool> RunOnMainThreadAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (!Application.isPlaying)
            {
                tcs.SetResult(false);
                return tcs.Task;
            }
            
            if (IsMainThread)
            {
                action();
                tcs.SetResult(true);
                return tcs.Task;
            }

            Instance.ExecutionQueue.Enqueue(() =>
            {
                try
                {
                    // Execute the provided action
                    action();
                    // Signal success
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError("Error while running async action on main thread");
                    tcs.SetResult(false);
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}