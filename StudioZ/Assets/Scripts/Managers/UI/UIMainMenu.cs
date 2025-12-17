using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerSelector;
    private VisualElement root;

    // buttons are named "0", "1", "2", "3"
    private readonly int firstIndex = 1;  // initial focus → Start button

    private List<Button> buttons = new List<Button>();
    private int current = 1;

    private bool controllerActive = true;
    private Vector2 lastMouse;

    private bool controllerJustConnected = false;

    public AudioSource clickSound;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        // Reset list to prevent duplicate references
        buttons.Clear();

        // collect buttons (0–3)
        for (int i = 0; i <= 3; i++)
        {
            Button b = root.Q<Button>(i.ToString());
            if (b != null)
                buttons.Add(b);
        }

        // remove old callbacks just in case
        buttons[0].clicked -= Quit;
        buttons[1].clicked -= StartGame;
        buttons[2].clicked -= OpenSettings;
        buttons[3].clicked -= OpenHowToPlay;

        // add fresh callbacks
        buttons[0].clicked += Quit;
        buttons[1].clicked += StartGame;
        buttons[2].clicked += OpenSettings;
        buttons[3].clicked += OpenHowToPlay;

        // input
        root.RegisterCallback<MouseMoveEvent>(OnMouseMove);

        controllerActive = Gamepad.current != null;

    }


    private void OnMouseMove(MouseMoveEvent evt)
    {
        if ((evt.mousePosition - lastMouse).sqrMagnitude > 1f)
        {
            controllerActive = false;
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        lastMouse = evt.mousePosition;
    }

    private void Update()
    {
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.5f)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            controllerActive = false;
            if (current >= 0 && current < buttons.Count)
                buttons[current].RemoveFromClassList("LevelButtonsFocus");
            root.Focus();
        }
        var pad = Gamepad.current;
        if (pad == null)
        {
            controllerJustConnected = false;
            return;
        }
        if (pad != null && !controllerJustConnected)
        {
            controllerJustConnected = true;
            buttons[current].Focus();
            buttons[current].AddToClassList("LevelButtonsFocus");
        }
        // controller takes control
        if (pad.leftStick.ReadValue().sqrMagnitude > 0.3f ||
            pad.dpad.ReadValue() != Vector2.zero || pad.buttonSouth.wasPressedThisFrame)
        {
            if (!controllerActive)
            {
                controllerActive = true;
                UnityEngine.Cursor.visible = false;
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                buttons[current].Focus();
                buttons[current].AddToClassList("LevelButtonsFocus");
            }
        }

        if (!controllerActive) return;

        if (pad.dpad.left.wasPressedThisFrame || pad.leftStick.left.wasPressedThisFrame)
        {
            if (current == 3)
                Move(-1);

            Move(-1);

        }

        if (pad.dpad.right.wasPressedThisFrame || pad.leftStick.right.wasPressedThisFrame)
            Move(+1);
        if ((pad.dpad.up.wasPressedThisFrame || pad.leftStick.up.wasPressedThisFrame) && current == 3)
            Move(-1);
        if ((pad.dpad.down.wasPressedThisFrame || pad.leftStick.down.wasPressedThisFrame) && current == 2)
            Move(+1);
        else if (pad.dpad.down.wasPressedThisFrame || pad.leftStick.down.wasPressedThisFrame)
            Focus(1);
        if (pad.buttonSouth.wasPressedThisFrame)
            Activate();

        
    }

    private void Move(int direction)
    {
        int next = current + direction;

        if (next < 0 || next >= buttons.Count)
            return;

        Focus(next);


    }

    private void Focus(int index)
    {
        if (current >= 0 && current < buttons.Count)
            buttons[current].RemoveFromClassList("LevelButtonsFocus");

        current = index;

        buttons[current].Focus();
        buttons[current].AddToClassList("LevelButtonsFocus");
    }

    private void Activate()
    {
        buttons[current].SendEvent(new ClickEvent());
    }

    // ---------------- BUTTON ACTIONS ----------------

    private void Quit()
    {
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif

    }

    private void StartGame()
    {
        controllerJustConnected = false;
        playerSelector.SetActive(true);
        this.gameObject.SetActive(false);
       
    }

    private void OpenSettings()
    {
        Debug.Log("SETTINGS");
    }

    private void OpenHowToPlay()
    {
        Debug.Log("HOW TO PLAY");
    }
}
