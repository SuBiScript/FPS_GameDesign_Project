﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

[System.Serializable]
public class TutorialChange : UnityEvent<string, PlayerDetector.ColorPanel, VideoClip>
{
}
