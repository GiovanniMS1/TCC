using UnityEngine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private PlayerBehaviour playerBehaviour;

    public void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playerBehaviour = FindObjectOfType<PlayerBehaviour>();

        // Registra o evento para desbloquear o jogador quando a cinemática terminar
        playableDirector.stopped += OnCinematicEnd;
    }

    public void PlayCinematic()
    {
        // Bloqueia o controle do jogador quando a cinemática inicia
        playerBehaviour.DisablePlayerControl();

        // Inicia a cinemática
        playableDirector.Play();
    }

    private void OnCinematicEnd(PlayableDirector director)
    {
        playableDirector.stopped -= OnCinematicEnd;

        // Libera o controle do jogador quando a cinemática termina
        playerBehaviour.EnablePlayerControl();
    }
}
