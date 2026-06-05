#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BinarySerializer 
{
    public class VersionTree<T>
        where T : Enum
    {
        public VersionTree(Node root)
        {
            Root = root;
        }

        public Node Root { get; }
        public Node? Current { get; set; }

        public void Init() => Root.PropagateParents();

        public bool HasParent(T version) => Current?.HasParent(version) == true;
        public bool HasAnyParent(params T[] versions) => versions.Any(HasParent);
        public Node? FindVersion(T version) => Root.FindVersion(version);

        public class Node : IEnumerable<Node>
        {
            public Node(T version)
            {
                Version = version;
                Children = new List<Node>();
                ParentVersions = new HashSet<T>();
            }

            private List<Node> Children { get; }
            private HashSet<T> ParentVersions { get; }

            public T Version { get; }

            public bool HasParent(T version) => Version.Equals(version) || ParentVersions.Contains(version);
            public Node? FindVersion(T version)
            {
                if (Version.Equals(version))
                    return this;

                return Children.Select(child => child.FindVersion(version)).FirstOrDefault(result => result != null);
            }

            public void PropagateParents() 
            {
                foreach (Node child in Children) 
                {
                    foreach (T pv in ParentVersions)
                        child.ParentVersions.Add(pv);

                    child.ParentVersions.Add(Version);
                    child.PropagateParents();
                }
            }

            public void Add(Node node) => Children.Add(node);
            public void AddRange(IEnumerable<Node> nodes) => Children.AddRange(nodes);

            public IEnumerator<Node> GetEnumerator() => Children.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}