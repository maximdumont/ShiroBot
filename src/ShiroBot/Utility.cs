using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShiroBot
{

    public class Utility
    {
        public static string FormatBytes(double bytes)
        {
            string type;
            if (bytes > 1024 * 1024)
            {
                type = "mb";
                bytes /= (1024 * 1024);
            }
            else if (bytes > 1024)
            {
                type = "kb";
                bytes /= 1024;
            }
            else
                type = "b";
            return $"{bytes:0}{type}";
        }

        // Gets the path only
        public static string GetDirectoryName(string name)
        {
            try
            {
                name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                return name.Substring(0, name.LastIndexOf(Path.DirectorySeparatorChar));
            }
            catch
            {
                return null;
            }
        }

        public static string GetFileNameWithoutExtension(string value)
        {
            var lastIndex = value.Length - 1;
            for (var i = lastIndex; i >= 1; i--)
            {
                if (value[i] != '.') continue;
                lastIndex = i - 1;
                break;
            }
            var firstIndex = 0;
            for (var i = lastIndex - 1; i >= 0; i--)
            {
                switch (value[i])
                {
                    case '/':
                    case '\\':
                        {
                            firstIndex = i + 1;
                            goto End;
                        }
                }
            }
        End:
            return value.Substring(firstIndex, (lastIndex - firstIndex + 1));
        }

        public static string CleanPath(string path)
        {
            return path?.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }
    }

    public static class StringExtensions
    {
        public static string UppercaseFirst(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
    }

    // A partially thread-safe HashSet (iterating is not thread-safe)
    public class ConcurrentHashSet<T> : ICollection<T>
    {
        readonly HashSet<T> collection;
        readonly object syncRoot = new object();

        public ConcurrentHashSet()
        {
            collection = new HashSet<T>();
        }

        public ConcurrentHashSet(ICollection<T> values)
        {
            collection = new HashSet<T>(values);
        }

        public bool IsReadOnly => false;
        public int Count { get { lock (syncRoot) return collection.Count; } }
        public bool Contains(T value) { lock (syncRoot) return collection.Contains(value); }
        public bool Add(T value) { lock (syncRoot) return collection.Add(value); }
        public bool Remove(T value) { lock (syncRoot) return collection.Remove(value); }
        public void Clear() { lock (syncRoot) collection.Clear(); }
        public void CopyTo(T[] array, int index) { lock (syncRoot) collection.CopyTo(array, index); }
        public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();
        public bool Any(Func<T, bool> callback) { lock (syncRoot) return collection.Any(callback); }
        public T[] ToArray() { lock (syncRoot) return collection.ToArray(); }

        public bool TryDequeue(out T value)
        {
            lock (syncRoot)
            {
                value = collection.ElementAtOrDefault(0);
                if (value != null) collection.Remove(value);
                return value != null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<T>.Add(T value) => Add(value);
    }
}

