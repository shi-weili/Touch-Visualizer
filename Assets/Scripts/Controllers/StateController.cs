using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    // References
    MasterMenuController masterMenuController;
    BasicVisualizerController basicVisualizerController;
    TouchFiltersController touchFiltersController;
    BenchmarkController benchmarkController;

    // Data and states
    enum State
    {
        MasterMenu,
        BasicVisualizer,
        TouchFilters,
        Benchmark
    }

    State state = State.MasterMenu;

    // Life cycle
    void Awake()
    {
        masterMenuController = GetComponentInChildren<MasterMenuController>();
        basicVisualizerController = GetComponentInChildren<BasicVisualizerController>();
        touchFiltersController = GetComponentInChildren<TouchFiltersController>();
        benchmarkController = GetComponentInChildren<BenchmarkController>();
    }

    void Start()
    {
        SetState(State.MasterMenu, force: true);
    }

    void OnEnable()
    {
        masterMenuController.basicVisualizerButtonClicked += OnBasicVisualizerButtonClicked;
        masterMenuController.touchFiltersButtonClicked += OnTouchFiltersButtonClicked;
        masterMenuController.benchmarkButtonClicked += OnBenchmarkButtonClicked;
        basicVisualizerController.backButtonClicked += OnBackButtonClicked;
        touchFiltersController.backButtonClicked += OnBackButtonClicked;
        benchmarkController.backButtonClicked += OnBackButtonClicked;
    }

    void OnDisable()
    {
        masterMenuController.basicVisualizerButtonClicked -= OnBasicVisualizerButtonClicked;
        masterMenuController.touchFiltersButtonClicked -= OnTouchFiltersButtonClicked;
        masterMenuController.benchmarkButtonClicked -= OnBenchmarkButtonClicked;
        basicVisualizerController.backButtonClicked -= OnBackButtonClicked;
        touchFiltersController.backButtonClicked -= OnBackButtonClicked;
        benchmarkController.backButtonClicked -= OnBackButtonClicked;
    }

    // Helper functions
    void SetState(State newState, bool force = false)
    {
        if (newState == state && !force)
        {
            return;
        }

        switch (newState)
        {
            case State.MasterMenu:
                {
                    masterMenuController.enabled = true;
                    basicVisualizerController.enabled = false;
                    touchFiltersController.enabled = false;
                    benchmarkController.enabled = false;

                    break;
                }
            case State.BasicVisualizer:
                {
                    masterMenuController.enabled = false;
                    basicVisualizerController.enabled = true;
                    touchFiltersController.enabled = false;
                    benchmarkController.enabled = false;

                    break;
                }
            case State.TouchFilters:
                {
                    masterMenuController.enabled = false;
                    basicVisualizerController.enabled = false;
                    touchFiltersController.enabled = true;
                    benchmarkController.enabled = false;

                    break;
                }
            case State.Benchmark:
                {
                    masterMenuController.enabled = false;
                    basicVisualizerController.enabled = false;
                    touchFiltersController.enabled = false;
                    benchmarkController.enabled = true;

                    break;
                }
        }

        state = newState;
    }

    // Handlers
    void OnBasicVisualizerButtonClicked()
    {
        SetState(State.BasicVisualizer);
    }

    void OnTouchFiltersButtonClicked()
    {
        SetState(State.TouchFilters);
    }

    void OnBenchmarkButtonClicked()
    {
        SetState(State.Benchmark);
    }

    void OnBackButtonClicked()
    {
        SetState(State.MasterMenu);
    }
}
