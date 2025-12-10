using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class QuizzController : MonoBehaviour
{
    [Header("Scenes")]
    public string winScene;
    public string loseScene;

    [Header("UI Elemente")]
    public TMP_Text questionText;
    public Button[] answerButtons;
    public Image lifeHeartImage;

    [Header("Farben")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    [Header("Grafics")]
    public Sprite redHeart;
    public Sprite greyHeart;

    [System.Serializable]
    public class QuestionData
    {
        [TextArea(2, 5)]
        public string question;
        public string[] answers;
        public int correctAnswerIndex;
    }

    [Header("Questions")]
    public List<QuestionData> questions;

    private int currentQuestionIndex = 0;
    private bool hasLostLife = false;
    private bool isInputBlocked = false;

    private List<int> currentShuffleMapping = new List<int>();

    void Start()
    {
        currentQuestionIndex = 0;
        hasLostLife = false;

        if (redHeart != null) lifeHeartImage.sprite = redHeart;
        lifeHeartImage.color = Color.white;

        LoadQuestion();
    }

    void LoadQuestion()
    {
        isInputBlocked = false;

        if (currentQuestionIndex >= questions.Count)
        {
            WinGame();
            return;
        }

        QuestionData currentQ = questions[currentQuestionIndex];
        questionText.text = currentQ.question;

        currentShuffleMapping.Clear();
        List<int> availableIndices = new List<int>() { 0, 1, 2, 3 };

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].image.color = defaultColor;
            answerButtons[i].interactable = true;

            int randomIndexPosition = Random.Range(0, availableIndices.Count);
            int logicalAnswerIndex = availableIndices[randomIndexPosition];

            currentShuffleMapping.Add(logicalAnswerIndex);

            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                if (logicalAnswerIndex < currentQ.answers.Length)
                {
                    btnText.text = currentQ.answers[logicalAnswerIndex];
                }
            }

            answerButtons[i].onClick.RemoveAllListeners();
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerClicked(buttonIndex));

            availableIndices.RemoveAt(randomIndexPosition);
        }
    }

    public void OnAnswerClicked(int physicalButtonIndex)
    {
        if (isInputBlocked) return;

        int realAnswerIndex = currentShuffleMapping[physicalButtonIndex];
        int correctIndex = questions[currentQuestionIndex].correctAnswerIndex;

        if (realAnswerIndex == correctIndex)
        {
            StartCoroutine(HandleCorrectAnswerSequence(physicalButtonIndex));
        }
        else
        {
            answerButtons[physicalButtonIndex].image.color = wrongColor;
            HandleWrongAnswer(physicalButtonIndex);
        }
    }

    IEnumerator HandleCorrectAnswerSequence(int index)
    {
        isInputBlocked = true;

        answerButtons[index].image.color = correctColor;

        yield return new WaitForSeconds(1.0f);

        currentQuestionIndex++;
        LoadQuestion();
    }

    void HandleWrongAnswer(int buttonIndex)
    {
        answerButtons[buttonIndex].interactable = false;

        if (!hasLostLife)
        {
            hasLostLife = true;
            if (greyHeart != null) lifeHeartImage.sprite = greyHeart;
            else lifeHeartImage.color = Color.gray;
        }
        else
        {
            LoseGame();
        }
    }

    void WinGame()
    {
        SceneManager.LoadScene(winScene);
    }

    void LoseGame()
    {
        SceneManager.LoadScene(loseScene);
    }
}