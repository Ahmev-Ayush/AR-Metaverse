using UnityEngine;
using UnityEngine.InputSystem; // Required for New Input System

public class ExitHandlerForAndroidScript : MonoBehaviour
{
    private InputAction _backButtonAction;

    private void OnEnable()
    {
        // 1. Initialize the action for the Escape key (Android Back button)
        _backButtonAction = new InputAction("Back", binding: "<Keyboard>/escape");

        // 2. Subscribe to the 'performed' event
        _backButtonAction.performed += OnBackButtonPressed;

        // 3. Enable the action
        _backButtonAction.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe and disable to prevent memory leaks
        _backButtonAction.performed -= OnBackButtonPressed;
        _backButtonAction.Disable();
    }

    private void OnBackButtonPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Back button pressed! Exiting app...");
        Application.Quit();
    }
}