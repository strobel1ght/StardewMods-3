using System;
using System.Collections.Generic;
using Phrasefable.StardewMods.StarUnit.Framework.Definitions;

namespace Phrasefable.StardewMods.StarUnit.Internal.TestListers
{
    internal class VerboseLister : ILister
    {
        private const char Separator = '/';
        private readonly Action<string> _writer;

        private readonly IDictionary<IIdentifiable, string>
            _fullyQualifiedIds = new Dictionary<IIdentifiable, string>();


        public VerboseLister(Action<string> writer)
        {
            this._writer = writer;
        }


        public void List(ITraversable node)
        {
            this.ListVerbose(node, null);
        }


        private void ListVerbose(IIdentifiable node, IIdentifiable parent)
        {
            this._writer(this.GetVerboseListing(node, parent));

            if (node is ITraversableBranch branch)
            {
                foreach (ITraversable child in branch.Children)
                {
                    this.ListVerbose(child, branch);
                }
            }
        }


        private string GetVerboseListing(IIdentifiable node, IIdentifiable parent)
        {
            string s = this.GetFullyQualifiedId(node, parent);
            if (node is ITraversableBranch) s += VerboseLister.Separator;
            if (!string.IsNullOrWhiteSpace(node.LongName)) s += $"    ({node.LongName})";
            return s;
        }


        private string GetFullyQualifiedId(IIdentifiable node, IIdentifiable parent)
        {
            if (this._fullyQualifiedIds.TryGetValue(node, out string id)) return id;

            id = parent switch
            {
                null => node.Key,
                _ => $"{this._fullyQualifiedIds[parent]}{VerboseLister.Separator}{node.Key}"
            };

            this._fullyQualifiedIds[node] = id;
            return id;
        }
    }
}
