using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardId;
    public Image frontImage;
    public GameObject backSide;
    public bool isFlipped = false;
    public bool isMatched = false;

    private void Start()
    {
        ShowBack();
    }

    public void OnCardClicked()
    {
        if (isFlipped || isMatched) return;

        Debug.Log($"Card clicked: {cardId}");
        FlipToFront();
        GameManager.Instance.OnCardSelected(this);
    }

    public void FlipToFront()
    {
        isFlipped = true;
        frontImage.gameObject.SetActive(true);
        backSide.SetActive(false);
    }

    public void ResetFlip()
    {
        if (isMatched) return;
        isFlipped = false;
        ShowBack();
    }

    public void ShowBack()
    {
        frontImage.gameObject.SetActive(false);
        backSide.SetActive(true);
    }

    public void MarkAsMatched()
    {
        isMatched = true;
        isFlipped = true;
        frontImage.gameObject.SetActive(true);
        backSide.SetActive(false);
    }
}
