using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Rocket.API;
using Tavstal.TLibrary.Models;

namespace Tavstal.TLibrary.Helpers
{
    /// <summary>
    /// Provides per-player, per-kind asynchronous locking utilities.
    /// </summary>
    public static class LockHelper
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _commandLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _uiLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Asynchronously acquires a lock for the specified player and <see cref="ELockKind"/>.
        /// </summary>
        /// <param name="caller">The player requesting the lock.</param>
        /// <param name="kind">The kind of lock to acquire.</param>
        /// <returns>A <see cref="Task"/> that completes when the lock has been acquired.</returns>
        public static Task WaitForLockAsync(IRocketPlayer caller, ELockKind kind)
        {
            ConcurrentDictionary<string, SemaphoreSlim> locks = GetLocks(kind);
            var semaphoreSlim = locks.GetOrAdd(caller.Id, new SemaphoreSlim(1, 1));
            return semaphoreSlim.WaitAsync();
        }

        /// <summary>
        /// Releases the lock held by the specified player for the given <see cref="ELockKind"/>.
        /// </summary>
        /// <param name="caller">The player whose lock should be released.</param>
        /// <param name="kind">The kind of lock to release.</param>
        public static void ReleaseLock(IRocketPlayer caller,  ELockKind kind)
        {
            ConcurrentDictionary<string, SemaphoreSlim> locks = GetLocks(kind);
            if (!locks.TryRemove(caller.Id, out SemaphoreSlim semaphoreSlim))
                return;
            semaphoreSlim.Release();
        }

        /// <summary>
        /// Returns the <see cref="ConcurrentDictionary{TKey,TValue}"/> that stores locks for the specified <see cref="ELockKind"/>.
        /// </summary>
        /// <param name="kind">The lock kind whose dictionary is returned.</param>
        /// <returns>The dictionary of active locks for <paramref name="kind"/>.</returns>
        /// <exception cref="Exception">Thrown when <paramref name="kind"/> is not a recognized value.</exception>
        private static ConcurrentDictionary<string, SemaphoreSlim> GetLocks(ELockKind kind)
        {
            return kind switch
            {
                ELockKind.COMMAND => _commandLocks,
                ELockKind.UI => _uiLocks,
                _ => throw new Exception("Unknown ELockKind")
            };
        }
    }
}