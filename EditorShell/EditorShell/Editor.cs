using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CodeLibrary;
using CodeLibrary.Audio;
using CodeLibrary.Content;
using CodeLibrary.Context;
using CodeLibrary.Engine;
using CodeLibrary.Graphics;
using CodeLibrary.Input;

using EditorUILibrary.Controls;
using EditorUILibrary.Panels;
using EditorUILibrary.UI;
using EditorUILibrary;
using XMLContent;

namespace EditorShell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Editor : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        Control mainWindowControl;
        GameEngine game;
        UIModel ui;
        int currentMode;
        string objectSelector;
        KeyboardState oldKeyboard;
        string key;
        int keyTimer;
        string oldSelection;
        bool resetGrid;
        #endregion

        public Editor()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;       
            Content.RootDirectory = "Content";
            
            IsFixedTimeStep = true;
            this.IsMouseVisible = true;

            mainWindowControl = Form.FromHandle(this.Window.Handle);
            mainWindowControl.MouseClick += MouseClick;
            ui = new UIModel(mainWindowControl, this.Window);
            objectSelector = "";
            System.Windows.Forms.Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
            resetGrid = false;
        }

        private void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs args)
        {
            args.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = ui.GameWindowPanel.Handle;
        }

        private void MouseClick(Object sender, MouseEventArgs e)
        {
            MessageBox.Show("test");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ui.Initialize();
            ui.GameWindowPanel.Clicked = false;
            game = new GameEngine(graphics, Content, GraphicsDevice);
            game.Initialize();
            SwitchContext(1);
            base.Initialize();
            keyTimer = 1;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ui.LoadContent();
            game.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //game.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            objectSelector = ui.BottomControlPanel.ObjectSelected;
            if (objectSelector != oldSelection)
            {
                ui.BottomControlPanel.PopulateDataGrid(objectSelector);
            }
            ui.GameWindowPanel.CurrentContext = currentMode;
            KeyboardState keyState = Keyboard.GetState();
            if (ui.MenuBarPanel.LoadLevel)
            {
                Console.WriteLine(ui.MenuBarPanel.SavePath);
                string[] split = ui.MenuBarPanel.SavePath.Split('\\');
                string l = split[split.Length - 1].Split('.')[0];
                Console.WriteLine(l);
                Level level = game.GetLevel(l);
                game.ChangeEditorContext(1, level);
                ui.MenuBarPanel.LoadLevel = false;
            }
            if (ui.MenuBarPanel.GenerateLevel)
            {
                XMLGenerator.ToXML(CreateLevel(), ui.MenuBarPanel.SavePath);
                ui.MenuBarPanel.GenerateLevel = false;
            }
            if (keyTimer >0)
            {
                keyTimer--;
            }
            else
            {
                keyTimer = 0;
                if (KeyboardHandling.TryConvertKeyboardInput(keyState, oldKeyboard, out key))
                {
                    ui.RightControlPanel.UpdateTitleText(key);
                    ui.BottomControlPanel.UpdateTitleText(key, (int)game.currentContext.GridWidth, (int)game.currentContext.GridHeight);
                }
            }
            if (currentMode == 1)
            {
                EditorBuildContext curr = game.currentContext as EditorBuildContext;
                if (curr != null)
                {
                    if (curr.ResetGrid)
                    {
                        Console.WriteLine((int)curr.GridWidth);
                        ui.RightControlPanel.setGridWidth((int)curr.GridWidth);
                        ui.RightControlPanel.setGridHeight((int)curr.GridHeight);
                        curr.ResetGrid = false;
                    }
                    curr.RibbonOnly = ui.BottomControlPanel.OnlyRibbon;
                    if (curr.BuildingRibbon != null)
                    {
                        ui.BottomControlPanel.moveToRibbon(curr.MoveToRibbon);
                        curr.BuildingRibbon.MoveToRibbon = false;
                    }

                    if (ui.BottomControlPanel.LoopButton || ui.BottomControlPanel.TerminateButton)
                    {
                        curr.RibbonLoop = ui.BottomControlPanel.RibbonLooped;
                        ui.BottomControlPanel.LoopButton = false;
                        ui.BottomControlPanel.TerminateButton = false;
                    }

                    if (ui.RightControlPanel.EditPressed)
                    {
                        if (curr.labelSelected())
                        {
                            curr.selectRibbon();
                            ui.BottomControlPanel.showRibbonEdit();
                        }
                        ui.RightControlPanel.EditPressed = false;
                    }

                    if (ui.BottomControlPanel.AnchorPressed)
                    {
                        if (curr.ribbonObjectSelected() && curr.ribbonSelected())
                        {
                            Console.WriteLine("ribbon object selected");
                        }
                        ui.BottomControlPanel.AnchorPressed = false;
                    }

                    if (ui.BottomControlPanel.RibbonCancel)
                    {
                        if (curr.BuildingRibbon != null)
                        {
                            curr.BuildingRibbon.RemoveBodies();
                            curr.BuildingRibbon = null;
                        }
                        if (curr.Ribbon != null)
                        {
                            curr.Ribbon.RemoveBodies();
                            curr.Ribbon = null;
                        }
                        ui.BottomControlPanel.RibbonCancel = false;
                    }

                    if (ui.BottomControlPanel.BackButton)
                    {
                        if (curr.Ribbon != null)
                        {
                            curr.Ribbon.RemoveBodies();
                            curr.Ribbon = null;
                        }
                        curr.BuildingRibbon.RibbonEnd.body.Dispose();
                        curr.BuildingRibbon.RibbonEnd = null;
                        curr.BuildingRibbon.RibbonStart.body.Dispose();
                        curr.BuildingRibbon.RibbonStart = null;
                        ui.BottomControlPanel.BackButton = false;
                    }

                    if (ui.BottomControlPanel.CreateButton)
                    {
                        curr.createRibbon();
                        ui.BottomControlPanel.CreateButton = false;
                    }

                    if (ui.BottomControlPanel.AddRibbonPoints && curr.BuildingRibbon != null && curr.BuildingRibbon.PathStart != null && curr.BuildingRibbon.PathEnd != null)
                    {
                        curr.addRibbonToPath();
                        ui.BottomControlPanel.AddRibbonPoints = false;
                         ui.BottomControlPanel.setMaxRibbonPath((int)Math.Round((double)curr.lengthOfPath(curr.BuildingRibbon.PathStart.Point, curr.BuildingRibbon.PathEnd.Point)));
                         ui.BottomControlPanel.setEndPos((int)Math.Round((double)curr.Ribbon.PointToPosition(curr.BuildingRibbon.PathEnd.Point) - 1f));
                        ui.BottomControlPanel.setMaxRibbonPath((int)Math.Round((double)curr.lengthOfPath(curr.BuildingRibbon.PathStart.Point, curr.BuildingRibbon.PathEnd.Point)));
                    }
                    if (curr.Ribbon != null && curr.BuildingRibbon != null)
                    {
                        if (ui.BottomControlPanel.StartScrollValue != curr.Ribbon.PointToPosition(curr.BuildingRibbon.RibbonStart.Point))
                        {
                            curr.moveRibbonStart(ui.BottomControlPanel.StartScrollValue);
                            ui.BottomControlPanel.setMaxRibbonPath((int)Math.Round((double)curr.lengthOfPath(curr.BuildingRibbon.PathStart.Point, curr.BuildingRibbon.PathEnd.Point)));
                        }
                    }
                    if (curr.Ribbon != null && curr.BuildingRibbon != null)
                    {
                        if(ui.BottomControlPanel.EndScrollValue != curr.Ribbon.PointToPosition(curr.BuildingRibbon.RibbonEnd.Point))
                        {
                            curr.moveRibbonEnd(ui.BottomControlPanel.EndScrollValue);
                            ui.BottomControlPanel.setMaxRibbonPath((int)Math.Round((double)curr.lengthOfPath(curr.BuildingRibbon.PathStart.Point, curr.BuildingRibbon.PathEnd.Point)));
                        }
                    }
                }
                if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
                {
                    if (curr.Selection != null)
                        curr.deleteSelection();
                }
                if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    if (curr != null && curr is EditorBuildContext)
                    {
                        curr.removeSelection();
                    }
                }
                if (ui.BottomControlPanel.ClearSelection)
                {
                     curr.removeSelection();
                    ui.BottomControlPanel.ClearSelection = false;
                }
                else if (ui.GameWindowPanel.Clicked)
                {
                    System.Drawing.Point clickedPoint = ui.GameWindowPanel.ClickedPoint;
                    Vector2 formatedClick = curr.PanelToScreenCoords(clickedPoint.X, clickedPoint.Y, ui.GameWindowPanel.Width, ui.GameWindowPanel.Height);
                    if (formatedClick.X != -10000 && formatedClick.Y != 10000)
                    {
                        if (curr.isSquareOccupied((int)formatedClick.X, (int)formatedClick.Y))
                        {
                            ui.BottomControlPanel.OnlyRibbon = false;
                            if (ui.BottomControlPanel.BuildPanelContext == 3)
                            {
                                curr.setSelection((int)formatedClick.X, (int)formatedClick.Y, false, true,false);
                            }
                            else if (ui.BottomControlPanel.BuildPanelContext == 4) {
                                curr.setRibbonSelection((int)formatedClick.X, (int)formatedClick.Y);
                            }
                            else
                            {
                                curr.setSelection((int)formatedClick.X, (int)formatedClick.Y, true,true,true);
                            }         
                        }
                        else if(curr.selectionSet()){
                            curr.moveSelection((int)formatedClick.X, (int)formatedClick.Y);
                        }
                        else if (ui.BottomControlPanel.OnlyRibbon)
                        {
                            if (curr.checkIfRibbonPoint((int)formatedClick.X, (int)formatedClick.Y))
                            {
                                if (objectSelector == "box")
                                {
                                    curr.addBox(formatedClick.X, formatedClick.Y);
                                }
                                else if (objectSelector == "hook")
                                {
                                    curr.addHook(formatedClick.X, formatedClick.Y);
                                }
                                else if (objectSelector == "telescoping")
                                {
                                    curr.addTelescoping(formatedClick.X, formatedClick.Y, 10);
                                }
                                else if (objectSelector == "ribbonShooter")
                                {
                                    curr.addRibbonShooter(formatedClick.X, formatedClick.Y, 5);
                                }
                                else if (objectSelector == "flipBar")
                                {
                                    curr.addFlipBar(formatedClick.X, formatedClick.Y);
                                }
                            }
                        }
                        else
                        {       
                            if (objectSelector == "platform")
                            {
                                curr.addGround(formatedClick.X, formatedClick.Y, ui.BottomControlPanel.getObjectWidth(),ui.BottomControlPanel.getObjectHeight());
                                curr.GenerateGround();
                            }
                            else if (objectSelector == "miasma")
                            {
                                curr.addMiasma(formatedClick.X, formatedClick.Y, ui.BottomControlPanel.getObjectWidth(), ui.BottomControlPanel.getObjectHeight());
                                curr.GenerateMiasma();
                            }
                            if (objectSelector == "shooter")
                            {
                                curr.addShooter(formatedClick.X, formatedClick.Y, 0, 5);
                            }
                            else if (objectSelector == "rotating")
                            {
                                curr.addRotating(formatedClick.X, formatedClick.Y, 0, new Vector2(1, 1));
                            }
                            else if (objectSelector == "spikes")
                            {
                                curr.addSpikes(formatedClick.X, formatedClick.Y, 0, new Vector2(1, 1));
                            }
                            else if (objectSelector == "save")
                            {
                                curr.addSave(formatedClick.X, formatedClick.Y, ui.BottomControlPanel.getCheckpointStatus(), ui.RightControlPanel.WorldNum, ui.RightControlPanel.LevelNum);
                            }
                            else if (objectSelector == "pathStart" || objectSelector == "pathPoint" || objectSelector == "pathEnd")
                            {
                                curr.addRibbonPoint(objectSelector, formatedClick.X, formatedClick.Y);
                            }
                            else { }
                        }
                    }
                    ui.GameWindowPanel.Clicked = false;
                }

                if (ui.GameWindowPanel.ScrollStatus > 0)
                {
                    System.Drawing.Point scrollPoint = ui.GameWindowPanel.ScrollPosition;
                    Vector2 formatedScrollClick = curr.PanelToScreenCoords(scrollPoint.X, scrollPoint.Y, ui.GameWindowPanel.Width, ui.GameWindowPanel.Height);
                    if (ui.GameWindowPanel.ScrollStatus == 1)
                    {
                        ui.GameWindowPanel.Select();
                        ui.GameWindowPanel.Focus();
                        //Console.WriteLine(ui.GameWindowPanel.ScrollPosition);
                        if (formatedScrollClick.X != -10000 && formatedScrollClick.Y != 10000)
                        {
                            curr.screenZoom(ui.GameWindowPanel.ScrollStatus, new Vector2((float)Math.Floor(formatedScrollClick.X),formatedScrollClick.Y));
                            ui.GameWindowPanel.ScrollStatus = 0;
                        }
                        else
                        {
                            ui.GameWindowPanel.ScrollStatus = 0;
                        }
                    }
                    else
                    {
                        if (formatedScrollClick.X != -10000 && formatedScrollClick.Y != 10000)
                        {
                            curr.screenZoom(ui.GameWindowPanel.ScrollStatus, new Vector2((float)Math.Floor(formatedScrollClick.X),formatedScrollClick.Y));
                            ui.GameWindowPanel.ScrollStatus = 0;
                        }
                        else
                        {
                            ui.GameWindowPanel.ScrollStatus = 0;
                        }
                    }
                }
            }
            if (currentMode == 4){
            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    ui.ModePanel.ContextSwitch = 1;
                }
            }
            if (ui.ModePanel.ContextSwitch != 0)
            {
                SwitchContext(ui.ModePanel.ContextSwitch);
                ui.GameWindowPanel.ScrollStatus = 0;
            }

            game.Update(gameTime);
            if (ui.RightControlPanel.GridChanged)
            {
                game.currentContext.GridWidth = ui.RightControlPanel.GridWidth;
                game.currentContext.GridHeight = ui.RightControlPanel.GridHeight;
                game.currentContext.GridChanged = true;
                ui.RightControlPanel.GridChanged = false;
            }
            ui.GameWindowPanel.Clicked = false;
            oldKeyboard = keyState;
            oldSelection = objectSelector;
            base.Update(gameTime); 
        }

        protected void SwitchContext(int contextSwitch){
            game.ChangeEditorContext(ui.ModePanel.ContextSwitch, CreateLevel());
            ui.ModePanel.ContextSwitch = 0;
            ui.RightControlPanel.SetContextControl(contextSwitch);
            ui.BottomControlPanel.SetContextControl(contextSwitch);
            if (contextSwitch != 0 && contextSwitch != 1)
                ui.fullscreen();
            else
                ui.smallscreen();
            currentMode = contextSwitch;
            resetGrid = true;
        }

        private Level CreateLevel()
        {
            if (game.currentContext is EditorBuildContext)
            {
                EditorBuildContext curr = game.currentContext as EditorBuildContext;
                return LevelOutput.CreateLevel(new Vector2(curr.GridWidth, -curr.GridHeight), curr.Seamstress, curr.RibbonStores, curr.Ribbons, curr.GridModel, curr.Ground, curr.Miasma);
            }
            else if (game.currentContext is LevelContext)
            {
                LevelContext curr = game.currentContext as LevelContext;
                return curr.level;
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Stencil | ClearOptions.Target, Color.Gainsboro, 0, 0);
            game.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}