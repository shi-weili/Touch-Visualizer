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
    bool isTouchEnabled = false;
    bool onlyAllowToggleOn;

    // Public functions

    /// <summary>
    /// Create a ToogleController instance using a toggle UI reference and a initial state.
    /// </summary>
    /// <param name="toggle">Of UIElements.Button type. Has to be a toggle.</param>
    /// <param name="onlyAllowToggleOn">If true, can't be toggled off by clicking.</param>
    public ToggleController(Button toggle, bool selected, bool onlyAllowToggleOn = false)
    {
        this.toggle = toggle;
        SetState(selected);
        this.onlyAllowToggleOn = onlyAllowToggleOn;
        SetTouchEnabled(false);
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
        if (isSelected && !onlyAllowToggleOn)
        {
            SetState(false);
        }
        else if (!isSelected)
        {
            SetState(true);
        }

        clicked?.Invoke(isSelected);
    }
}
