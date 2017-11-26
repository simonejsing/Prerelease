namespace Contracts
{
    public interface IBinding<out T>
    {
        string Path { get; }
        T Object { get; }
        bool Resolved { get; }
    }
}