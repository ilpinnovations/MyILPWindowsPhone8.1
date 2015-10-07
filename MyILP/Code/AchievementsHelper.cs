using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyILP.Code
{
    class AchievementsHelper
    {
        private static string[] astrings = { "Beginner", "Karma Warrior", "Karma Empower", "Karma Leader", "Karma King" };
        private static int[] levelVector = { 15, 30, 60, 100, 999 };
        public static string GetAchievementString(int points)
        {
            int level = GetLevel(points);
            if (level < 0 || level >= levelVector.Length)
                throw new Exception("Invalid level being selected");

            return astrings[level];
        }
        public static int GetLevel(int points)
        {
            int index = -1;
            for (index = 0; index < levelVector.Length; ++index)
            {
                if (levelVector[index] > points)
                    break;
            }
            if (index == levelVector.Length)
                index = levelVector.Length - 1;

            return index;
        }
    }
}
