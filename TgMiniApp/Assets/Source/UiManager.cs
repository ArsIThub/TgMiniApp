using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject gamePanel;
    [Space]
    [SerializeField] private Button playButton;
    [SerializeField] private Button menuButton;

    private void Awake()
    {
        playButton.onClick.AddListener(Play);
        menuButton.onClick.AddListener(Menu);
    }

    private void Start()
    {
        Menu();
    }

    private void Play() 
    {
        registerPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void Menu() 
    {
        gamePanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(Play);
        menuButton.onClick.RemoveListener(Menu);
    }
}
