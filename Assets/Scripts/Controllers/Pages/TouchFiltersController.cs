using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TouchFiltersController : MonoBehaviour
{
    // Actions
    public Action backButtonClicked;

    // References
    VisualElement root;

    TouchAreaController touchArea;

    Label resultsLabel;

    ToggleController avgToggle;
    ToggleController harmonicToggle;
    ToggleController targetToggle;

    Label numSamplesLabel;
    Button downButton;
    Button upButton;

    Button clearButton;
    Button backButton;

    // Data and states    

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("TouchFilters");

        touchArea = new TouchAreaController(root.Q("TouchArea"));

        resultsLabel = root.Q<Label>("ResultsLabel");

        avgToggle = new ToggleController(
            root.Q<Button>("AvgToggle"),
            touchArea. ShowMovingAverageFilter);

        harmonicToggle = new ToggleController(
            root.Q<Button>("HarmonicToggle"),
            touchArea.ShowHarmonicFilter);

        targetToggle = new ToggleController(
            root.Q<Button>("TargetToggle"),
            touchArea.ShowTarget);

        numSamplesLabel = root.Q<Label>("NumSamplesLabel");
        downButton = root.Q<Button>("DownButton");
        upButton = root.Q<Button>("UpButton");

        clearButton = root.Q<Button>("ClearButton");
        backButton = root.Q<Button>("BackButton");
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        touchArea.ClearTouchQueue();

        touchArea.ShowMovingAverageFilter = true;
        avgToggle.SetState(touchArea.ShowMovingAverageFilter);
        avgToggle.SetTouchEnabled(true);
        avgToggle.clicked += OnAvgToggleClicked;

        touchArea.ShowHarmonicFilter = true;
        harmonicToggle.SetState(touchArea.ShowHarmonicFilter);
        harmonicToggle.SetTouchEnabled(true);
        harmonicToggle.clicked += OnHarmonicToggleClicked;

        touchArea.ShowTarget = false;
        targetToggle.SetState(touchArea.ShowTarget);
        targetToggle.SetTouchEnabled(true);
        targetToggle.clicked += OnTargetToggleClicked;

        ClearResults();

        ResetNumSamples();

        downButton.clicked += OnDownButtonClicked;
        upButton.clicked += OnUpButtonClicked;

        clearButton.clicked += OnClearButtonClicked;
        backButton.clicked += OnBackButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        avgToggle.SetTouchEnabled(false);
        avgToggle.clicked -= OnAvgToggleClicked;

        harmonicToggle.SetTouchEnabled(false);
        harmonicToggle.clicked -= OnHarmonicToggleClicked;

        targetToggle.SetTouchEnabled(false);
        targetToggle.clicked -= OnTargetToggleClicked;

        downButton.clicked -= OnDownButtonClicked;
        upButton.clicked -= OnUpButtonClicked;

        clearButton.clicked -= OnClearButtonClicked;
        backButton.clicked -= OnBackButtonClicked;
    }

    void Update()
    {
        if (touchArea.ProcessTouches())
        {
            UpdateResults();
        }
    }

    // Helper functions

    void ClearResults()
    {
        resultsLabel.text = "";
    }

    void UpdateResults()
    {
        if ((!touchArea.ShowMovingAverageFilter
            && !touchArea.ShowHarmonicFilter)
            || !touchArea.ShowTarget
            || touchArea.MovingAverageFilterResult == null
            || touchArea.HarmonicFilterResult == null)
        {
            ClearResults();
        }
        else
        {
            float movingAverageFilterOffset = Vector2.Distance(
                touchArea.MovingAverageFilterResult.Value,
                touchArea.TargetPosition);

            float harmonicFilterOffset = Vector2.Distance(
                touchArea.HarmonicFilterResult.Value,
                touchArea.TargetPosition);

            if (touchArea.ShowMovingAverageFilter && touchArea.ShowHarmonicFilter)
            {
                if (movingAverageFilterOffset < harmonicFilterOffset)
                {
                    resultsLabel.text = String.Format(
                        "<b><color=red>Moving Avg: {0:0.00}px off</color></b>  <color=blue>Harmonic: {1:0.00} px off</color>", movingAverageFilterOffset, harmonicFilterOffset);
                }
                else if (movingAverageFilterOffset > harmonicFilterOffset)
                {
                    resultsLabel.text = String.Format(
                        "<color=red>Moving Avg: {0:0.00}px off</color>  <b><color=blue>Harmonic: {1:0.00} px off</color></b>", movingAverageFilterOffset, harmonicFilterOffset);
                }
                else
                {
                    resultsLabel.text = String.Format(
                        "<color=red>Moving Avg: {0:0.00}px off</color>  <color=blue>Harmonic: {1:0.00} px off</color>", movingAverageFilterOffset, harmonicFilterOffset);
                }
            }
            else if (touchArea.ShowMovingAverageFilter)
            {
                resultsLabel.text = String.Format(
                        "<color=red>Moving Avg: {0:0.00}px off</color>", movingAverageFilterOffset);
            }
            else
            {
                resultsLabel.text = String.Format(
                        "<color=blue>Harmonic: {0:0.00} px off</color>", harmonicFilterOffset);
            }
        }

    }

    void ResetNumSamples()
    {
        touchArea.NumSamples = 2;
        numSamplesLabel.text = touchArea.NumSamples.ToString();
    }    

    void UpdateNumSamples(int delta)
    {
        touchArea.NumSamples = touchArea.NumSamples + delta;
        numSamplesLabel.text = touchArea.NumSamples.ToString();
        UpdateResults();
    }

    // Handlers
    void OnDownButtonClicked()
    {
        UpdateNumSamples(-1);
    }

    void OnUpButtonClicked()
    {
        UpdateNumSamples(1);
    }

    void OnAvgToggleClicked(bool selected)
    {
        touchArea.ShowMovingAverageFilter = selected;
        UpdateResults();
    }

    void OnHarmonicToggleClicked(bool selected)
    {
        touchArea.ShowHarmonicFilter = selected;
        UpdateResults();
    }

    void OnTargetToggleClicked(bool selected)
    {
        touchArea.ShowTarget = selected;
        UpdateResults();
    }

    void OnClearButtonClicked()
    {
        touchArea.ClearTouchQueue();
        ClearResults();
    }

    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();
    }
}
