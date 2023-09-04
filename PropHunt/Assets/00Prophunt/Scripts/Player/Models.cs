using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Models 
{
    [Serializable]

    public class PlayerSettingsModel
    {
        [Header("ViewSetting")]
        public float viewXSensitivity;
        public float viewYSensitivity;

        public bool viewXInverted;
        public bool viewYInverted;
    }
}
