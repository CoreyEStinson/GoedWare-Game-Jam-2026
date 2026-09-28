using UnityEngine;

public class UrsaMionrSounds : MonoBehaviour
{
    [SerializeField] private AudioClip claw;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip roll;
    [SerializeField] private AudioClip walk;

    public AudioSource aud;

    private void Awake()
    {
        // Auto-fetch AudioSource if not assigned in Inspector
        if (aud == null)
        {
            aud = GetComponent<AudioSource>();
        }
    }

    public void PlayClaw() => PlaySound(claw);
    public void PlayDie() => PlaySound(die);
    public void PlayRoll() => PlaySound(roll);
    public void PlayWalk() => PlaySound(walk);

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && aud != null)
        {
            aud.PlayOneShot(clip);
        }
    }
}
