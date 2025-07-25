using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [Header("Card Properties")]
    public int cardId;
    public Image frontImage;
    public GameObject backSide;

    private bool isFlipped = false;
    private bool isMatched = false;

    [Header("Animation Settings")]
    public float flipDuration = 0.15f;  
    public Color backColor = new Color(0.2f, 0.4f, 0.8f, 1f); 
    public Color hoverColor = new Color(0.4f, 0.6f, 1f, 1f); 
    private Button backButton;
    private Image backImage;

    private void Awake()
    {
        backButton = backSide.GetComponent<Button>();
        if (backButton == null)
            backButton = backSide.AddComponent<Button>();

        backButton.transition = Selectable.Transition.None;
        backButton.onClick.AddListener(OnCardClicked);

        backImage = backSide.GetComponent<Image>();
        if (backImage != null)
            backImage.color = backColor;
    }

    private void Update()
    {
        if (backImage != null)
        {
            if (IsMouseOver(backSide))
                backImage.color = Color.Lerp(backImage.color, hoverColor, Time.deltaTime * 8f);
            else
                backImage.color = Color.Lerp(backImage.color, backColor, Time.deltaTime * 8f);
        }
    }

    public void OnCardClicked()
    {
        if (isMatched || isFlipped) return;

        FlipCard();
        GameManager.Instance.OnCardSelected(this);
        Debug.Log("Card clicked!");
    }

    public void FlipCard()
    {
        isFlipped = true;
        StopAllCoroutines();
        StartCoroutine(FlipAnimation(false));
    }

    public void ResetFlip()
    {
        isFlipped = false;
        StopAllCoroutines();
        StartCoroutine(FlipAnimation(true));
    }

    public void MarkAsMatched()
    {
        isMatched = true;
        if (frontImage != null)
            StartCoroutine(MatchPulse());
    }

    private System.Collections.IEnumerator FlipAnimation(bool showBack)
    {
        float time = 0;
        Transform target = transform;

        while (time < flipDuration)
        {
            float scale = Mathf.Lerp(1f, 0f, time / flipDuration);
            target.localScale = new Vector3(scale, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }
        target.localScale = new Vector3(0f, 1f, 1f);

        if (backSide != null)
            backSide.SetActive(showBack);

        time = 0;
        while (time < flipDuration)
        {
            float scale = Mathf.Lerp(0f, 1f, time / flipDuration);
            target.localScale = new Vector3(scale, 1f, 1f);
            time += Time.deltaTime;
            yield return null;
        }
        target.localScale = new Vector3(1f, 1f, 1f);
    }

    private System.Collections.IEnumerator MatchPulse()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float t = 0f;
        while (t < 0.2f)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0f;
        while (t < 0.2f)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }

    private bool IsMouseOver(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
    }
}
