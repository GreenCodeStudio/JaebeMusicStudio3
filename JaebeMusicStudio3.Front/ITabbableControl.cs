namespace JaebeMusicStudio3.Front;

public interface ITabbableControl
{
    string Title { get; }
    event Action ChangedMetadata;
}