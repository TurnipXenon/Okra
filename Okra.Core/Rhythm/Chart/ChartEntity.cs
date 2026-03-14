using System.Collections.Generic;

namespace Okra.Core.Rhythm.Chart;

/**
 * todo(turnip): improve documentation
 * Reflects the JSON structure
 */
public class ChartEntity
{
    public List<BeatEntity> BeatList { get; set; } = new();
}