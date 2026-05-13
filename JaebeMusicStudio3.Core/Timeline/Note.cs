namespace JaebeMusicStudio3.Core.Timeline;

public class Note
{
    public double Pitch { get; set; }
    public double Start { get; set; }
    public double Length { get; set; }
    public double Volume { get; set; } = 1;
}