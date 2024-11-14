using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1.5f; // Tempo antes de a plataforma começar a cair
    public float resetDelay = 3.0f; // Tempo antes da plataforma retornar à posição original
    public float fallSpeed = 5.0f; // Velocidade constante de queda
    private Vector3 initialPosition; // Posição inicial da plataforma
    private Rigidbody2D rb;
    private TilemapCollider2D tilemapCollider2D;
    private bool playerOnPlatform; // Para controlar quando o jogador está na plataforma

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); // Adiciona um Rigidbody2D para simular a queda
        rb.isKinematic = true; // Define como kinematic para não afetar a plataforma até o momento de cair
        tilemapCollider2D = GetComponent<TilemapCollider2D>();
        tilemapCollider2D.enabled = false;
        initialPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FloorDetection")) // Detecta o FloorDetection na plataforma
        {
            playerOnPlatform = true;
            tilemapCollider2D.enabled = true;
            StartCoroutine(FallAfterDelay());
        }
    }
    

    private IEnumerator FallAfterDelay()
    {
        yield return new WaitForSeconds(fallDelay);

        if (playerOnPlatform) // Confirma se o player ainda está na plataforma
        {
            rb.isKinematic = false; // Ativa a gravidade para a queda

            yield return new WaitForSeconds(resetDelay); // Aguarda o tempo para resetar

            // Reseta a plataforma
            rb.isKinematic = true; // Define o Rigidbody como kinematic para "desligar" a física
            rb.velocity = Vector2.zero; // Zera a velocidade para evitar movimentos bruscos
            transform.position = initialPosition; // Retorna para a posição inicial
            tilemapCollider2D.enabled = false;
            playerOnPlatform = false;
        }
    }
}
