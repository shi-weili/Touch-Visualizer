using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Keiwando.NFSO;
using Keiwando.NFSO.Samples;
using System.IO;
using System;

public class BasicVisualizerController : MonoBehaviour
{
    public Action backButtonClicked;

    const int MaxMultiTouches = 5;

    VisualElement root;
    Button backButton;
    VisualElement fingerContainer;
    VisualElement[] fingers = new VisualElement[MaxMultiTouches];

    bool hadTouchInLastFrame = false;
    Dictionary<int, int> touchToFinger = new Dictionary<int, int>(); // Map Touch.fingerId to finger

    void Awake()
    {
        root = FindObjectOfType<UIDocument>().rootVisualElement.Q("BasicVisualizer");
        backButton = root.Q<Button>("BackButton");
        fingerContainer = root.Q("FingerContainer");

        for (int i = 1; i <= MaxMultiTouches; i++)
        {
            fingers[i - 1] = root.Q("Finger" + i.ToString());
        }
    }

    void OnEnable()
    {
        root.style.display = DisplayStyle.Flex;
        HideAllFingers();
        backButton.clicked += OnBackButtonClicked;
    }

    void OnDisable()
    {
        root.style.display = DisplayStyle.None;
        backButton.clicked -= OnBackButtonClicked;
    }

    void Update()
    {
        ProcessTouches();
    }

    void ProcessTouches()
    {
        Boolean hasTouchInCurrentFrame = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Canceled)
            {
                continue;
            }

            Vector2 localTouchPosition = new Vector2(
                touch.position.x,
                Screen.height - touch.position.y);

            if (fingerContainer.localBound.Contains(localTouchPosition))
            {
                if (!touchToFinger.ContainsKey(touch.fingerId)
                    && touchToFinger.Count < MaxMultiTouches)
                {
                    touchToFinger[touch.fingerId] = touchToFinger.Count;
                }

                if (touchToFinger.ContainsKey(touch.fingerId))
                {
                    hasTouchInCurrentFrame = true;

                    // If this is a new group of touches, reset finger states.
                    if (!hadTouchInLastFrame)
                    {
                        HideAllFingers();
                        hadTouchInLastFrame = true;
                    }

                    VisualElement finger = fingers[touchToFinger[touch.fingerId]];
                    finger.style.visibility = Visibility.Visible;
                    finger.style.left = localTouchPosition.x;
                    finger.style.top = localTouchPosition.y;
                }
            }
        }

        // When we lose all touches in a group, clear finger mapping.
        if (!hasTouchInCurrentFrame)
        {
            touchToFinger.Clear();
            hadTouchInLastFrame = false;
        }
    }

    void HideAllFingers()
    {
        foreach (VisualElement finger in fingers)
        {
            finger.style.visibility = Visibility.Hidden;
        }
    }

    void OnBackButtonClicked()
    {
        backButtonClicked?.Invoke();

        //FileWriter.WriteTestFile(Application.persistentDataPath);
        //NativeFileSO.shared.SaveFile(GetFileToSave());
    }

    FileToSave GetFileToSave()
    {
        var testFilePath = Path.Combine(Application.persistentDataPath, "NativeFileSOTest.txt");
        return new FileToSave(testFilePath, "NativeFileSOTest.txt", SupportedFileType.PlainText);
    }
}
