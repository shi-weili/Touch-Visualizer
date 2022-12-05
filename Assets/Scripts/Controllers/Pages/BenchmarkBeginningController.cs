using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchmarkBeginningController : MonoBehaviour
{
    // Actions
    public Action backButtonClicked;
    public Action<bool, int, float, float> beginButtonClicked; // Parameters: bool useMovingAverageFilter, int numSamples, float targetRadius, float timeLimit

    // Constants
    const int DefaultNumSamples = 2;
    const int DeltaNumSamples = 1;
    const int MinNumSamples = BaseFilter.MinNumSamples;
    const int MaxNumSamples = BaseFilter.MaxNumSamples;

    const int DefaultRadius = 20;
    const int DeltaRadius = 10;
    const int MinRadius = 10;
    const int MaxRadius = 50;

    const int DefaultTimeLimit = 30;
    const int DeltaTimeLimit = 30;
    const int MinTimeLimit = 30;
    const int MaxTimeLimit = 60;

    // References
    VisualElement root;

    ToggleController avgToggle;
    ToggleController harmonicToggle;

    Label numSamplesLabel;
    Button numSamplesDownButton;
    Button numSamplesUpButton;

    Label radiusLabel;
    Button radiusDownButton;
    Button radiusUpButton;

    Label timeLimitLabel;
    Button timeLimitDownButton;
    Button timeLimitUpButton;

    Button beginButton;
    Button backButton;

    // Data and states
    bool useMovingAverageFilter = true;
    int numSamples = DefaultNumSamples;
    int targetRadius = DefaultRadius;
    int timeLimit = DefaultTimeLimit;

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("BenchmarkBeginning");

        avgToggle = new ToggleController(
            root.Q<Button>("AvgToggle"),
            useMovingAverageFilter,
            onlyAllowToggleOn: true);

        harmonicToggle = new ToggleController(
            root.Q<Button>("HarmonicToggle"),
            !useMovingAverageFilter,
            onlyAllowToggleOn: true);

        numSamplesLabel = root.Q<Label>("NumSamplesLabel");
        numSamplesDownButton = root.Q<Button>("NumSamplesDownButton");
        numSamplesUpButton = root.Q<Button>("NumSamplesUpButton");

        radiusLabel = root.Q<Label>("RadiusLabel");
        radiusDownButton = root.Q<Button>("RadiusDownButton");
        radiusUpButton = root.Q<Button>("RadiusUpButton");

        timeLimitLabel = root.Q<Label>("TimeLabel");
        timeLimitDownButton = root.Q<Button>("TimeDownButton");
        timeLimitUpButton = root.Q<Button>("TimeUpButton");

        beginButton = root.Q<Button>("BeginButton");
        backButton = root.Q<Button>("BackButton");
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        useMovingAverageFilter = true;
        numSamples = DefaultNumSamples;
        targetRadius = DefaultRadius;
        timeLimit = DefaultTimeLimit;

        avgToggle.SetState(useMovingAverageFilter);
        avgToggle.SetTouchEnabled(true);
        avgToggle.clicked += OnAvgToggleClicked;

        harmonicToggle.SetState(!useMovingAverageFilter);
        harmonicToggle.SetTouchEnabled(true);
        harmonicToggle.clicked += OnHarmonicToggleClicked;

        UpdateNumSamples(DefaultNumSamples);
        numSamplesDownButton.clicked += OnNumSamplesDownButtonClicked;
        numSamplesUpButton.clicked += OnNumSamplesUpButtonClicked;

        UpdateTargetRadius(DefaultRadius);
        radiusDownButton.clicked += OnRadiusDownButtonClicked;
        radiusUpButton.clicked += OnRadiusUpButtonClicked;

        UpdateTimeLimit(DefaultTimeLimit);
        timeLimitDownButton.clicked += OnTimeLimitDownButtonClicked;
        timeLimitUpButton.clicked += OnTimeLimitUpButtonClicked;

        beginButton.clicked += OnBeginButtonClicked;
        backButton.clicked += OnBackButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        avgToggle.SetTouchEnabled(false);
        avgToggle.clicked -= OnAvgToggleClicked;

        harmonicToggle.SetTouchEnabled(false);
        harmonicToggle.clicked -= OnHarmonicToggleClicked;

        numSamplesDownButton.clicked -= OnNumSamplesDownButtonClicked;
        numSamplesUpButton.clicked -= OnNumSamplesUpButtonClicked;

        radiusDownButton.clicked -= OnRadiusDownButtonClicked;
        radiusUpButton.clicked -= OnRadiusUpButtonClicked;

        timeLimitDownButton.clicked -= OnTimeLimitDownButtonClicked;
        timeLimitUpButton.clicked -= OnTimeLimitUpButtonClicked;

        beginButton.clicked -= OnBeginButtonClicked;
        backButton.clicked -= OnBackButtonClicked;
    }

    // Helper functions
    void UpdateNumSamples(int numSamples)
    {
        this.numSamples = Math.Clamp(numSamples, MinNumSamples, MaxNumSamples);
        numSamplesLabel.text = this.numSamples.ToString();
    }

    void UpdateTargetRadius(int radius)
    {
        targetRadius = Math.Clamp(radius, MinRadius, MaxRadius);
        radiusLabel.text = targetRadius.ToString() + "px";
    }

    void UpdateTimeLimit(int timeLimit)
    {
        this.timeLimit = Math.Clamp(timeLimit, MinTimeLimit, MaxTimeLimit);
        timeLimitLabel.text = this.timeLimit.ToString() + "s";
    }

    // Handlers
    void OnAvgToggleClicked(bool selected)
    {
        useMovingAverageFilter = selected;
        harmonicToggle.SetState(!useMovingAverageFilter);
    }

    void OnHarmonicToggleClicked(bool selected)
    {
        useMovingAverageFilter = !selected;
        avgToggle.SetState(useMovingAverageFilter);
    }

    void OnNumSamplesDownButtonClicked()
    {
        UpdateNumSamples(numSamples - DeltaNumSamples);
    }

    void OnNumSamplesUpButtonClicked()
    {
        UpdateNumSamples(numSamples + DeltaNumSamples);
    }

    void OnRadiusDownButtonClicked()
    {
        UpdateTargetRadius(targetRadius - DeltaRadius);
    }

    void OnRadiusUpButtonClicked()
    {
        UpdateTargetRadius(targetRadius + DeltaRadius);
    }

    void OnTimeLimitDownButtonClicked()
    {
        UpdateTimeLimit(timeLimit - DeltaTimeLimit);
    }

    void OnTimeLimitUpButtonClicked()
    {
        UpdateTimeLimit(timeLimit + DeltaTimeLimit);
    }

    void OnBeginButtonClicked()
    {
        beginButtonClicked?.Invoke(useMovingAverageFilter, numSamples, targetRadius, timeLimit);
    }

    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();
    }
}
