using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System;
using System.Linq;

public class TouchFiltersController : MonoBehaviour
{
    public Action backButtonClicked;

    VisualElement root;
    Label resultsLabel;
    Button downButton;
    Button upButton;
    Button avgToggle;
    Button harmonicToggle;
    Button targetToggle;
    Button clearButton;
    Button backButton;
    Label numSamplesLabel;
    VisualElement fingerContainer;
    VisualElement[] fingers = new VisualElement[BaseFilter.MaxNumSamples];
    VisualElement movingAverageFilterVE;
    VisualElement harmonicFilterVE;
    VisualElement target;

    BaseFilter movingAverageFilter = new MovingAverageFilter();
    BaseFilter harmonicFilter = new HarmonicFilter();

    bool showMovingAverageFilter = true;
    bool showHarmonicFilter = true;
    bool showTarget = false;

    int numSamples = 2;
    Queue<Vector2> touchQueue = new Queue<Vector2>();
    Vector2 movingAverageFilterResult;
    Vector2 harmonicFilterResult;

    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("TouchFilters");
        resultsLabel = root.Q<Label>("ResultsLabel");
        downButton = root.Q<Button>("DownButton");
        upButton = root.Q<Button>("UpButton");
        avgToggle = root.Q<Button>("AvgToggle");
        harmonicToggle = root.Q<Button>("HarmonicToggle");
        targetToggle = root.Q<Button>("TargetToggle");
        clearButton = root.Q<Button>("ClearButton");
        backButton = root.Q<Button>("BackButton");
        numSamplesLabel = root.Q<Label>("NumSamplesLabel");
        fingerContainer = root.Q("FingerContainer");

        for (int i = 0; i < BaseFilter.MaxNumSamples; i++)
        {
            fingers[i] = root.Q("Finger" + (i + 1).ToString());
        }

        movingAverageFilterVE = root.Q("MovingAverageFilter");
        harmonicFilterVE = root.Q("HarmonicFilter");
        target = root.Q("Target");
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;
        HideAllFingers();
        HideAllFilters();
        HideTarget();
        showMovingAverageFilter = true;
        showHarmonicFilter = true;
        showTarget = false;
        UpdateToggles();
        ClearResults();
        movingAverageFilterVE.style.visibility = Visibility.Hidden;
        harmonicFilterVE.style.visibility = Visibility.Hidden;

        downButton.clicked += OnDownButtonClicked;
        upButton.clicked += OnUpButtonClicked;
        avgToggle.clicked += OnAvgToggleClicked;
        harmonicToggle.clicked += OnHarmonicToggleClicked;
        targetToggle.clicked += OnTargetToggleClicked;
        clearButton.clicked += OnClearButtonClicked;
        backButton.clicked += OnBackButtonClicked;

