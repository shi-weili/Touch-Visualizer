using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchmarkGameController : MonoBehaviour
{
    // Actions
    public Action backButtonClicked;
    public Action<int, int> timeUp; // Parameter: int points, int taps

    // References
    VisualElement root;
    TouchAreaController touchArea;
    Label pointsValueLabel;
    Label timeValueLabel;
    Button backButton;

    LogManager logManager;

    // Data and states
    bool useMovingAverageFilter;
    int numSamples;
    float targetRadius;

    bool hitTarget = false;
    int points = 0;
    int taps = 0;
    float startTime = 0;
    float timeLimit = 30;

    float RemainingTime
    {
        get => Math.Clamp(timeLimit - (Time.time - startTime), 0.0f, timeLimit);
    }

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("BenchmarkGame");

        touchArea = new TouchAreaController(root.Q("TouchArea"));

        pointsValueLabel = root.Q<Label>("PointsValueLabel");
        timeValueLabel = root.Q<Label>("TimeValueLabel");

        backButton = root.Q<Button>("BackButton");

        logManager = FindObjectOfType<LogManager>();
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        touchArea.ClearTouchQueue();

        touchArea.ShowMovingAverageFilter = useMovingAverageFilter;
        touchArea.ShowHarmonicFilter = !useMovingAverageFilter;
        touchArea.ShowTarget = true;
        touchArea.ShowTargetRadius = true;
        touchArea.NumSamples = numSamples;
        touchArea.TargetRadius = targetRadius;

        points = 0;
        UpdatePointsValueLabel();

        taps = 0;

        startTime = Time.time;
        UpdateTimeValueLabel();

        backButton.clicked += OnBackButtonClicked;

        StartNewLog();
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        backButton.clicked -= OnBackButtonClicked;
    }

    void Update()
    {
        if (RemainingTime <= 0)
        {
            timeUp?.Invoke(points, taps);
        }
        else
        {
            UpdateTimeValueLabel();

            if (touchArea.ProcessTouches())
            {
                UpdatePoints();
            }
        }
    }

    // Public functions
    public void SetParams(bool useMovingAverageFilter, int numSamples, float targetRadius, float timeLimit)
    {
        this.useMovingAverageFilter = useMovingAverageFilter;
        this.numSamples = numSamples;
        this.targetRadius = targetRadius;
        this.timeLimit = timeLimit;
    }

    // Helper functions
    void UpdatePoints()
    {
        Vector2 filterResult = useMovingAverageFilter ?
                touchArea.MovingAverageFilterResult.Value
                : touchArea.HarmonicFilterResult.Value;

        float offset = Vector2.Distance(
            filterResult,
            touchArea.TargetPositionInPixel);

        hitTarget = offset <= targetRadius;

        if (hitTarget)
        {
            points += 1;
            UpdatePointsValueLabel();
            AddNewTouchesToLog();
            touchArea.ClearTouchQueue();
            touchArea.SetRandomTargetPosition();
        }
        else
        {
            AddNewTouchesToLog();
        }
    }

    void UpdatePointsValueLabel()
    {
        pointsValueLabel.text = points.ToString();
    }

    void UpdateTimeValueLabel()
    {
        timeValueLabel.text = String.Format("{0:0}", RemainingTime);
    }

    void StartNewLog()
    {
        logManager.StartNewLog();

        List<string> logItems = new List<string>();

        logItems.Add("tap_index");
        logItems.Add("use_moving_average_filter");
        logItems.Add("num_samples");
        logItems.Add("target_radius");
        logItems.Add("time_limit");
        logItems.Add("remaining_time");
        logItems.Add("frame_index");
        logItems.Add("touch_position_x");
        logItems.Add("touch_position_y");
        logItems.Add("moving_average_filter_result_x");
        logItems.Add("moving_average_filter_result_y");
        logItems.Add("harmonic_filter_result_x");
        logItems.Add("harmonic_filter_result_y");
        logItems.Add("moving_average_filter_result_offset");
        logItems.Add("harmonic_filter_result_offset");
        logItems.Add("hit_target");
        logItems.Add("points");

        logManager.AddToLog(String.Join(",", logItems));
    }

    void AddNewTouchesToLog()
    {
        foreach (Vector2 touchPositon in touchArea.NewlyProcessedTouches)
        {
            List<string> logItems = new List<string>();

            logItems.Add((taps++).ToString());
            logItems.Add(useMovingAverageFilter ? "YES" : "NO");
            logItems.Add(numSamples.ToString());
            logItems.Add(targetRadius.ToString());
            logItems.Add(timeLimit.ToString());
            logItems.Add(RemainingTime.ToString());
            logItems.Add(Time.frameCount.ToString());
            logItems.Add(touchPositon.x.ToString());
            logItems.Add(touchPositon.y.ToString());
            logItems.Add(touchArea.MovingAverageFilterResult.Value.x.ToString());
            logItems.Add(touchArea.MovingAverageFilterResult.Value.y.ToString());
            logItems.Add(touchArea.HarmonicFilterResult.Value.x.ToString());
            logItems.Add(touchArea.HarmonicFilterResult.Value.y.ToString());
            logItems.Add(Vector2.Distance(
                touchArea.MovingAverageFilterResult.Value,
                touchArea.TargetPositionInPixel).ToString());
            logItems.Add(Vector2.Distance(
                touchArea.HarmonicFilterResult.Value,
                touchArea.TargetPositionInPixel).ToString());
            logItems.Add(hitTarget ? "YES" : "NO");
            logItems.Add(points.ToString());

            logManager.AddToLog(String.Join(",", logItems));
        }
    }

    // Handlers
    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();
    }
}
