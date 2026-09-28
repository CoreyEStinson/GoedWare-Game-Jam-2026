using UnityEngine;

public class UrsaMajorSounds : MonoBehaviour
{
    [SerializeField] private AudioClip claw;
    [SerializeField] private AudioClip die;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;
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

    public void Claw() => PlaySound(claw);
    public void Die() => PlaySound(die);
    public void Jump() => PlaySound(jump);
    public void Land() => PlaySound(land);
    public void Roll() => PlaySound(roll);
    public void Walk() => PlaySound(walk);

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && aud != null)
        {
            aud.PlayOneShot(clip);
        }
    }
}