        resetNumSamples();
        movingAverageFilter.NumSamples = numSamples;
        harmonicFilter.NumSamples = numSamples;
        touchQueue.Clear();
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        downButton.clicked -= OnDownButtonClicked;
        upButton.clicked -= OnUpButtonClicked;
        avgToggle.clicked -= OnAvgToggleClicked;
        harmonicToggle.clicked -= OnHarmonicToggleClicked;
        targetToggle.clicked -= OnTargetToggleClicked;
        clearButton.clicked -= OnClearButtonClicked;
        backButton.clicked -= OnBackButtonClicked;
    }

    void Update()
    {
        ProcessTouches();
    }

    void ProcessTouches()
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

            if (fingerContainer.localBound.Contains(localTouchPosition))
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
            UpdateFilters();
        }
    }

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

            if (i < Math.Min(touchQueue.Count, numSamples))
            {
                Vector2 localTouchPosition = touchQueue.ElementAt(touchQueue.Count - 1 - i);

                finger.style.visibility = Visibility.Visible;
                finger.style.left = localTouchPosition.x;
                finger.style.top = localTouchPosition.y;
                finger.style.opacity = 1.0f - (float)i / numSamples;
            }
            else
            {
                finger.style.visibility = Visibility.Hidden;
            }
        }
    }

    void HideAllFilters()
    {
        movingAverageFilterVE.style.visibility = Visibility.Hidden;
        harmonicFilterVE.style.visibility = Visibility.Hidden;
    }

    void UpdateFilters()
    {
        movingAverageFilter.NumSamples = numSamples;
        harmonicFilter.NumSamples = numSamples;

        if ((showMovingAverageFilter || showHarmonicFilter) && touchQueue.Count > 0)
        {
            Vector2[] samples = touchQueue.ToArray();

            if (showMovingAverageFilter)
            {
                movingAverageFilterResult = movingAverageFilter.Filter(samples);
                movingAverageFilterVE.style.visibility = Visibility.Visible;
                movingAverageFilterVE.style.left = movingAverageFilterResult.x;
                movingAverageFilterVE.style.top = movingAverageFilterResult.y;
            }

            if (showHarmonicFilter)
            {
                harmonicFilterResult = harmonicFilter.Filter(samples);
                harmonicFilterVE.style.visibility = Visibility.Visible;
                harmonicFilterVE.style.left = harmonicFilterResult.x;
                harmonicFilterVE.style.top = harmonicFilterResult.y;
            }

            UpdateResults();
        }
        else
        {
            ClearResults();
        }

        if (!showMovingAverageFilter)
        {
            movingAverageFilterVE.style.visibility = Visibility.Hidden;
        }

        if (!showHarmonicFilter)
        {
            harmonicFilterVE.style.visibility = Visibility.Hidden;
        }
    }

    void UpdateResults()
    {
        if (!showMovingAverageFilter || !showHarmonicFilter || !showTarget)
        {
            ClearResults();
        }
        else
        {
            float movingAverageFilterOffset = Vector2.Distance(
                movingAverageFilterResult,
                target.layout.center);

            float harmonicFilterOffset = Vector2.Distance(
                harmonicFilterResult,
                target.layout.center);

            if (movingAverageFilterOffset < harmonicFilterOffset)
            {
                resultsLabel.text = String.Format(
                    "<b><color=red>Moving Avg: {0:0.00}px off</color></b>  <color=blue>Harmonic: {1:0.00} px off</color>", movingAverageFilterOffset, harmonicFilterOffset);
            }
            else if (movingAverageFilterOffset > harmonicFilterOffset)
            {
                resultsLabel.text = String.Format(
                    "<color=red>Moving Avg: {0:0.00}px off</color>  <b><color=blue>Harmonic: {1:0.00} px off</color></b>", movingAverageFilterOffset, harmonicFilterOffset);
            } else
            {
                resultsLabel.text = String.Format(
                    "<color=red>Moving Avg: {0:0.00}px off</color>  <color=blue>Harmonic: {1:0.00} px off</color>", movingAverageFilterOffset, harmonicFilterOffset);
            }
        }

    }

    void ClearResults()
    {
        resultsLabel.text = "";
    }

    void HideTarget()
    {
        target.style.visibility = Visibility.Hidden;
    }

    void resetNumSamples()
    {
        numSamples = 2;
        numSamplesLabel.text = numSamples.ToString();
    }    

    void UpdateNumSamples(int delta)
    {
        numSamples = Math.Clamp(numSamples + delta, 1, BaseFilter.MaxNumSamples);
        numSamplesLabel.text = numSamples.ToString();
        UpdateFingers();
        UpdateFilters();
    }

    void OnDownButtonClicked()
    {
        UpdateNumSamples(-1);
    }

    void OnUpButtonClicked()
    {
        UpdateNumSamples(1);
    }

    void OnAvgToggleClicked()
    {
        showMovingAverageFilter = !showMovingAverageFilter;
        UpdateToggles();
        UpdateFilters();
    }

    void OnHarmonicToggleClicked()
    {
        showHarmonicFilter = !showHarmonicFilter;
        UpdateToggles();
        UpdateFilters();
    }

    void OnTargetToggleClicked()
    {
        showTarget = !showTarget;
        UpdateToggles();

        if (showTarget)
        {
            target.style.visibility = Visibility.Visible;
        }
        else
        {
            target.style.visibility = Visibility.Hidden;
        }

        UpdateFilters();
    }

    void UpdateToggles()
    {
        if (showMovingAverageFilter)
        {
            avgToggle.AddToClassList("selected");
        }
        else
        {
            avgToggle.RemoveFromClassList("selected");
        }

        if (showHarmonicFilter)
        {
            harmonicToggle.AddToClassList("selected");
        }
        else
        {
            harmonicToggle.RemoveFromClassList("selected");
        }

        if (showTarget)
        {
            targetToggle.AddToClassList("selected");
        }
        else
        {
            targetToggle.RemoveFromClassList("selected");
        }
    }

    void OnClearButtonClicked()
    {
        touchQueue.Clear();
        HideAllFingers();
        HideAllFilters();
        ClearResults();
    }

    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();
    }
}
