﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRulesController : MonoBehaviour {

    public void Go()
    {
        SceneManager.LoadScene("GamePlay");
    }
}