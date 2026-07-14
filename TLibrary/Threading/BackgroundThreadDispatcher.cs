using System;
using System.Threading.Tasks;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TLibrary.Threading
{
    /// <summary>
    /// Provides fire-and-forget task execution with automatic exception handling.
    /// </summary>
    public static class BackgroundThreadDispatcher
    {
        /// <summary>
        /// Runs an action on a background thread, logging any exceptions.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="context">Optional context string for logging.</param>
        public static void Run(Action action, string context = "unknown")
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError($"Unexpected error happened while executing async task in '{context}' context: {ex}");
                }
            });
        }

        /// <summary>
        /// Runs an action on a background thread and awaits its completion, logging any exceptions.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="context">Optional context string for logging.</param>
        public static async Task RunAsync(Action action, string context = "unknown")
        {
            await Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogError($"Unexpected error happened while executing async task in '{context}' context: {ex}");
                }
            });
        }

        /// <summary>
        /// Runs a task on a background thread and awaits its completion, logging any exceptions.
        /// </summary>
        /// <param name="taskFactory">The task to execute.</param>
        /// <param name="context">Optional context string for logging.</param>
        public static async Task RunAsync(Func<Task> taskFactory, string context = "unknown")
        {
            try
            {
                await Task.Run(taskFactory);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError($"Unexpected error happened while executing async task in '{context}' context: {ex}");
            }
        }
    }
}