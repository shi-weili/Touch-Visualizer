using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseFilter
{
    public const int MaxNumSamples = 10;

    int _numSamples = 2;

    public int NumSamples
    {
        get => _numSamples;
        set
        {
            if (value > 0 && value <= MaxNumSamples)
            {
                _numSamples = value;
            }
        }
    }

    /// <summary>
    /// Return filtered result based on samples, using up to NumSamples samples from the end of the samples array.
    /// </summary>
    /// <param name="samples"></param>
    /// <returns></returns>
    public abstract Vector2 Filter(Vector2[] samples);
}
