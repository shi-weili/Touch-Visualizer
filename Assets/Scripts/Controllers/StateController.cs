using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    // References
    MasterMenuController masterMenuController;
    BasicVisualizerController basicVisualizerController;
    TouchFiltersController touchFiltersController;

    // Data and states
    enum State
    {
        MasterMenu,
        BasicVisualizer,
        TouchFilters
    }

    State state = State.MasterMenu;

    // Life cycle
    void Awake()
    {
        masterMenuController = GetComponentInChildren<MasterMenuController>();
        basicVisualizerController = GetComponentInChildren<BasicVisualizerController>();
        touchFiltersController = GetComponentInChildren<TouchFiltersController>();
    }

    void Start()
    {
        SetState(State.MasterMenu, force: true);
    }

    void OnEnable()
    {
        masterMenuController.basicVisualizerButtonClicked += OnBasicVisualizerButtonClicked;
        masterMenuController.touchFiltersButtonClicked += OnTouchFiltersButtonClicked;
        basicVisualizerController.backButtonClicked += OnBackButtonClicked;
        touchFiltersController.backButtonClicked += OnBackButtonClicked;
    }

    void OnDisable()
    {
        masterMenuController.basicVisualizerButtonClicked -= OnBasicVisualizerButtonClicked;
        masterMenuController.touchFiltersButtonClicked -= OnTouchFiltersButtonClicked;
        basicVisualizerController.backButtonClicked -= OnBackButtonClicked;
        touchFiltersController.backButtonClicked -= OnBackButtonClicked;
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
                    break;
                }
            case State.BasicVisualizer:
                {
                    masterMenuController.enabled = false;
                    basicVisualizerController.enabled = true;
                    touchFiltersController.enabled = false;
                    break;
                }
            case State.TouchFilters:
                {
                    masterMenuController.enabled = false;
                    basicVisualizerController.enabled = false;
                    touchFiltersController.enabled = true;
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

    void OnBackButtonClicked()
    {
        SetState(State.MasterMenu);
    }
}
