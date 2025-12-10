using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Video Parts")]
    public GameObject part1;
    public GameObject part2;
    public GameObject part3;

    [Header("Video Players")]
    public VideoPlayer videoPlayerPart1;
    public VideoPlayer videoPlayerPart2;
    public VideoPlayer videoPlayerPart3;

    [Header("Slider")]
    public Slider video2Slider;

    [Header("Scene Management")]
    public string nextScene;

    void Start()
    {
        part1.SetActive(true);
        part2.SetActive(false);
        part3.SetActive(false);

        video2Slider.minValue = 0;
        video2Slider.maxValue = 1;
        video2Slider.value = 0;

        video2Slider.onValueChanged.AddListener(OnSliderDrag);

        videoPlayerPart1.loopPointReached += OnVideo1Finished;

        videoPlayerPart3.loopPointReached += OnVideo3Finished;
    }

    void OnVideo1Finished(VideoPlayer vp)
    {
        part1.SetActive(false);

        part2.SetActive(true);

        videoPlayerPart2.Play();
        videoPlayerPart2.Pause();
        videoPlayerPart2.time = 0;
    }

    public void OnSliderDrag(float value)
    {
        if (part2.activeSelf)
        {
            videoPlayerPart2.time = videoPlayerPart2.length * value;

            videoPlayerPart2.Pause();

            if (value >= 0.99f)
            {
                StartVideo3();
            }
        }
    }

    void StartVideo3()
    {
        if (part3.activeSelf) return;

        video2Slider.onValueChanged.RemoveListener(OnSliderDrag);

        part2.SetActive(false);

        part3.SetActive(true);

        videoPlayerPart3.Play();
    }

    void OnVideo3Finished(VideoPlayer vp)
    {
        Debug.Log("Video 3 fertig. Lade Quiz-Szene.");
        SceneManager.LoadScene(nextScene);
    }
}