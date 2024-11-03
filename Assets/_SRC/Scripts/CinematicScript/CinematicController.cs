using UnityEngine;
using UnityEngine.Playables;

public class CinematicController : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private PlayerBehaviour playerBehaviour;

    void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
        playerBehaviour = FindObjectOfType<PlayerBehaviour>();

        // Bloqueia o controle do jogador no início
        playerBehaviour.DisablePlayerControl();
    }

    void OnEnable()
    {
        // Registra o evento para desbloquear o jogador quando a cinemática terminar
        playableDirector.stopped += OnCinematicEnd;
    }

    void OnDisable()
    {
        // Desregistra o evento
        playableDirector.stopped -= OnCinematicEnd;
    }

    private void OnCinematicEnd(PlayableDirector director)
    {
        // Libera o controle do jogador quando a cinemática termina
        playerBehaviour.EnablePlayerControl();
    }
}
