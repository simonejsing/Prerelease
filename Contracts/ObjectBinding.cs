namespace Contracts
{
    public class ObjectBinding<T> : IBinding<T>
    {
        public ObjectBinding(string path)
        {
            Path = path;
        }

        public T Object
        {
            get
            {
                throw new UnresolvedBindingException($"Attempt to access unresolved object binding: {Path}");
            }
        }

        public bool Resolved => false;
        public string Path { get; set; }
    }
}