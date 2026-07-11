using System.Threading;
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
    }
}