using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Helpers
{

    public static class NodeExtensions
    {
        public static IEnumerable<T> GetChildrenWhere<T>(this Node instance, Func<T, bool> predicate)
        {
            return instance.GetChildren().Cast<T>().Where(predicate);
        }
    }
}