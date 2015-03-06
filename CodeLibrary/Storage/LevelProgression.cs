using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Content;
using CodeLibrary.Context;

namespace CodeLibrary.Storage
{
    public enum Contexts
    {
        Level, Cutscene, Splash
    }

    public class ContextInfo
    {
        public Contexts ContextType;
        public string ContextName;
    }

    public class LevelProgression
    {
        Dictionary<string, ContextInfo> progression;
        FileManager fileManager;

        public LevelProgression(FileManager fileManager, Text levelList)
        {
            this.fileManager = fileManager;
            progression = new Dictionary<string, ContextInfo>();
            for (int i = 2; i < levelList.Length; i += 2)
            {
                ContextInfo contextInfo = new ContextInfo();
                if (levelList[i] == "level")
                    contextInfo.ContextType = Contexts.Level;
                else if (levelList[i] == "cutscene")
                    contextInfo.ContextType = Contexts.Cutscene;
                else if (levelList[i] == "splash")
                    contextInfo.ContextType = Contexts.Splash;
                if (contextInfo.ContextType != Contexts.Splash)
                    contextInfo.ContextName = levelList[i + 1];
                progression[levelList[i - 1]] = contextInfo;
            }
        }

        public GameContext GetNextContext(GameContext self, string currentLevel)
        {
            if (!progression.ContainsKey(currentLevel))
                return new LevelSelectContext(self);
            ContextInfo nextContext = progression[currentLevel];
            if (nextContext.ContextType == Contexts.Cutscene)
                return new CutsceneContext(nextContext.ContextName, self);
            else if (nextContext.ContextType == Contexts.Level)
            {
                int actNumber = StorageHelper.GetActNumber(nextContext.ContextName);
                int levelNumber = StorageHelper.GetLevelNumber(nextContext.ContextName);
                return new LevelContext(fileManager.ActiveFile.GetLevelInfo(actNumber, levelNumber), self);
            }
            return new CreditsContext(self);
        }
    }
}
