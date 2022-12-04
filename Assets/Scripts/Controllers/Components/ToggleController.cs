using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleController
{
    // Actions
    public Action<bool> clicked;

    // References
    Button toggle;

    // Data and states
    bool isSelected;
    bool isTouchEnabled;

    // Public functions

    /// <summary>
    /// Create a ToogleController instance using a toggle UI reference and a initial state.
    /// </summary>
    /// <param name="toggle">Of UIElements.Button type. Has to be a toggle.</param>
    /// <param name="initialState"></param>
    public ToggleController(Button toggle, bool selected, bool enableTouch = false)
    {
        this.toggle = toggle;
        SetState(selected);
        SetTouchEnabled(enableTouch);
    }

    public void SetState(bool selected)
    {
        isSelected = selected;

        if (isSelected)
        {
            toggle.AddToClassList("selected");
        }
        else
        {
            toggle.RemoveFromClassList("selected");
        }
    }

    public void SetTouchEnabled(bool enableTouch)
    {
        isTouchEnabled = enableTouch;

        if (isTouchEnabled)
        {
            toggle.clicked += OnClicked;
        }
        else
        {
            toggle.clicked -= OnClicked;
        }
    }

    // Handlers
    void OnClicked()
    {
        SetState(!isSelected);
        clicked?.Invoke(isSelected);
    }
}
