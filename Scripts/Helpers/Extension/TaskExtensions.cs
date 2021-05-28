using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Helpers
{
    public static class TaskExtensions
    {
        public static Task ThenAfterDelay(this Task instance, Action action, int delay)
        {
            return instance.ContinueWith(_ => Task.Delay(delay).ContinueWith(action));
        }
        
        public static Task ContinueWith(this Task task, Action action)
        {
            return task.ContinueWith(_ => action());
        }
    }
}