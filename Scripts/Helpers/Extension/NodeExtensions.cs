using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Helpers
{
    public static class NodeExtensions
    {
        
        public static IEnumerable<T> GetChildren<T>(this Node instance)
        {
            return instance.GetChildren().OfType<T>();
        }
        
        public static IEnumerable<T> GetChildrenWhere<T>(this Node instance, Func<T, bool> predicate) where T : Node
        {
            return instance.GetChildren<T>().Where(predicate);
        }
        
        /**
         * If FindNode() and GetChildrenWhere() had a baby
         */
        public static T? FindInChildrenWhere<T>(this Node instance, Func<T, bool>? predicate = null, bool recursive = false) where T : Node
        {
            // We start by getting all the children of the searched type
            var results = instance.GetChildren().OfType<T>();

            // We filter the list if possible
            if (predicate != null)
                results = results.Where(predicate);

            // If we don't have a result yet, but told to keep looking...
            if (recursive && !results.Any())
            {
                // ... we iterate over each child and do a recursive call
                T? result = null;
                foreach (var node in instance.GetChildren().Cast<Node>())
                {
                    // The first to gives a result is taken
                    result ??= node.FindInChildrenWhere(predicate, recursive);
                    if (result != null) break;
                }

                return result;
            }

            // If we have a result or don't (but don't need to keep searching) we return
            return results.FirstOrDefault();
        }
        
        /**
         * If FindNode() and GetChildrenWhere() had a baby
         */
        public static T? FindInChildren<T>(this Node instance, bool recursive = false) where T : Node
        {
            return instance.FindInChildrenWhere<T>(null, recursive);
        }
    }
}