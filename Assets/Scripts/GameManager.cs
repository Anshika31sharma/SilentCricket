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
    public TMP_Text timerText; 

    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    private int score = 0;
    private int matchedPairs = 0;

    [Header("Timer Settings")]
    public float totalTime = 120f; 
    private float remainingTime;
    private bool gameEnded = false;

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
        }

        if (timerText == null)
        {
            timerText = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
            if (timerText == null)
                Debug.LogError("TimerText TMP not found in scene!");
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (scoreText != null)
            scoreText.text = "Score: 0";
 if (timerText != null)
            timerText.text = "Time: 02:00";
        SetupGame(4, 4); 
        remainingTime = totalTime;
        UpdateTimerUI();
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
    }

    private void UpdateTimer()
    {
        if (gameEnded) return;

        remainingTime -= 1f;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            TimerEnd();
        }
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return; 
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
private void TimerEnd()
{
    CancelInvoke(nameof(UpdateTimer));
    gameEnded = true;
    Debug.Log("Time's Up!");

    foreach (Transform child in grid.transform)
    {
        Destroy(child.gameObject);
    }
    if (timerText != null)
        timerText.gameObject.SetActive(false);

    if (scoreText != null)
        scoreText.text = $"Time's Up! Final Score: {score}";
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

    public void SetupGame(int rows, int columns)
    {
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
            card.backSide.SetActive(true);
            cards.Add(card);
        }

        score = 0;
        matchedPairs = 0;
        UpdateScoreUI();
    }

    private void CheckGameOver()
    {
        int totalPairs = (cards.Count) / 2;
        if (matchedPairs >= totalPairs)
        {
            CancelInvoke(nameof(UpdateTimer));
            gameEnded = true;
            PlaySound(gameOverSound);
            foreach (Transform child in grid.transform)
            {
                Destroy(child.gameObject);
            }
            cards.Clear();
            if (scoreText != null)
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
