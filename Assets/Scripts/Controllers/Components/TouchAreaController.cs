using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TouchAreaController
{
    // Actions
    public Action<Vector2[]> tapped;

    // References
    VisualElement root;
    VisualElement[] fingers = new VisualElement[BaseFilter.MaxNumSamples];
    VisualElement movingAverageFilterVE;
    VisualElement harmonicFilterVE;
    VisualElement target;

    BaseFilter movingAverageFilter = new MovingAverageFilter();
    BaseFilter harmonicFilter = new HarmonicFilter();

    // Public data and states
    public int NumSamples
    {
        get => _numSamples;
        set
        {
            _numSamples = Math.Clamp(value, 1, BaseFilter.MaxNumSamples);
            UpdateFingers();
            UpdateFilterResults();
        }
    }

    public bool ShowMovingAverageFilter
    {
        get => _showMovingAverageFilter;
        set
        {
            _showMovingAverageFilter = value;
            UpdateFilters();
        }
    }

    public bool ShowHarmonicFilter
    {
        get => _showHarmonicFilter;
        set
        {
            _showHarmonicFilter = value;
            UpdateFilters();
        }
    }

    public bool ShowTarget
    {
        get => _showTarget;
        set
        {
            _showTarget = value;
            UpdateTargetVisibility();
        }
    }

    public Vector2? MovingAverageFilterResult
    {
        get => movingAverageFilterResult;
    }

    public Vector2? HarmonicFilterResult
    {
        get => harmonicFilterResult;
    }

    public Vector2 TargetPosition
    {
        get => target.layout.center;
    }

    // Internal Data and states
    Queue<Vector2> touchQueue = new Queue<Vector2>();

    Vector2? movingAverageFilterResult = null;
    Vector2? harmonicFilterResult = null;

    int _numSamples = 2;
    bool _showMovingAverageFilter = true;
    bool _showHarmonicFilter = true;
    bool _showTarget = false;

    // Public functions
    public TouchAreaController(VisualElement touchAreaVE)
    {
        root = touchAreaVE;

        for (int i = 0; i < BaseFilter.MaxNumSamples; i++)
        {
            fingers[i] = root.Q("Finger" + (i + 1).ToString());
        }

        movingAverageFilterVE = root.Q("MovingAverageFilter");
        harmonicFilterVE = root.Q("HarmonicFilter");
        target = root.Q("Target");
    }

    public void ClearTouchQueue()
    {
        touchQueue.Clear();
        UpdateFilterResults();
        HideAllFingers();
        HideAllFilters();
    }

    /// <summary>
    /// Process touches and update fingers and filters.
    /// </summary>
    /// <returns>True if touchQueue has been updated.</returns>
    public bool ProcessTouches()
    {
        bool hasUpdatedTouchQueue = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase != TouchPhase.Ended)
            {
                continue;
            }

            Vector2 localTouchPosition = new Vector2(
                touch.position.x,
                Screen.height - touch.position.y);

            if (root.localBound.Contains(localTouchPosition))
            {
                touchQueue.Enqueue(localTouchPosition);

                while (touchQueue.Count > BaseFilter.MaxNumSamples)
                {
                    touchQueue.Dequeue();
                }

                hasUpdatedTouchQueue = true;
            }
        }

        if (hasUpdatedTouchQueue)
        {
            UpdateFingers();
            UpdateFilterResults();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Helper functions
    void HideAllFingers()
    {
        foreach (VisualElement finger in fingers)
        {
            finger.style.visibility = Visibility.Hidden;
        }
    }

    void UpdateFingers()
    {
        for (int i = 0; i < BaseFilter.MaxNumSamples; i++)
        {
            VisualElement finger = fingers[i];

            if (i < Math.Min(touchQueue.Count, NumSamples))
            {
                Vector2 localTouchPosition = touchQueue.ElementAt(touchQueue.Count - 1 - i);

                finger.style.visibility = Visibility.Visible;
                finger.style.left = localTouchPosition.x;
                finger.style.top = localTouchPosition.y;
                finger.style.opacity = 1.0f - (float)i / NumSamples;
            }
            else
            {
                finger.style.visibility = Visibility.Hidden;
            }
        }
    }

    void UpdateFilterResults()
    {
        if (touchQueue.Count > 0)
        {
            Vector2[] samples = touchQueue.ToArray();

            movingAverageFilter.NumSamples = NumSamples;
            movingAverageFilterResult = movingAverageFilter.Filter(samples);

            harmonicFilter.NumSamples = NumSamples;
            harmonicFilterResult = harmonicFilter.Filter(samples);
        }
        else
        {
            movingAverageFilterResult = null;
            harmonicFilterResult = null;
        }

        UpdateFilters();
    }

    void HideAllFilters()
    {
        movingAverageFilterVE.style.visibility = Visibility.Hidden;
        harmonicFilterVE.style.visibility = Visibility.Hidden;
    }

    void UpdateFilters()
    {
        if (ShowMovingAverageFilter && movingAverageFilterResult != null)
        {
            movingAverageFilterVE.style.visibility = Visibility.Visible;
            movingAverageFilterVE.style.left = movingAverageFilterResult.Value.x;
            movingAverageFilterVE.style.top = movingAverageFilterResult.Value.y;
        }
        else
        {
            movingAverageFilterVE.style.visibility = Visibility.Hidden;
        }

        if (ShowHarmonicFilter && harmonicFilterResult != null)
        {
            harmonicFilterVE.style.visibility = Visibility.Visible;
            harmonicFilterVE.style.left = harmonicFilterResult.Value.x;
            harmonicFilterVE.style.top = harmonicFilterResult.Value.y;
        }
        else
        {
            harmonicFilterVE.style.visibility = Visibility.Hidden;
        }
    }

    void UpdateTargetVisibility()
    {
        if (ShowTarget)
        {
            target.style.visibility = Visibility.Visible;
        }
        else
        {
            target.style.visibility = Visibility.Hidden;
        }
    }
}
