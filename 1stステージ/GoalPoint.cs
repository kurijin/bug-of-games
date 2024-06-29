using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalPoint : MonoBehaviour
{
    public AudioClip goalSound; 
    private AudioSource audioSource; 
    private GameManager gameManager; // GameManagerスクリプト

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ゴール地点に到達したらクリア音を鳴らす,またGameManagerスクリプトのクリア後会話のためのメソッドを呼びだず
            PlayGoalSound();
            if (gameManager != null)
            {
                gameManager.PlayerReachedGoal();
            }
        }
    }

    private void PlayGoalSound()
    {
        if (goalSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(goalSound);
        }
    }
}
