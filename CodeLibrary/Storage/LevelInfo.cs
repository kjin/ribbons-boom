using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeLibrary.Storage
{
    public class LevelInfo
    {
        //persisting
        private int actID;
        private int levelID;
        private bool complete;
        private int completionTime;
        private int collectables;
        //temp stuff
        private int collectablesThisRound;
        private int checkpointIndex;
        private int ribbonGems;
        private int spawnID;
        private string levelName;
        private bool displayTime;
        private int saveTime;
        private int currentTime;

        public const int MAX_TIME = 5999999;

        public LevelInfo(int actID, int id, bool complete = false, int col = 0, int compTime = MAX_TIME, bool displayTime = true)
        {
            this.actID = actID;
            levelID = id;
            this.complete = complete;
            collectables = col;
            completionTime = compTime;
            levelName = "Level" + actID + "_" + levelID;
            this.displayTime = displayTime;
            ClearTemp();
        }

        public LevelInfo Clone()
        {
            LevelInfo ret = new LevelInfo(actID, levelID, complete, collectables, completionTime);
            ret.collectablesThisRound = collectablesThisRound;
            ret.checkpointIndex = checkpointIndex;
            ret.ribbonGems = ribbonGems;
            ret.spawnID = spawnID;
            return ret;
        }

        public void ClearTemp()
        {
            collectablesThisRound = checkpointIndex = ribbonGems = spawnID = 0;
        }

        public int ActID
        {
            get { return actID; }
        }

        public int LevelID
        {
            get { return levelID; }
        }

        public int SpawnID
        {
            get { return spawnID; }
            set { spawnID = value; }
        }

        public bool Complete
        {
            get { return complete; }
            set { complete = value; }
        }

        public int Collectables
        {
            get { return collectables; }
            set { collectables = value; Console.WriteLine("Collectables Changed:" + collectables); }
        }

        public int CollectablesThisRound
        {
            get { return collectablesThisRound; }
            set { collectablesThisRound = value; }
        }

        public int CompletionTime
        {
            get { return completionTime; }
            set { completionTime = value; }
        }

        public int CheckpointIndex
        {
            get { return checkpointIndex; }
            set { checkpointIndex = value; }
        }

        public int RibbonGems
        {
            get { return ribbonGems; }
            set { ribbonGems = value; }
        }

        public string LevelName
        {
            get { return levelName; }
        }

        public bool DisplayTime
        {
            get { return displayTime; }
            set { displayTime = value; }
        }

        public int CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        public int SaveTime
        {
            get { return saveTime; }
            set { saveTime = value; }
        }
    }
}
