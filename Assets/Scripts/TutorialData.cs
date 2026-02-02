using System;

[Serializable]
public class TutorialStep
{
    public string text;
    public string imagePath;
}

[Serializable]
public class TutorialData
{
    public TutorialStep[] steps;
}
