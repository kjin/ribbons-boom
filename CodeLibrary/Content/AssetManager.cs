using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using XMLContent;
using CodeLibrary.Graphics;

namespace CodeLibrary.Content
{
    /// <summary>
    /// Stores all importable content.
    /// </summary>
    public class AssetManager
    {
        class AssetCollection<T> where T : class
        {
            string directory;
            Dictionary<string, T> assets;
            List<string> assetNames;

            public AssetCollection(string directory)
            {
                this.directory = directory;
                assets = new Dictionary<string, T>();
                assetNames = new List<string>();
            }

            public void AddAsset(string identifier, T asset)
            {
                assets[identifier] = asset;
                assetNames.Add(identifier);
            }

            public T GetAsset(string identifier)
            {
                T asset;
                bool success = assets.TryGetValue(identifier, out asset);
                if (!success)
                    Console.WriteLine("Warning: {0} {1} not found. The game may crash.", typeof(T), identifier);
                return success ? asset : null;
            }

            public string Directory { get { return directory; } }

            public List<string> GetAssetNames()
            {
                return new List<string>(assetNames);
            }
        }

        AssetCollection<SoundEffect> sounds;
        AssetCollection<Song> songs;
        AssetCollection<Texture2D> textures;
        AssetCollection<Texture2D> animations;
        AssetCollection<Texture2D> masks;
        AssetCollection<Texture2D[]> miasmaAnimations;
        AssetCollection<SpriteFont> fonts;
        AssetCollection<Effect> effects;
        AssetCollection<Text> text;
        AssetCollection<Level> levels;

        int NUM_MIASMA_FRAMES = 120;

        /// <summary>
        /// Constructs a new instance of AssetManager.
        /// </summary>
        public AssetManager()
        {
            sounds = new AssetCollection<SoundEffect>("SFX");
            songs = new AssetCollection<Song>("Music");
            textures = new AssetCollection<Texture2D>("Textures");
            animations = new AssetCollection<Texture2D>("Animations");
            masks = new AssetCollection<Texture2D>("Masks");
            miasmaAnimations = new AssetCollection<Texture2D[]>("Animations");
            effects = new AssetCollection<Effect>("Effects");
            fonts = new AssetCollection<SpriteFont>("Fonts");
            text = new AssetCollection<Text>("Text");
            levels = new AssetCollection<Level>("Levels");
        }

        /// <summary>
        /// Scans the content project folder for assets, and loads them.
        /// </summary>
        /// <param name="content">The ContentManager object associated with the game.</param>
        public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
        {
            Console.Write("Loading all assets... ");
            DateTime before = DateTime.Now;
            try
            {
                LoadNormalContent<SoundEffect>(content, sounds);
                LoadNormalContent<Song>(content, songs);
            }
            catch
            {
                Console.WriteLine("Warning: The audio device is unavailable... no sounds will be loaded.");
            }
            LoadNormalContent<Texture2D>(content, textures);
            LoadNormalContent<Texture2D>(content, animations);
            LoadNormalContent<Texture2D>(content, masks);
            LoadNormalContent<Effect>(content, effects);
            LoadNormalContent<SpriteFont>(content, fonts);
            LoadNormalContent<Text>(content, text);
            LoadNormalContent<Level>(content, levels);

            LoadMiasmaAnimations(graphicsDevice);

            DateTime after = DateTime.Now;
            Console.WriteLine("loaded in {0} seconds.", TimeSpan.FromTicks(after.Ticks - before.Ticks).TotalSeconds);
            //load sprites
            /*string[] spriteFiles = GetNames<Sprite>(sprites);
            foreach (string item in spriteFiles)
            {
                Texture2D texture = content.Load<Texture2D>(item);
                Sprite sprite = new Sprite(texture);
                sprites.AddAsset(item.Substring(sprites.Directory.Length + 1), sprite);
            }*/
        }

