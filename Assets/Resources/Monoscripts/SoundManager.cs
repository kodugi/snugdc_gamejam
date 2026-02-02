using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioClip ButtonClickSound;
    [SerializeField] public AudioClip CorrectSound;
    [SerializeField] public AudioClip IncorrectSound;
    [SerializeField] public AudioClip VictorySound;
    [SerializeField] public AudioClip ItemUseSound;
    [SerializeField] public AudioClip TitleScreenMusic;
    [SerializeField] public AudioClip IngameScreenMusic;
    [SerializeField] public AudioClip TutorialMusic;

    public void PlaySound(AudioType audioType)
    {
        GetComponent<AudioSource>().PlayOneShot(
            audioType == AudioType.Correct ? CorrectSound :
            audioType == AudioType.Incorrect ? IncorrectSound :
            audioType == AudioType.Victory ? VictorySound :
            audioType == AudioType.ButtonClick ? ButtonClickSound :
            audioType == AudioType.ItemUse ? ItemUseSound :
            null
        );
    }

    public void PlayMusic(AudioType audioType)
    {
        AudioClip clipToPlay = null;
        switch (audioType)
        {
            case AudioType.TitleScreenMusic:
                clipToPlay = TitleScreenMusic;
                break;
            case AudioType.IngameScreenMusic:
                clipToPlay = IngameScreenMusic;
                break;
            case AudioType.TutorialMusic:
                clipToPlay = TutorialMusic;
                break;
        }

        if (clipToPlay != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = clipToPlay;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}