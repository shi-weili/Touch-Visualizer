using System.Collections;
using System.Collections.Generic;
using Keiwando.NFSO;
using Keiwando.NFSO.Samples;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MasterMenuController : MonoBehaviour
{
    public Action basicVisualizerButtonClicked;
    public Action touchFiltersButtonClicked;

    VisualElement root;
    Button basicVisualizerButton;
    Button touchFiltersButton;

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

    void OnBasicVisualizerButtonClicked()
    {
        basicVisualizerButtonClicked?.Invoke();
    }

    void OnTouchFiltersButtonClicked()
    {
        touchFiltersButtonClicked?.Invoke();
    }
}
