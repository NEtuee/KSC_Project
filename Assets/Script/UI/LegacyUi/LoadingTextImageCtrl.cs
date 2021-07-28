using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextImageCtrl : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    private bool _active = false;
    private float _term = 0.5f;

    [SerializeField] private List<Sprite> loadingCycleSprites = new List<Sprite>();
    private int _currentNum = 0;

    public void Active(bool active)
    {
        _active = active;
        if (active)
        {
            _currentNum = 0;
            targetImage.sprite = loadingCycleSprites[_currentNum];
            StartCoroutine(SpriteCycle());
        }
    }

    IEnumerator SpriteCycle()
    {
        while(_active)
        {
            yield return CoroutineUtilities.WaitForRealTime(_term);
            _currentNum++;
            if (_currentNum >= loadingCycleSprites.Count)
                _currentNum = 0;
            targetImage.sprite = loadingCycleSprites[_currentNum];
        }
    }
}
