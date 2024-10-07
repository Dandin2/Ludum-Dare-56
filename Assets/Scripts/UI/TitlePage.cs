using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePage : MonoBehaviour
{
    public UIClick TheButton;
    public ScreenFader Fader;
    public Texture2D mainCursor;
    internal Vector2 hotSpot = new Vector2(5, 5);
    internal CursorMode cursorMode = CursorMode.Auto;

    private bool fading = false;

    private void Awake()
    {
        Cursor.SetCursor(mainCursor, hotSpot, cursorMode);
        TheButton.SetClickAction(() => { if (!fading) { Fader.transform.parent.gameObject.SetActive(true); Fader.FadeToBlack(1, () => { SceneManager.LoadScene(3); }); } });
    }
}