        private static string[] GetNames<T>(AssetCollection<T> collection) where T : class
        {
            string dir = ".\\Content\\";
            if (!Directory.Exists(dir + collection.Directory))
                return new string[0];
            string[] files = Directory.GetFiles(dir + collection.Directory, "*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                int hyphen = files[i].IndexOf('-');
                if (hyphen > 10) //this is the length of string .\\Content\\
                    files[i] = files[i].Substring(dir.Length, hyphen);
                else
                    files[i] = files[i].Substring(dir.Length, files[i].Length - dir.Length - 4);
                files[i] = files[i].Replace('\\', '/');
            }
            return files;
        }

        private static void LoadNormalContent<T>(ContentManager content, AssetCollection<T> collection) where T : class
        {
            string[] items = GetNames<T>(collection);
            foreach (string item in items)
                collection.AddAsset(item.Substring(collection.Directory.Length + 1), content.Load<T>(item));
        }

        private void LoadMiasmaAnimations(GraphicsDevice graphicsDevice)
        {
            string[] maskNames = new string[16]{
                "miasmaNoMask",                     // 0
                "miasmaTopMask",                    // 1
                "miasmaBottomMask",                 // 2
                "miasmaLeftMask",                   // 3
                "miasmaRightMask",                  // 4
                "miasmaTopLeftMask",                // 5
                "miasmaTopRightMask",               // 6
                "miasmaTopBottomMask",              // 7
                "miasmaBottomLeftMask",             // 8
                "miasmaBottomRightMask",            // 9
                "miasmaLeftRightMask",              // 10
                "miasmaTopLeftRightMask",           // 11
                "miasmaTopLeftBottomMask",          // 12
                "miasmaTopBottomRightMask",         // 13
                "miasmaLeftRightBottomMask",        // 14
                "miasmaTopLeftRightBottomMask"      // 15
            };

            LoadMiasmaAnimationPanel(graphicsDevice, maskNames);
        }

        // pass in "miasmaNoMask" for no mask applied
        private void LoadMiasmaAnimationPanel(GraphicsDevice graphicsDevice, string[] names)
        {
            for (int k = 0; k < names.Length; k++){
                Texture2D[] maskedFrames = new Texture2D[NUM_MIASMA_FRAMES];
                for (int i = 0; i < NUM_MIASMA_FRAMES; i++)
                {
                    string texNum = "000000";
                    if (i < 10)
                        texNum = "00000" + i;
                    else if (i < 100)
                        texNum = "0000" + i;
                    else
                        texNum = "000" + i;

                    Texture2D texture = animations.GetAsset("miasmaAnim_" + texNum);
                    Texture2D mask = masks.GetAsset("miasmaTopMask_" + texNum);
                    Texture2D newTexture = new Texture2D(graphicsDevice, texture.Width, texture.Height);
                    Color[] colors = ApplyMaskToMiasmaAnimation(k, texture, mask);
                    newTexture.SetData<Color>(colors);
                    maskedFrames[i] = newTexture;
                }
                miasmaAnimations.AddAsset(names[k], maskedFrames);
                }
        }

        private Color[] ApplyMaskToMiasmaAnimation(int id, Texture2D texture, Texture2D mask)
        {
            Color[,] textureColors = TextureTo2DArray(texture);
            Color[,] maskColors = TextureTo2DArray(mask);

            for (int i = 0; i < texture.Width; i++)
                for (int j = 0; j < texture.Height; j++)
                {
                    List<Vector2> textureColorsToMask = GetTextureColorsToMask(id, textureColors, i, j);
                    foreach (Vector2 color in textureColorsToMask)
                    {
                        if (maskColors[i, j].A < textureColors[(int)color.X, (int)color.Y].A)
                        {
                            textureColors[(int)color.X, (int)color.Y].A = maskColors[i, j].A;
                        }

                        if (textureColors[(int)color.X, (int)color.Y].A > 200)
                           textureColors[(int)color.X, (int)color.Y].A = 200;
                    }
                }

            return Colors2DTo1D(textureColors);
        }

