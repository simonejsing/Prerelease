using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class SharedObjectBinding<T> : IBinding<T>
    {
        public IBinding<T> InnerBinding { get; set; }

        public string Path => InnerBinding.Path;
        public T Object => InnerBinding.Object;
        public bool Resolved => InnerBinding.Resolved;

        public SharedObjectBinding(IBinding<T> innerBinding)
        {
            InnerBinding = innerBinding;
        }
    }
}
