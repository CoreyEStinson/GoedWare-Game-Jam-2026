using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Boss uMajor;
    [SerializeField] private Boss uMinor;
    [SerializeField] private TextMeshProUGUI youWinText;

    [SerializeField] private Vector4 mapBounds;

    private Vector2 lastPosition;

    private void Update()
    {
        if (uMajor.CurrentHealth <= 0 && uMinor.CurrentHealth <= 0)
        {
            StartCoroutine(EndGame());
        }

        Vector2 newPosition = player.position;

        if (player.position.y > mapBounds.x)
        {
            newPosition.y = mapBounds.x;
        }
        if (player.position.x > mapBounds.y)
        {
            newPosition.x = mapBounds.y;
        }
        if (player.position.y < mapBounds.z)
        {
            newPosition.y = mapBounds.z;
        }
        if (player.position.x < mapBounds.w)
        {
            newPosition.x = mapBounds.w;
        }

        if (newPosition != Vector2.zero)
        {
            player.position = newPosition;
        }

        player.position = newPosition;
        lastPosition = player.position;
    }

    private IEnumerator EndGame()
    {
        youWinText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("MainMenu");
    }
}
