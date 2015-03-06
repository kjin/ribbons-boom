using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CodeLibrary.Graphics;
using CodeLibrary.Storage;
using CodeLibrary.Content;

namespace CodeLibrary.Context
{
    public class FileOption : Option
    {
        Texture2D backdrop;
        Texture2D[] previews;
        Texture2D grid;
        Texture2D bow;

        SpriteFont wordFont;
        SpriteFont collectablesFont;

        PlayerFile pf;

        float scale;

        int fileNumber;
        int actCompleted;
        int levelCompleted;
        int numLevels;
        float percentDone;
        int numCollectables;

        Vector2 gridPosition;
        Vector2 textPosition;

        public FileOption(PlayerFile pf)
        {
            this.pf = pf;
            UpdateStats();
            scale = 0.5f * GraphicsConstants.VIEWPORT_HEIGHT / 720f;
            gridPosition = scale * new Vector2(0, 280);
        }

        private void UpdateStats()
        {
            fileNumber = pf.ID;
            percentDone = pf.Progress;
            actCompleted = pf.FarthestAct;
            levelCompleted = pf.FarthestLevel;
            numLevels = actCompleted * 10 + levelCompleted;
            if (actCompleted > 0)
                numLevels -= 4;
            numCollectables = pf.NumCollectables;
        }

        public override void Initialize(Canvas canvas, string themeName)
        {
            bow = canvas.Assets.GetTexture("Collectables/collect0");
            wordFont = canvas.Assets.GetFont("Conformity24");
            collectablesFont = canvas.Assets.GetFont("CollectableCountFont");
            backdrop = canvas.Assets.GetTexture("file_back");
            textPosition = scale * new Vector2(0, 500);
            grid = canvas.Assets.GetTexture("Previews/preview_grid");
            previews = new Texture2D[4];
            for (int i = 0; i < previews.Length; i++)
                previews[i] = canvas.Assets.GetTexture("Previews/preview_act" + i);
            dimensions = scale * new Vector2(backdrop.Width, backdrop.Height);
            base.Initialize(canvas, themeName);
        }

        public override void Update(bool active)
        {
            if (active)
                UpdateStats();
            base.Update(active);
        }

        public override void Draw(Canvas canvas, Vector2 selectorPosition)
        {
            base.Draw(canvas, selectorPosition);
            canvas.DrawTexture(backdrop, new Color(0,0,0,0.5f), position + selectorPosition, 0f, scale);
            Vector2 drawPosition = position + selectorPosition + new Vector2(250, 0) * scale;
            canvas.DrawTexture(previews[0], percentDone > 0 && actCompleted >= 0 ? Color.White : Color.Gray, drawPosition + gridPosition, Anchor.BottomRight, 0, scale);
            canvas.DrawTexture(previews[1], actCompleted >= 1 ? Color.White : Color.Gray, drawPosition + gridPosition, Anchor.BottomLeft, 0, scale);
            canvas.DrawTexture(previews[2], actCompleted >= 2 ? Color.White : Color.Gray, drawPosition + gridPosition, Anchor.TopRight, 0, scale);
            canvas.DrawTexture(previews[3], actCompleted >= 3 ? Color.White : Color.Gray, drawPosition + gridPosition, Anchor.TopLeft, 0, scale);
            canvas.DrawTexture(grid, Color.White, drawPosition + gridPosition, Anchor.Center, 0, scale);
            canvas.DrawString("FILE " + fileNumber, wordFont, Color.White, drawPosition + new Vector2(0, 5 * scale), Anchor.TopCenter);
            if (percentDone > 0)
            {
                //percent done
                canvas.DrawString(String.Format("{0:p} DONE", percentDone), wordFont, Color.White, drawPosition + textPosition, Anchor.TopCenter);
                //num bows
                canvas.DrawTexture(bow, Color.White, drawPosition + textPosition + scale * new Vector2(0, 180), Anchor.CenterRight, 0, scale);
                canvas.DrawString(numCollectables, collectablesFont, Color.White, drawPosition + textPosition + scale * new Vector2(60, 180), Anchor.Center);
            }
            else
                canvas.DrawString("New File", wordFont, Color.White, drawPosition + textPosition, Anchor.TopCenter);
        }
    }
}
