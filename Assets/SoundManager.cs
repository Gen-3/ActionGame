using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClips;

    public AudioSource audioSources;

    void Start()
    {
//        audioSources = GetComponent<AudioSource>();
    }

    void SetWeapon()
    {
        
    }

    public void PlaySoundEffect(int id)
    {
        audioSources.PlayOneShot(audioClips[id]);
        Debug.Log($"効果音{id}を鳴らしました");

        //switch (id)
        //{
        //    case 1:
        //        audioSources.PlayOneShot(1);
        //        Debug.Log($"効果音{id}を鳴らしました");
        //        break;
        //    case 2:
        //        audioSources.PlayOneShot(2);
        //        Debug.Log($"効果音{id}を鳴らしました");
        //        break;
        //    case 3:
        //        audioSources.PlayOneShot(1);
        //        Debug.Log($"効果音{id}を鳴らしました");
        //        break;
        //    case 4:
        //        audioSources.PlayOneShot(2);
        //        Debug.Log($"効果音{id}を鳴らしました");
        //        break;
        //    default:
        //        Debug.Log("効果音設定ミス");
        //        return;
        //}
    }
}
