using System.Collections;
using UnityEngine;

public class SongController : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    [SerializeField] private AudioClip songLoop;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource.loop = false;
        audioSource.clip = song;
        audioSource.Play();
        StartCoroutine(PlayLoopAfterSong());
    }

    private IEnumerator PlayLoopAfterSong()
    {
        yield return new WaitForSeconds(song.length);

        audioSource.clip = songLoop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
