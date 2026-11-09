namespace ReverseSolver.Core
{
    public static class IntroPlan
    {
        public static bool ShowOpening(string introParam, bool debug, bool showSet, bool playOverride,
            bool hasLvParam, bool hasSetParam, bool pt1Cohort, bool redesignSet, bool alreadyShown, bool hasProgress) =>
            introParam == "0" ? false : introParam == "1" ||
            (!debug && !showSet && !playOverride && !hasLvParam && !hasSetParam && !pt1Cohort && redesignSet && !alreadyShown && !hasProgress);

        public static bool EndingEligible(string introParam, bool debug, bool showSet, bool playOverride,
            bool hasLvParam, bool hasSetParam, bool pt1Cohort, bool redesignSet, bool endingShown, bool firstLevelWonAtLoad) =>
            introParam == "0" ? false : introParam == "1" ||
            (!debug && !showSet && !playOverride && !hasLvParam && !hasSetParam && !pt1Cohort && redesignSet && !endingShown && !firstLevelWonAtLoad);
    }
}
