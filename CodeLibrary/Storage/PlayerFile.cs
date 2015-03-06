using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeLibrary.Content;

namespace CodeLibrary.Storage
{
    public class PlayerFile
    {
        private int id;
        private int farthestAct;
        private int farthestLevel;
        private int numCollectables;
        private float progress;
        private string filePath;
        private List<LevelInfo> levelsStatus;
        AssetManager assetManager;

        static int WORLDS = 4;
        public static int LEVELS_PER_WORLD = 10;
        static int COLLECTABLES_PER_LEVEL = 3;

        /// <summary>
        /// Checks to see if there is a file that exists with the corresponding playerID.  If there isn't, it creates a blank one based off of empty levels.
        /// </summary>
        /// <param name="id">
        /// This is the identity of the player file.  should take the values of 0,1, and 2.
        /// </param>
        public PlayerFile(AssetManager assetManager, int id)
        {
            this.assetManager = assetManager;
            this.id = id;
            levelsStatus = new List<LevelInfo>();
            filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Wet Floor Studios\\Ribbons\\Player" + id + ".ribbons";
        }

        public int ID
        {
            get { return id; }
        }

        public int FarthestLevel
        {
            get { return farthestLevel; }
            set { farthestLevel = value; }
        }

        public int FarthestAct
        {
            get { return farthestAct; }
            set { farthestAct = value; }
        }

        public int NumCollectables
        {
            get { return numCollectables; }
        }

        public float Progress
        {
            get { return progress; }
        }

        public void Clear()
        {
            farthestAct = numCollectables = 0;
            farthestLevel = 0;
            levelsStatus.Clear();
            List<string> levels = assetManager.GetLevelList();
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Substring(0, 5) == "Level")
                {
                    string nums = levels[i].Substring(5);
                    string[] ActLevel = nums.Split('_');
                    int act = Convert.ToInt32(ActLevel[0]);
                    int level = Convert.ToInt32(ActLevel[1]);
                    levelsStatus.Add(new LevelInfo(act, level));
                }
            }
            progress = 0;
        }

        /// <summary>
        /// Creates all of the levelInfo objects with the parameters read from the file.  
        /// Stores them in a list inside the PlayerFile object.
        /// </summary>
        public void Load()
        {
            string line;
            if (!File.Exists(filePath))
            {
                Clear();
                return;
            }
            System.IO.StreamReader file = new System.IO.StreamReader(filePath);
            int tutorials = 0;
            int levels = 0;
            int tutorialsCompleted = 0;
            int levelsCompleted = 0;
            while((line = file.ReadLine()) != null)
            {
                if(line.Substring(0,1) == "L")
                {
                    string[] str = line.Split(' ');
                    int actID = Convert.ToInt32(str[1]);
                    if (actID == 0) tutorials++;
                    else levels++;
                    int levelID = Convert.ToInt32(str[2]);
                    bool completed = str[3] == "1";
                    int collectables = Convert.ToInt32(str[4]);
                    int completionTime = Convert.ToInt32(str[5]);
                    levelsStatus.Add(new LevelInfo(actID, levelID, completed, collectables, completionTime));
                    if(completed)
                    {
                        farthestAct = actID;
                        farthestLevel = levelID;
                    }
                    for (int c = 0; c < 32; c++)
                        if ((collectables & (1 << c)) != 0)
                            numCollectables++;
                    
                    if (actID == 0) { tutorials++; if (completed) tutorialsCompleted++; }
                    else { levels++; if (completed) levelsCompleted++; }
                }
            }
            file.Close();
            progress = 2 * (float)(5 * tutorialsCompleted + 2 * levelsCompleted + numCollectables) / (5 * tutorials + 5 * levels);
        }

        public LevelInfo GetLevelInfo(int act, int level)
        {
            foreach (LevelInfo l in levelsStatus)
            {
                if(l.ActID == act && l.LevelID == level)
                {
                    return l.Clone();
                }
            }
            return new LevelInfo(act, level);
        }

        public void SetLevelInfo(LevelInfo li)
        {
            LevelInfo levelInfo = li.Clone();
            levelInfo.ClearTemp();
            int act = levelInfo.ActID;
            int level = levelInfo.LevelID;
            for(int i = 0; i<levelsStatus.Count; i++)
            {
                if (levelsStatus[i].ActID == act && levelsStatus[i].LevelID == level)
                {
                    levelsStatus[i] = levelInfo;
                    Console.WriteLine("Level Info Set: " + act + " " + level);
                    return;
                }
            }
            levelsStatus.Add(levelInfo);
        }

        /// <summary>
        /// Takes level info (that has been updated by playing the game) and writes it to the text file.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath.Substring(0, filePath.LastIndexOf('\\')));
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine("P " + id);
                for(int i = 0; i<levelsStatus.Count; i++)
                {
                    LevelInfo temp = levelsStatus[i];
                    int tempComplete;
                    if(temp.Complete)
                    {
                        tempComplete = 1;
                    }
                    else
                    {
                        tempComplete = 0;
                    }
                    sw.WriteLine("L " + temp.ActID + " " + temp.LevelID + " " + tempComplete + " " + temp.Collectables + " " + temp.CompletionTime);
                    if(temp.Complete)
                    {
                        farthestAct = temp.ActID;
                        farthestLevel = temp.LevelID;
                    }
                }
            }
            Console.WriteLine("Saved Game.");
        }
    }
}
