using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePage : MonoBehaviour
{
    public UIClick TheButton;
    public ScreenFader Fader;

    private bool fading = false;

    private void Awake()
    {
        TheButton.SetClickAction(() => { if (!fading) { Fader.transform.parent.gameObject.SetActive(true); Fader.FadeToBlack(1, () => { SceneManager.LoadScene(3); }); } });
    }
}
