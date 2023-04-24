using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ManagingMath : MonoBehaviour
{

    [Space]
    [Header("First Question")]
    [Space]

    public int term_1_firstTerm;
    public int term_1_secondTerm;
    public string symbol_1_operator;
    public int term_1_answer;

    [Space]
    [Header("Second Question")]
    [Space]

    public int term_2_firstTerm;
    public int term_2_secondTerm;
    public string symbol_2_operator;
    public int term_2_answer;

    [Space]
    [Header("Third Question")]
    [Space]

    public int term_3_firstTerm;
    public int term_3_secondTerm;
    public string symbol_3_operator;
    public int term_3_answer;

    [Space]
    [Header("Box 1 (Top) Reference")]
    [Space]

    public Transform box_one;
    public TextMeshProUGUI box_one_string;
    public TextMeshProUGUI box_one_input;
    private Transform box_temp_one;

    [Space]
    [Header("Box 2 (Middle) Reference")]
    [Space]

    public Transform box_two;
    public TextMeshProUGUI box_two_string;
    public TextMeshProUGUI box_two_input;
    private Transform box_temp_two;

    [Space]
    [Header("Box 3 (Bottom) Reference")]
    [Space]

    public Transform box_three;
    public TextMeshProUGUI box_three_string;
    public TextMeshProUGUI box_three_input;
    private Transform box_temp_three;

    [Space]
    [Header("Box 1-3 Canvas Groups")]
    [Space]

    public CanvasGroup[] boxCanasGroups; 

    [Space]
    [Header("Box Images Reference")]
    [Space]

    public Image[] boxImages;
    private Color alphaColor;
    private Color fullColor;

    [Space]
    [Header("Box Images Reference")]
    [Space]

    public TMP_InputField inputField;
    public CanvasGroup inputField_canvasGroup;

    [Space]
    [Header("Top Bars")]
    [Space]

    public TextMeshProUGUI questionsLeft;
    public TextMeshProUGUI timer_text;
    public TextMeshProUGUI level_text;

    private bool isTransitioning = false;
    private Transform[] boxes = new Transform[3];
    private TextMeshProUGUI[] boxes_string = new TextMeshProUGUI[3];
    private TextMeshProUGUI[] boxes_input = new TextMeshProUGUI[3];
    private Transform[] boxes_temp = new Transform[3];

    //calculating time
    private int timer = 305;
    private int timeLeft = 300;
    private bool startGame = false;
    private Coroutine timerCoroutine;
    private int movementTime = 1;
    private int minutesLeft;
    private int secondsLeft;

    private int level = 0;
    private int currentQuestionTotal = 5;
    private int currentCount = -1;
    private bool multiplicationAllowed = true;
    private bool multiplicationAccessed = false;

    [Space]
    [Header("Buttons")]
    [Space]

    public CanvasGroup checkMarkAlpha;
    public Button checkMarkButton;

    [Space]
    [Header("Audio")]
    [Space]

    public AudioSource audio_gameMusic;
    public AudioSource audio_clickSound;
    public AudioSource audio_correctAnswer;
    public AudioSource audio_incorrectAnswer;
    public AudioSource audio_gameOver;

    [Space]
    [Header("Lives")]
    [Space]

    public List<Transform> lives;
    public Transform gameOverScreen;
    public Transform livesParent;
    public Transform hiddenLife;

    [Space]
    [Header("Settings")]
    [Space]

    public Slider slider_audioEffects;
    public AudioMixer mixer_overall;
    public Slider slider_backgroundMusic;
    public Toggle toggle_fullScreen;

    // Start is called before the first frame update
    void Start()
    {

        audio_gameMusic.Play();

        inputField.gameObject.SetActive(false);

        boxes[0] = box_one;
        boxes[1] = box_two;
        boxes[2] = box_three;

        boxes_string[0] = box_one_string;
        boxes_string[1] = box_two_string;
        boxes_string[2] = box_three_string;

        boxes_input[0] = box_one_input;
        boxes_input[1] = box_two_input;
        boxes_input[2] = box_three_input;

        //the third box must be made first
        box_temp_three = Instantiate(box_three.gameObject).transform;
        box_temp_three.SetParent(box_three.parent);
        box_temp_three.position = box_three.position;

        box_temp_one = Instantiate(box_one.gameObject).transform;
        box_temp_one.SetParent(box_one.parent);
        box_temp_one.position = box_one.position;

        box_temp_two = Instantiate(box_two.gameObject).transform;
        box_temp_two.SetParent(box_two.parent);
        box_temp_two.position = box_two.position;

        box_temp_one.gameObject.SetActive(false);
        box_temp_two.gameObject.SetActive(false);
        box_temp_three.gameObject.SetActive(false);

        boxes_temp[0] = box_temp_one;
        boxes_temp[1] = box_temp_two;
        boxes_temp[2] = box_temp_three;

        alphaColor = boxImages[0].GetComponent<Image>().color;
        fullColor = alphaColor;
        alphaColor.a = 0;

        //StartCoroutine(moveBoxes(3));
        //StartCoroutine(testQuestions(3));
    }

    public void removeLife()
    {
        if(lives.Count > 2)
        {
            Destroy(lives[lives.Count - 1].gameObject);
            lives.RemoveAt(lives.Count - 1);
        }
        else
        {
            Destroy(lives[lives.Count - 1].gameObject);
            lives.RemoveAt(lives.Count - 1);
            audio_gameOver.Play();
            pauseGame();
            gameOverScreen.gameObject.SetActive(true);
        }
    }

    public void addLife()
    {
        if (lives.Count > 0 && lives.Count <= 5)
        {
            Transform temp = Instantiate(lives[0].gameObject).transform;
            temp.SetParent(lives[0].parent);
            temp.position = lives[0].position;
            temp.GetComponent<Image>().enabled = true;
            temp.gameObject.SetActive(true);
            lives.Add(temp);
        }
    }

    public void pauseGame()
    {
        StopAllCoroutines();
        box_temp_one.gameObject.SetActive(false);
        box_temp_two.gameObject.SetActive(false);
        box_temp_three.gameObject.SetActive(false);
        inputField.gameObject.SetActive(false);
        startGame = false;
        isTransitioning = true;
    }

    public void restartGame()
    {
        //box_temp_one.gameObject.SetActive(false);
        //box_temp_two.gameObject.SetActive(false);
        //box_temp_three.gameObject.SetActive(false);
        startGame = false;
        isTransitioning = true;
        setupFirstLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Code to execute when the player presses the Space key
            if (inputField.text.Length > 0)
            {
                checkAnswer();
            }
        }
    }

    public void openSettings()
    {
        float vol = -1;
        mixer_overall.GetFloat("Volume_SFX", out vol);
        Debug.Log(vol);
        slider_audioEffects.value = (vol + 80f) / 80f;

        mixer_overall.GetFloat("Volume_BackgroundMusic", out vol);
        slider_backgroundMusic.value = (vol + 80f) / 80f;
    }

    public void updateAudioSettings()
    {
        mixer_overall.SetFloat("Volume_SFX", Mathf.Log10(slider_audioEffects.value) * 20);
        //mixer_overall.SetFloat("Volume_SFX", slider_audioEffects.value);
        mixer_overall.SetFloat("Volume_BackgroundMusic", Mathf.Log10(slider_backgroundMusic.value) * 20);
    }

    public void updateFullScreen()
    {
        Screen.fullScreen = toggle_fullScreen.isOn;
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void checkAnswer()
    {
        if(currentCount == currentQuestionTotal)
        {
            if (inputField.text.Equals(term_2_answer.ToString()) && isTransitioning == false)
            {
                audio_correctAnswer.Play();
                Debug.Log("CONGRATS");
            }
            else
            {
                audio_incorrectAnswer.Play();
                Debug.Log("INCORRECT");
                removeLife();
            }
            Debug.Log("GOING TO NEXT LEVEL");
            nextLevel();
        }
        else
        {
            Debug.Log(level + " | " + currentCount + " | " + term_2_answer.ToString());
            if (inputField.text.Equals(term_2_answer.ToString()) && isTransitioning == false)
            {
                audio_correctAnswer.Play();
                Debug.Log("CONGRATS");
                nextQuestion(movementTime);
            }
            else
            {
                audio_incorrectAnswer.Play();
                Debug.Log("INCORRECT");
                removeLife();
                nextQuestion(movementTime);
            }
        }
    }

    public void checkIfAnswerIsApplicable()
    {
        if(inputField.text.Length > 0)
        {
            checkMarkAlpha.alpha = 1f;
            checkMarkButton.interactable = true;
        }
        else
        {
            checkMarkButton.interactable = false;
            checkMarkAlpha.alpha = 0.75f;
        }
    }

    public IEnumerator testQuestions(int time)
    {
        //StartCoroutine(moveBoxes(3));
        yield return new WaitForSeconds(5);
        nextQuestion(time);
        yield return new WaitForSeconds(5);
        nextQuestion(time);
    }

    public void generateAllQuestions()
    {
        generateQuestion(ref term_1_firstTerm, ref term_1_secondTerm, ref symbol_1_operator, ref term_1_answer);
        generateQuestion(ref term_2_firstTerm, ref term_2_secondTerm, ref symbol_2_operator, ref term_2_answer);
        generateQuestion(ref term_3_firstTerm, ref term_3_secondTerm, ref symbol_3_operator, ref term_3_answer);
    }

    public IEnumerator moveBoxesMultiple(int count, int time)
    {
        for(int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(time * i);
            Debug.Log("running!");
            StartCoroutine(moveBoxes(3));
        }
    }

    public IEnumerator moveBoxes(int time)
    {

        isTransitioning = true;
        timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");

        inputField.gameObject.SetActive(false);
        inputField.text = "";

        //set all invisible temporary boxes as visible
        foreach (Transform child in boxes_temp)
        {
            child.gameObject.SetActive(true);
        }

        //hide the original boxes
        foreach (Transform child in boxes)
        {
            child.gameObject.SetActive(false);
        }

        showcaseQuestions();

        //boxes_temp[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[0].text;
        //boxes_temp[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[1].text;
        //boxes_temp[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[2].text;

        boxes_temp[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[0].text;
        boxes_temp[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[1].text;
        boxes_temp[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = "? " + "? ? =";
        boxes_temp[0].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        boxes_temp[1].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + term_2_answer;
        boxes_temp[2].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";

        boxes_temp[0].GetComponent<CanvasGroup>().alpha = 1f;
        boxes_temp[1].GetComponent<CanvasGroup>().alpha = 0.75f;
        boxes_temp[2].GetComponent<CanvasGroup>().alpha = 0.75f;

        boxes_temp[0].GetChild(1).GetComponent<Image>().color = fullColor;
        boxes_temp[1].GetChild(1).GetComponent<Image>().color = alphaColor;
        boxes_temp[2].GetChild(1).GetComponent<Image>().color = fullColor;

        //do all the movement of the temporary boxes

        //one is set as two
        box_temp_one.DOScale(box_two.localScale, time);
        box_temp_one.DOMove(box_two.position, time);

        //two is set as three
        box_temp_two.DOScale(box_three.localScale, time);
        box_temp_two.DOMove(box_three.position, time);

        //three is set as one
        box_temp_three.DOScale(box_one.localScale, time);
        box_temp_three.DOMove(box_one.position, time);

        switchQuestions();
        showcaseQuestions();

        yield return new WaitForSeconds(time);

        //set all invisible temporary boxes as visible
        foreach (Transform child in boxes_temp)
        {
            child.gameObject.SetActive(false);
        }

        //hide the original boxes
        foreach (Transform child in boxes)
        {
            child.gameObject.SetActive(true);
        }

        box_temp_one.localScale = box_one.localScale;
        box_temp_one.position = box_one.position;

        box_temp_two.localScale = box_two.localScale;
        box_temp_two.position = box_two.position;

        box_temp_three.localScale = box_three.localScale;
        box_temp_three.position = box_three.position;

        timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");

        isTransitioning = false;

        inputField.gameObject.SetActive(true);

        inputField.ActivateInputField();

        //inputField.Select();

        timerCoroutine = StartCoroutine(startTime());

    }

    public void switchQuestions()
    {

        int temp_2_firstTerm = term_2_firstTerm;
        int temp_2_secondTerm = term_2_secondTerm;
        string term_2_operator = symbol_2_operator;
        int temp_2_answer = term_2_answer;

        //set values of two as values of one
        term_2_firstTerm = term_1_firstTerm;
        term_2_secondTerm = term_1_secondTerm;
        symbol_2_operator = symbol_1_operator;
        term_2_answer = term_1_answer;

        //set values of three as values of two (from the temp values of it)
        term_3_firstTerm = temp_2_firstTerm;
        term_3_secondTerm = temp_2_secondTerm;
        symbol_3_operator = term_2_operator;
        term_3_answer = temp_2_answer;

        //set values of one from the new equation
        generateQuestion(ref term_1_firstTerm, ref term_1_secondTerm, ref symbol_1_operator, ref term_1_answer);

    }

    public IEnumerator startTime()
    {
        while (isTransitioning == false && startGame == true)
        {
            // Decrease the time left by one second
            timeLeft -= 1;
            yield return new WaitForSeconds(1);

            // Calculate the minutes and seconds left
            minutesLeft = timeLeft / 60;
            secondsLeft = timeLeft % 60;

            // Update the timer text
            timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");

            // If the timer reaches zero, print a debug log
            if (timeLeft <= 0)
            {
                Debug.Log("Timer has reached zero!");
                // Stop the timer
                startGame = false;
            }
        }
    }

    public void setupFirstLevel()
    {
        gameOverScreen.gameObject.SetActive(false);
        StopAllCoroutines();
        isTransitioning = false;
        startGame = false;

        foreach (Transform child in livesParent)
        {
            if(child != hiddenLife)
            {
                Destroy(child.gameObject);
            }
        }

        lives = new List<Transform>(){ hiddenLife };

        addLife();
        addLife();
        addLife();

        currentCount = 0;
        currentQuestionTotal = 5;
        timer = 300;
        timeLeft = timer;
        level = 1;
        level_text.text = "Level " + level;
        multiplicationAccessed = false;
        questionsLeft.text = "0 / 5";
        generateAllQuestions();
        showcaseQuestions();
        inputField.gameObject.SetActive(true);
        startGame = true;
        timerCoroutine = StartCoroutine(startTime());

    }

    public void nextLevel()
    {
        addLife();
        boxes_string[0].text = "";
        boxes_string[1].text = "";
        boxes_string[2].text = "";
        boxes_temp[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        boxes_temp[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        boxes_temp[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        inputField.gameObject.SetActive(false);
        startGame = false;
        StopCoroutine(timerCoroutine);
        timer_text.text = "";
        currentCount = 0;
        currentQuestionTotal++;
        timer = timer - 15;
        if (timer < 60)
        {
            timer = 60;
        }
        timeLeft = timer;
        timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");
        level++;
        if (level > 3)
        {
            multiplicationAccessed = true;
        }
        level_text.text = "Level " + level;
        questionsLeft.text = "" + currentCount + " / " + currentQuestionTotal;
        generateAllQuestions();
        showcaseQuestions();
        StartCoroutine(moveBoxes(movementTime));
        startGame = true;
        timerCoroutine = StartCoroutine(startTime());
        inputField.gameObject.SetActive(true);
    }

    public void nextQuestion(int time)
    {
        minutesLeft = timeLeft / 60;
        secondsLeft = timeLeft % 60;
        timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");
        StopCoroutine(timerCoroutine);
        timer_text.text = minutesLeft.ToString("00") + ":" + secondsLeft.ToString("00");
        Debug.Log("Going to next question");
        currentCount++;
        questionsLeft.text = "" + currentCount + " / " + currentQuestionTotal;
        //generateQuestion(ref term_1_firstTerm, ref term_1_secondTerm, ref symbol_1_operator, ref term_1_answer);
        StartCoroutine(moveBoxes(time));
    }

    public void showcaseQuestions()
    {

        checkMarkAlpha.alpha = 0.75f;
        checkMarkButton.interactable = false;

        //show the equation for the first question along with on its duplicate
        boxes_string[0].text = "" + term_1_firstTerm + " " + symbol_1_operator + " " + term_1_secondTerm + " =";
        boxImages[0].color = fullColor;
        boxCanasGroups[0].alpha = 0.75f;
        //boxes_temp[0].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[0].text;
        //Debug.Log(boxes_string[0].text);

        boxes_string[1].text = "" + term_2_firstTerm + " " + symbol_2_operator + " " + term_2_secondTerm + " =";
        boxImages[1].color = fullColor;
        boxCanasGroups[1].alpha = 1f;
        //boxes_temp[1].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[1].text;

        boxes_string[2].text = "" + term_3_firstTerm + " " + symbol_3_operator + " " + term_3_secondTerm + " =";
        boxes_input[2].text = "" + term_3_answer;
        boxImages[2].color = alphaColor;
        boxCanasGroups[2].alpha = 0.75f;

        if (currentCount == 0)
        {
            boxes_string[2].text = "";
            boxes_input[2].text = "";
        }
        //boxes_temp[2].GetChild(0).GetComponent<TextMeshProUGUI>().text = boxes_string[2].text;

        //boxes_temp[0].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + term_1_answer;
        //boxes_temp[1].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + term_2_answer;
        //boxes_temp[2].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + term_3_answer;

        boxes_input[0].gameObject.SetActive(true);
        boxes_input[1].gameObject.SetActive(false);
        boxes_input[2].gameObject.SetActive(true);

    }

    public void generateQuestion(ref int e_term1, ref int e_term2, ref string e_operator, ref int e_answer)
    {
        //this will make a random question for whatever the values are.
        //the parameters will actually be replaced manually due to "ref"

        int remainder = -1;
        int temp_e_operator = -1;
        while (remainder != 0)
        {
            //for a randomRange(inclusive, exclusive)

            //get a random operator if it previously wasnt selected
            //operator is based on an integer
            //1 = add
            //2 = subtract
            //3 = multiply
            //4 = divide
            if (temp_e_operator == -1) { temp_e_operator = Random.Range(1, 5); }

            //check if multiplication is allowed
            //if not, only let adding (1) or substraction (2)
            if (multiplicationAccessed == false || multiplicationAllowed != true)
            {
                temp_e_operator = Random.Range(1, 3);
            }
            //addition
            if(temp_e_operator == 1)
            {
                //get random terms
                e_term1 = Random.Range(0, 5 * level);
                e_term2 = Random.Range(0, 5 * level);

                e_operator = "+";
                e_answer = e_term1 + e_term2;
                remainder = 0; // dont need to worry about if the answer is a whole number
            }
            //subtraction
            else if (temp_e_operator == 2)
            {
                //get random terms
                e_term1 = Random.Range(0, 5 * level);
                e_term2 = Random.Range(0, 5 * level);

                e_operator = "-";
                e_answer = e_term1 - e_term2;
                remainder = 0; // dont need to worry about if the answer is a whole number
            }
            //multiplication
            else if (temp_e_operator == 3)
            {
                //get random terms
                e_term1 = Random.Range(-12, 13);
                e_term2 = Random.Range(-12, 13);

                e_operator = "*";
                e_answer = e_term1 * e_term2;
                remainder = 0; // dont need to worry about if the answer is a whole number
            }
            //division
            else if (temp_e_operator == 4)
            {
                //get random terms
                int maxLevelValue = Random.Range(1,level-2);
                if(level > 10)
                {
                    maxLevelValue = 12;
                }
                int minValue = -12 * maxLevelValue;
                e_term1 = Random.Range(minValue, Mathf.Abs(minValue));
                e_term2 = Random.Range(minValue, Mathf.Abs(minValue));
                if(e_term1 == 0 || e_term2 == 0 || Mathf.Abs(e_term2) > Mathf.Abs(e_term1) || (Mathf.Abs(e_term1) == Mathf.Abs(e_term2) && level >= 5))
                {
                    remainder = -1;
                }
                else
                {
                    e_operator = "/";
                    float e_double_answer = (float) e_term1 / (float) e_term2;
                    e_answer = e_term1 / e_term2;
                    if(e_double_answer % 1 == 0 && Mathf.Abs(e_term2) != 1)
                    {
                        remainder = 0;
                    }
                    else
                    {
                        remainder = -1;
                    }
                }

            }
        }
    }
}
