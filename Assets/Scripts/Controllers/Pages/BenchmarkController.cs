using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchmarkController : MonoBehaviour
{
    // Actions
    public Action backButtonClicked;

    // References
    VisualElement root;

    BenchmarkBeginningController benchmarkBeginningController;
    BenchmarkGameController benchmarkGameController;
    BenchmarkEndController benchmarkEndController;

    // Data and states

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("BenchmarkMaster");

        benchmarkBeginningController = GetComponentInChildren<BenchmarkBeginningController>();
        benchmarkGameController = GetComponentInChildren<BenchmarkGameController>();
        benchmarkEndController = GetComponentInChildren<BenchmarkEndController>();
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        benchmarkBeginningController.enabled = true;
        benchmarkGameController.enabled = false;
        benchmarkEndController.enabled = false;

        benchmarkBeginningController.beginButtonClicked += OnBeginButtonClicked;
        benchmarkBeginningController.backButtonClicked += OnBackButtonClicked;
        benchmarkGameController.backButtonClicked += OnGameBackButtonClicked;
        benchmarkGameController.timeUp += OnTimeUp;
        benchmarkEndController.finishButtonClicked += OnFinishButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        benchmarkBeginningController.enabled = false;
        benchmarkGameController.enabled = false;
        benchmarkEndController.enabled = false;

        benchmarkBeginningController.beginButtonClicked -= OnBeginButtonClicked;
        benchmarkBeginningController.backButtonClicked -= OnBackButtonClicked;
        benchmarkGameController.backButtonClicked -= OnGameBackButtonClicked;
        benchmarkGameController.timeUp -= OnTimeUp;
        benchmarkEndController.finishButtonClicked -= OnFinishButtonClicked;
    }

    // Helper functions

    // Handlers
    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();
    }

    void OnBeginButtonClicked(bool useMovingAverageFilter, int numSamples, float targetRadius, float timeLimit)
    {
        benchmarkBeginningController.enabled = false;
        benchmarkGameController.SetParams(useMovingAverageFilter, numSamples, targetRadius, timeLimit);
        benchmarkGameController.enabled = true;
    }

    void OnGameBackButtonClicked()
    {
        benchmarkBeginningController.enabled = true;
        benchmarkGameController.enabled = false;
    }

    void OnTimeUp(int points, int taps)
    {
        benchmarkGameController.enabled = false;
        benchmarkEndController.SetValues(points, taps);
        benchmarkEndController.enabled = true;
    }

    void OnFinishButtonClicked()
    {
        benchmarkBeginningController.enabled = true;
        benchmarkEndController.enabled = false;
    }
}
