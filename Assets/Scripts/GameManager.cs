using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GridLayoutGroup grid;
    public GameObject cardPrefab;
    public Sprite[] cardSprites; 
    public TMP_Text scoreText;

    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    private int score = 0;
    private int matchedPairs = 0;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;

    private void Awake()
    {
        Instance = this;

        if (scoreText == null)
        {
            scoreText = GameObject.Find("ScoreText")?.GetComponent<TMP_Text>();
            if (scoreText == null)
                Debug.LogError("ScoreText TMP not found in scene!");
            else
                Debug.Log("ScoreText TMP auto-assigned.");
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("ScoreText (TMP) is NOT assigned!");
        }
        else
        {
            scoreText.text = "Score: 0";
            Debug.Log("ScoreText TMP is connected.");
        }

        SetupGame(2, 2);
    }

    public void SetupGame(int rows, int columns)
    {
        Debug.Log($"SetupGame called with rows={rows}, columns={columns}");

        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        cards.Clear();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        int pairsNeeded = (rows * columns) / 2;
        List<Sprite> chosenSprites = new List<Sprite>();

        for (int i = 0; i < pairsNeeded; i++)
        {
            Sprite sprite = cardSprites[i % cardSprites.Length];
            chosenSprites.Add(sprite);
        }

        chosenSprites.AddRange(chosenSprites);
        Shuffle(chosenSprites);

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, grid.transform);
            Card card = newCard.GetComponent<Card>();
            card.cardId = i;
            card.frontImage.sprite = chosenSprites[i];
            Debug.Log($"Assigned {chosenSprites[i].name} to card {i}");

            card.backSide.SetActive(true);
            cards.Add(card);
        }

        score = 0;
        matchedPairs = 0;
        UpdateScoreUI();
    }

    public void OnCardSelected(Card card)
    {
        PlaySound(flipSound);

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            CheckMatch();
        }
    }

    private void CheckMatch()
    {
        if (firstCard == null || secondCard == null) return;

        Debug.Log($"Checking Match: {firstCard.frontImage.sprite.name} vs {secondCard.frontImage.sprite.name}");

        if (firstCard.frontImage.sprite == secondCard.frontImage.sprite)
        {
            score += 10;
            matchedPairs++;
            firstCard.MarkAsMatched();
            secondCard.MarkAsMatched();
            PlaySound(matchSound);

            UpdateScoreUI();
            firstCard = null;
            secondCard = null;

            CheckGameOver();
        }
        else
        {
            Debug.Log("No match, flipping back.");
            PlaySound(mismatchSound);
            Invoke(nameof(ResetCards), 1f);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    private void ResetCards()
    {
        if (firstCard != null) firstCard.ResetFlip();
        if (secondCard != null) secondCard.ResetFlip();

        firstCard = null;
        secondCard = null;
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    private void CheckGameOver()
    {
        int totalPairs = (cards.Count) / 2;
        if (matchedPairs >= totalPairs)
        {
            Debug.Log("Game Over! All cards matched.");
            PlaySound(gameOverSound);
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
            cards.Clear();
            scoreText.text = $"Score: {score} - Game Over!";
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
