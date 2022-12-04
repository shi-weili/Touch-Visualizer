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

    // References
    VisualElement root;
    Button basicVisualizerButton;
    Button touchFiltersButton;

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("MasterMenu");
        basicVisualizerButton = root.Q<Button>("BasicVisualizerButton");
        touchFiltersButton = root.Q<Button>("TouchFiltersButton");
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;
        basicVisualizerButton.clicked += OnBasicVisualizerButtonClicked;
        touchFiltersButton.clicked += OnTouchFiltersButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;
        basicVisualizerButton.clicked -= OnBasicVisualizerButtonClicked;
        touchFiltersButton.clicked -= OnTouchFiltersButtonClicked;
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
}
