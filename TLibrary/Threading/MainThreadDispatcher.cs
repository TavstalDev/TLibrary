using System;
using System.Threading;
using System.Threading.Tasks;
using Rocket.Core.Utils;
using SDG.Unturned;
using Action = System.Action;

namespace Tavstal.TLibrary.Threading
{
    /// <summary>
    /// Ensures an action runs on the game's main thread.
    /// </summary>
    public static class MainThreadDispatcher
    {
        /// <summary>
        /// Runs the action immediately if already on the main thread, otherwise queues it.
        /// </summary>
        /// <param name="action">The action to execute on the main thread.</param>
        public static void Run(Action action)
        {
            if (Thread.CurrentThread == ThreadUtil.gameThread)
            {
                action();
                return;
            }
            TaskDispatcher.QueueOnMainThread(action);
        }
        
        /// <summary>
        /// Asynchronously runs the action on the game's main thread.
        /// </summary>
        /// <param name="action">The action to execute on the main thread.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous execution of the action.</returns>
        public static Task RunAsync(Action action)
        {
            if (Thread.CurrentThread == ThreadUtil.gameThread)
            {
                action();
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            TaskDispatcher.QueueOnMainThread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null!);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
        
        /// <summary>
        /// Asynchronously runs the function on the game's main thread and returns its result.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the function.</typeparam>
        /// <param name="func">The function to execute on the main thread.</param>
        /// <returns>A <see cref="Task{T}"/> that represents the asynchronous execution of the function, containing its result.</returns>
        public static Task<T> RunAsync<T>(Func<T> func)
        {
            if (Thread.CurrentThread == ThreadUtil.gameThread)
                return Task.FromResult(func());

            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            TaskDispatcher.QueueOnMainThread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}