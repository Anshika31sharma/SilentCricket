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

    public void OnCardClicked()
    {
        if (isMatched || isFlipped) return;

        FlipCard();
        GameManager.Instance.OnCardSelected(this);
    }

    public void FlipCard()
    {
        isFlipped = true;
        if (backSide != null)
            backSide.SetActive(false);
    }

    public void ResetFlip()
    {
        isFlipped = false;
        if (backSide != null)
            backSide.SetActive(true);
    }

    public void MarkAsMatched()
    {
        isMatched = true;
    }
}