        private List<Vector2> GetTextureColorsToMask(int id, Color[,] textureColors, int i, int j)
        {
            List<Vector2> output = new List<Vector2>();

            Vector2 top = new Vector2(i, j);
            Vector2 bottom = new Vector2(textureColors.GetLength(0) - 1 - i, textureColors.GetLength(1) - 1 - j);
            Vector2 right = new Vector2(textureColors.GetLength(0) - 1 - j, textureColors.GetLength(1) - 1 - i);
            Vector2 left = new Vector2(j, i);

            switch (id)
            {
                case 0:  // none
                    break;
                case 1:  // top
                    output.Add(top);
                    break;
                case 2:  // bottom
                    output.Add(bottom);
                    break;
                case 3:  // left
                    output.Add(left);
                    break;
                case 4:  // right
                    output.Add(right);
                    break;
                case 5: // top left
                    output.Add(top);
                    output.Add(left);
                    break;
                case 6: // top right
                    output.Add(top);
                    output.Add(right);
                    break;
                case 7: // top bottom
                    output.Add(top);
                    output.Add(bottom);
                    break;
                case 8: // bottom left
                    output.Add(bottom);
                    output.Add(left);
                    break;
                case 9: // bottom right
                    output.Add(bottom);
                    output.Add(right);
                    break;
                case 10: // left right
                    output.Add(left);
                    output.Add(right);
                    break;
                case 11: // top left right
                    output.Add(top);
                    output.Add(left);
                    output.Add(right);
                    break;
                case 12: // top left bottom
                    output.Add(top);
                    output.Add(left);
                    output.Add(bottom);
                    break;
                case 13: // top bottom right
                    output.Add(top);
                    output.Add(bottom);
                    output.Add(right);
                    break;
                case 14: // left right bottom
                    output.Add(left);
                    output.Add(right);
                    output.Add(bottom);
                    break;
                case 15: // top left right bottom
                    output.Add(top);
                    output.Add(left);
                    output.Add(right);
                    output.Add(bottom);
                    break;
                default:
                    break;
            }
            return output;
        }

        /// <summary>
        /// Gets an effect based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Effect GetEffect(string assetName) { return effects.GetAsset(assetName); }
        /// <summary>
        /// Gets a font based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public SpriteFont GetFont(string assetName) { return fonts.GetAsset(assetName); }
        /// <summary>
        /// Gets a level based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Level GetLevel(string assetName) { return levels.GetAsset(assetName); }
        /// <summary>
        /// Gets a sound effect based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public SoundEffect GetSFX(string assetName) { return sounds.GetAsset(assetName); }
        /// <summary>
        /// Gets a song based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Song GetSong(string assetName) { return songs.GetAsset(assetName); }
        /// <summary>
        /// Gets a text object based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Text GetText(string assetName) { return text.GetAsset(assetName); }
        /// <summary>
        /// Gets a texture based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Texture2D GetTexture(string assetName) { return textures.GetAsset(assetName); }

        /// <summary>
        /// Gets an array of animation frames for a block of miasma with a specified mask applied based on its asset name.
        /// </summary>
        /// <param name="assetName">The name of the asset. This typically excludes the .* extension as well as the directory.</param>
        /// <returns>The asset associated with the asset name provided.</returns>
        public Texture2D[] GetMiasmaAnimation(string assetName) { return miasmaAnimations.GetAsset(assetName); }

        public Texture2D GetAnimation(string assetName) { return animations.GetAsset(assetName); }

        /// <summary>
        /// Gets a list with the names of every loaded level.
        /// </summary>
        /// <returns>A list with the names of every loaded level.</returns>
        public List<string> GetLevelList() { return levels.GetAssetNames(); }


        //helper functions for creating miasma animations
        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public static Color[] Colors2DTo1D(Color[,] colors2d)
        {
            Color[] colors1d = new Color[colors2d.GetLength(0) * colors2d.GetLength(1)];
            for (int i = 0; i < colors2d.GetLength(0); i++)
            {
                for (int j = 0; j < colors2d.GetLength(1); j++)
                    colors1d[i + j * colors2d.GetLength(0)] = colors2d[i, j];
            }
            return colors1d;
        }
    }
}
