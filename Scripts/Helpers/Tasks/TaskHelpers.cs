using System;
using System.Threading.Tasks;

namespace Helpers
{
    public static class TaskHelpers
    {
        public static Task RunAfterDelay<TResult>(Func<Task, TResult> action, int delay)
        {
            return Task.Run(() => Task.Delay(delay).ContinueWith(action));
        } 
    }
}