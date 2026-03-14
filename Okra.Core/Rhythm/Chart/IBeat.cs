using Okra.Core.Rhythm.Input;

namespace Okra.Core.Rhythm.Chart;

public interface IBeat: IRhythmInputListener
{
    void SetVisualizer(IBeatVisualizer visualizer);
    BeatInputResult OnSimulateInputRelease();
    IBeatVisualizer? GetVisualizer();
}