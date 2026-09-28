using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private Boss uMajor;
    [SerializeField] private Boss uMinor;
    [SerializeField] private TextMeshProUGUI youWinText;

    private void Update()
    {
        if (uMajor.CurrentHealth <= 0 && uMinor.CurrentHealth <= 0)
        {
            StartCoroutine(EndGame());
        }
    }

    private IEnumerator EndGame()
    {
        youWinText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("MainMenu");
    }
}
