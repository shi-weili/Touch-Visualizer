using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MasterMenuController : MonoBehaviour
{
    // Actions
    public Action basicVisualizerButtonClicked;
    public Action touchFiltersButtonClicked;
    public Action benchmarkButtonClicked;

    // References
    VisualElement root;
    Button basicVisualizerButton;
    Button touchFiltersButton;
    Button benchmarkButton;

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("MasterMenu");
        basicVisualizerButton = root.Q<Button>("BasicVisualizerButton");
        touchFiltersButton = root.Q<Button>("TouchFiltersButton");
        benchmarkButton = root.Q<Button>("BenchmarkButton");
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        basicVisualizerButton.clicked += OnBasicVisualizerButtonClicked;
        touchFiltersButton.clicked += OnTouchFiltersButtonClicked;
        benchmarkButton.clicked += OnBenchmarkButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        basicVisualizerButton.clicked -= OnBasicVisualizerButtonClicked;
        touchFiltersButton.clicked -= OnTouchFiltersButtonClicked;
        benchmarkButton.clicked -= OnBenchmarkButtonClicked;
    }

    // Handlers
    void OnBasicVisualizerButtonClicked()
    {
        basicVisualizerButtonClicked?.Invoke();
    }

    void OnTouchFiltersButtonClicked()
    {
        touchFiltersButtonClicked?.Invoke();
    }

    void OnBenchmarkButtonClicked()
    {
        benchmarkButtonClicked?.Invoke();
    }
}
