using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Helpers
{
    public static class TaskExtensions
    {
        public static Task ThenAfterDelay<TResult>(this Task instance, Func<Task, TResult> action, int delay)
        {
            return instance.ContinueWith(_ => Task.Delay(delay).ContinueWith(action));
        } 
    }
}