namespace Contracts
{
    public class ResolvedBinding<T> : IBinding<T>
    {
        public string Path { get; }
        public T Object { get; }

        public bool Resolved => true;

        public ResolvedBinding(IBinding<T> binding, T obj)
        {
            Path = binding.Path;
            Object = obj;
        }
    }
}
