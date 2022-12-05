using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BenchmarkEndController : MonoBehaviour
{
    // Actions
    public Action finishButtonClicked;

    // References
    VisualElement root;

    Label pointsLabel;
    Label tapsLabel;
    Label tapsPerPointLabel;

    Button saveLogsButton;
    Button finishButton;

    LogManager logManager;

    // Data and states

    // Life cycle
    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("BenchmarkEnd");

        pointsLabel = root.Q<Label>("PointsLabel");
        tapsLabel = root.Q<Label>("TapsLabel");
        tapsPerPointLabel = root.Q<Label>("TapsPerPointLabel");

        saveLogsButton = root.Q<Button>("SaveLogsButton");
        finishButton = root.Q<Button>("FinishButton");

        logManager = FindObjectOfType<LogManager>();
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;

        saveLogsButton.clicked += OnSaveLogsButtonClicked;
        finishButton.clicked += OnFinishButtonClicked;

        logManager.SaveToPersistentDataPath("TouchVisualizer_BenchmarkLog_" + DateTime.Now.ToFileTimeUtc().ToString() + ".csv");
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;

        saveLogsButton.clicked -= OnSaveLogsButtonClicked;
        finishButton.clicked -= OnFinishButtonClicked;
    }

    // Public functions
    public void SetValues(int points, int taps)
    {
        pointsLabel.text = points.ToString();
        tapsLabel.text = taps.ToString();

        if (points > 0)
        {
            tapsPerPointLabel.text = String.Format("{0:0.00}", (float)taps / points);
        }
        else
        {
            tapsPerPointLabel.text = "N/A";
        }
    }

    // Helper functions

    // Handlers
    void OnSaveLogsButtonClicked()
    {
        logManager.ShareLogFile();
    }

    void OnFinishButtonClicked()
    {
        finishButtonClicked?.Invoke();
    }
}
