using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DummyEgg.ProjectGK.Model;

public class ElementShield : MonoBehaviour
{
    [SerializeField] GameObject[] RgbShields;

    public float ChangeTime = 3;

    // Start is called before the first frame update

    public void ChangeShield(HeroModel.ELE_TYPE eleType)
    {
        foreach(var s in RgbShields)
        {
            s.SetActive(false);
        }

        RgbShields[(int)eleType].SetActive(true);
    }
}
