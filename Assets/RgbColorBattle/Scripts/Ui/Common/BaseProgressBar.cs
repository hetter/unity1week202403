using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class BaseProgressBar : MonoBehaviour
{
    [SerializeField] TMP_Text txt_now_value;
    [SerializeField] TMP_Text txt_max_value;
    [SerializeField] UnityEngine.UI.Image img_bar;

    public float LimitValue { get;  set; }
    public float NowValue { get;  set; }

    // public float LimitValue;
    // public float NowValue;

    public void UpdateNowValue()
    {
        var fm = NowValue / LimitValue;
        img_bar.fillAmount = fm;

        txt_now_value.text = string.Format("{0}", (int)NowValue);
        txt_max_value.text = string.Format("{0}", (int)LimitValue);
    }
}
