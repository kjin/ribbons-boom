using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeLibrary.Content;

namespace CodeLibrary.Storage
{
    public class FileManager
    {
        private List<PlayerFile> listOfPlayers;
        private PlayerFile activeFile;
        private OptionsFile optionsFile;
        private AssetManager assetManager;
        private LevelProgression levelProgression;
        private string[] levelTitles;

        public FileManager(AssetManager assetManager)
        {
            listOfPlayers = new List<PlayerFile>();
            this.assetManager = assetManager;
            levelProgression = new LevelProgression(this, assetManager.GetText("levelprogression"));
            Text titles = assetManager.GetText("leveltitles");
            levelTitles = new string[titles.Length];
            for (int i = 0; i < levelTitles.Length; i++)
                levelTitles[i] = titles[i];
        }

        public PlayerFile ActiveFile
        {
            get { return activeFile; }
            set { activeFile = value; }
        }

        public LevelProgression LevelProgression { get { return levelProgression; } }

        public string GetLevelTitle(int act, int level)
        {
            try
            {
                return levelTitles[act * PlayerFile.LEVELS_PER_WORLD + level - 1];
            }
            catch
            {
                return levelTitles[0];
            }
        }

        public void SetOptionsFile(OptionsFile of)
        {
            optionsFile = of;
        }

        public PlayerFile NewPlayerFile(int id)
        {
            PlayerFile pf = new PlayerFile(assetManager, id);
            pf.Clear();
            return pf;
        }

        public PlayerFile LoadPlayerFile(int id)
        {
            PlayerFile pf = new PlayerFile(assetManager, id);
            pf.Load();
            return pf;
        }

        public PlayerFile SavePlayerFile(int id)
        {
            PlayerFile pf = new PlayerFile(assetManager, id);
            pf.Save();
            return pf;
        }
    }
}
