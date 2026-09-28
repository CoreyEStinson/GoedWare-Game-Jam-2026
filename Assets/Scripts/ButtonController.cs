using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private float movementHeight = 0.1f;
    [SerializeField] private float movementSpeed = 2f;

    private Vector3 startingPosition;
    private float randomOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPosition = transform.localPosition;
        randomOffset = Random.Range(0f, Mathf.PI * 2f);
        movementSpeed = Random.Range(movementSpeed * 0.75f, movementSpeed * 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = startingPosition;
        position.y += Mathf.Sin(Time.time * movementSpeed + randomOffset) * movementHeight;
        transform.localPosition = position;
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
