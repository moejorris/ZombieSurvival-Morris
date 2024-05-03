//////////////////////////////////////////////
//Assignment/Lab/Project: Zombie Survival
//Name: Joe Morris
//Section: SGD.213.0021
//Instructor: Professor Locklear
//Date: 05/02/2024
/////////////////////////////////////////////

using System.Linq;
using UnityEngine;

public class EnemySoundBank : MonoBehaviour
{
    //Modified component from my Basic AI project
    //Different Types of Sounds: arrays that hold sound effects for different behaviors of the enemy
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip[] chaseSounds;
    [SerializeField] AudioClip[] patrolSounds;
    [SerializeField] AudioClip[] deathSounds;

    AudioSource audioSource;

    void Start()
    {
        //Get reference to the audio source and set it's volume to the GM's enemy sound volume. This is done to ensure all enemies are at the same volume
        audioSource = GetComponent<AudioSource>();
    }

    void PlayRandomSound(AudioClip[] soundArray)
    {
        //Returns if the gameobject is inactive, or already playing a sound effect, or too many enemies are making a sound effect and the sound effect inputted is not an attack sound. 
        //The enemy sound effect limit is only allowed to be ignored if the sound effect inputted is an attack sound, as they do not last long.
        if(!gameObject.activeInHierarchy || audioSource.isPlaying)
        {
            return;
        }
        //plays a random sound effect from the inputted array of sound effects
        audioSource.PlayOneShot(soundArray[Random.Range(0, soundArray.Length)]);
    }

    //Called by enemy controller and plays the sound type in the name of the function.
    public void PlayAttackSound()
    {
        PlayRandomSound(attackSounds);
    }

    public void PlayChaseSound()
    {
        audioSource.minDistance = 3; //if the zombies are running (they yell) so the min distance is set to 3 so you can hear them from further away.
        PlayRandomSound(chaseSounds);
    }

    public void PlayPatrolSound()
    {
        PlayRandomSound(patrolSounds);
    }

    public void PlayDeathSound() //the death sound instantiates a game object so the sound plays after the zombie is destroyed. Sound is destroyed after it is finished playing
    {
        //instantiate empty gameobject with audio source and choose the death sound
        GameObject deathSound = new GameObject("zombieDeathSounds");
        AudioSource deathAudioSource = deathSound.AddComponent<AudioSource>();
        AudioClip sound = deathSounds[Random.Range(0, deathSounds.Length)];
        
        //destroy object after sound effect plays
        Destroy(deathSound, sound.length);
        
        //copy spatial audio sounds from this audio source
        deathAudioSource.spatialBlend = audioSource.spatialBlend;
        deathAudioSource.minDistance = audioSource.minDistance;
        deathAudioSource.maxDistance = audioSource.maxDistance;
        
        //play the death sound effect
        deathAudioSource.PlayOneShot(sound);
    }
}
