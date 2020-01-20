using UnityEngine;

public class PlayOnAwake : MonoBehaviour //A script that holds an audio clip to play on game object instantiation, so that we may avoid having an audio source for each.
{
    public AudioClip audioClip; //Set within the editor.

    void Awake()
    {
        GameControl.PlayClip(audioClip);
    }
}
