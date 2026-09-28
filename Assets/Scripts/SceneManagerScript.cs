using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private HealthComponent playerHealth;
    [SerializeField] private Boss uMajor;
    [SerializeField] private Boss uMinor;
    [SerializeField] private TextMeshProUGUI youWinText;
    [SerializeField] private TextMeshProUGUI youLoseText;

    [SerializeField] private Vector4 mapBounds;

    private Vector2 lastPosition;

    private void Update()
    {
        if (uMajor.CurrentHealth <= 0 && uMinor.CurrentHealth <= 0)
        {
            StartCoroutine(EndGame(0));
        }
        else if (playerHealth.Health <= 0)
        {
            StartCoroutine(EndGame(1));
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

        // End da game if the player dies

        player.position = newPosition;
        lastPosition = player.position;
    }

    private IEnumerator EndGame(int condition)
    {
        switch (condition)
        {
            case 0:
                // Good ending
                youWinText.gameObject.SetActive(true);

                break;

            case 1:
                // Bad ending
                youLoseText.gameObject.SetActive(true);

                break;
        }

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene("MainMenu");
    }
}
