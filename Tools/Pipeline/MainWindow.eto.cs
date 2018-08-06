// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace MonoGame.Tools.Pipeline
{
    partial class MainWindow
    {
        /// <summary>
        /// Pipeline menu bar.
        /// Required to Stop Eto Forms adding System Menu Items on MacOS
        /// This is because `IncludeSystemItems` defaults to `All` 
        /// and the menus are populated in the constructor.
        /// </summary>
        class PipelineMenuBar : MenuBar
        {
            public PipelineMenuBar()
            {
                Style = "MenuBar";
                IncludeSystemItems = MenuBarSystemItems.None;
            }
        }

        public Command cmdNew, cmdOpen, cmdClose, cmdImport, cmdSave, cmdSaveAs, cmdExit;
        public Command cmdUndo, cmdRedo, cmdAdd, cmdExclude, cmdRename, cmdDelete;
        public Command cmdNewItem, cmdNewFolder, cmdExistingItem, cmdExistingFolder;
        public Command cmdBuild, cmdRebuild, cmdClean, cmdCancelBuild;
        public CheckCommand cmdDebugMode;
        public Command cmdHelp, cmdAbout;
        public Command cmdOpenItem, cmdOpenItemWith, cmdOpenItemLocation, cmdOpenOutputItemLocation, cmdCopyAssetPath, cmdRebuildItem;

        ToolBar toolbar;
        ButtonMenuItem menuFile, menuRecent, menuEdit, menuAdd, menuView, menuBuild, menuHelp;
        ToolItem toolBuild, toolRebuild, toolClean, toolCancelBuild;
        MenuItem cmOpenItem, cmOpenItemWith, cmOpenItemLocation, cmOpenOutputItemLocation, cmCopyAssetPath, cmRebuildItem, cmExclude, cmRename, cmDelete;
        ButtonMenuItem cmAdd;

        ProjectControl projectControl;
        PropertyGridControl propertyGridControl;
        BuildOutput buildOutput;

        Splitter splitterHorizontal, splitterVertical;

        private void InitializeComponent()
        {
            Title = "MonoGame Pipeline Tool";
            Icon = Icon.FromResource("Icons.monogame.png");
            Size = new Size(750, 550);
            MinimumSize = new Size(400, 400);

            InitalizeCommands();
            InitalizeMenu();
            InitalizeContextMenu();
            InitalizeToolbar();

            splitterHorizontal = new Splitter
            {
                Orientation = Orientation.Horizontal,
                Position = 200,
                Panel1MinimumSize = 100,
                Panel2MinimumSize = 100
            };

            splitterVertical = new Splitter
            {
                Orientation = Orientation.Vertical,
                Position = 230,
                FixedPanel = SplitterFixedPanel.None,
                Panel1MinimumSize = 100,
                Panel2MinimumSize = 100
            };

            projectControl = new ProjectControl();
            _pads.Add(projectControl);
            splitterVertical.Panel1 = projectControl;

            propertyGridControl = new PropertyGridControl();
            _pads.Add(propertyGridControl);
            splitterVertical.Panel2 = propertyGridControl;

            splitterHorizontal.Panel1 = splitterVertical;

            buildOutput = new BuildOutput();
            _pads.Add(buildOutput);
            splitterHorizontal.Panel2 = buildOutput;

            Content = splitterHorizontal;

            cmdNew.Executed += CmdNew_Executed;
            cmdOpen.Executed += CmdOpen_Executed;
            cmdClose.Executed += CmdClose_Executed;
            cmdImport.Executed += CmdImport_Executed;
            cmdSave.Executed += CmdSave_Executed;
            cmdSaveAs.Executed += CmdSaveAs_Executed;
            cmdExit.Executed += CmdExit_Executed;

            cmdUndo.Executed += CmdUndo_Executed;
            cmdRedo.Executed += CmdRedo_Executed;
            cmdExclude.Executed += CmdExclude_Executed;
            cmdRename.Executed += CmdRename_Executed;
            cmdDelete.Executed += CmdDelete_Executed;

            cmdNewItem.Executed += CmdNewItem_Executed;
            cmdNewFolder.Executed += CmdNewFolder_Executed;
            cmdExistingItem.Executed += CmdExistingItem_Executed;
            cmdExistingFolder.Executed += CmdExistingFolder_Executed;

            cmdBuild.Executed += CmdBuild_Executed;
            cmdRebuild.Executed += CmdRebuild_Executed;
            cmdClean.Executed += CmdClean_Executed;
            cmdCancelBuild.Executed += CmdCancelBuild_Executed;
            cmdDebugMode.Executed += CmdDebugMode_Executed;

            cmdHelp.Executed += CmdHelp_Executed;
            cmdAbout.Executed += CmdAbout_Executed;

            cmdOpenItem.Executed += CmdOpenItem_Executed;
            cmdOpenItemWith.Executed += CmdOpenItemWith_Executed;
            cmdOpenItemLocation.Executed += CmdOpenItemLocation_Executed;
            cmdOpenOutputItemLocation.Executed += CmdOpenOutputItemLocation_Executed;
            cmdCopyAssetPath.Executed += CmdCopyAssetPath_Executed;
            cmdRebuildItem.Executed += CmdRebuildItem_Executed;
        }

        private void InitalizeCommands()
        {
            // File Commands

            cmdNew = new Command
            {
                MenuText = "New...",
                ToolTip = "New",
                Image = Global.GetEtoIcon("Commands.New.png"),
                Shortcut = Application.Instance.CommonModifier | Keys.N
            };

            cmdOpen = new Command
            {
                MenuText = "Open...",
                ToolTip = "Open",
                Image = Global.GetEtoIcon("Commands.Open.png"),
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };

            cmdClose = new Command
            {
                MenuText = "Close",
                Image = Global.GetEtoIcon("Commands.Close.png")
            };

            cmdImport = new Command
            {
                MenuText = "Import"
            };

            cmdSave = new Command
            {
                MenuText = "Save...",
                ToolTip = "Save",
                Image = Global.GetEtoIcon("Commands.Save.png"),
                Shortcut = Application.Instance.CommonModifier | Keys.S
            };

            cmdSaveAs = new Command
            {
                MenuText = "Save As",
                Image = Global.GetEtoIcon("Commands.SaveAs.png")
            };

            cmdExit = new Command
            {
                MenuText = Global.Unix ? "Quit" : "Exit",
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };

            // Edit Commands

            cmdUndo = new Command
            {
                MenuText = "Undo",
                ToolTip = "Undo",
                Image = Global.GetEtoIcon("Commands.Undo.png"),
                Shortcut = Application.Instance.CommonModifier | Keys.Z
            };

            cmdRedo = new Command
            {
                MenuText = "Redo",
                ToolTip = "Redo",
                Image = Global.GetEtoIcon("Commands.Redo.png"),
                Shortcut = Application.Instance.CommonModifier | Keys.Y
            };

            cmdAdd = new Command
            {
                MenuText = "Add"
            };

            cmdExclude = new Command
            {
                MenuText = "Exclude From Project"
            };

            cmdRename = new Command
            {
                MenuText = "Rename",
                Image = Global.GetEtoIcon("Commands.Rename.png")
            };

            cmdDelete = new Command
            {
                MenuText = "Delete",
                Image = Global.GetEtoIcon("Commands.Delete.png"),
                Shortcut = Keys.Delete
            };

            // Add Submenu

            cmdNewItem = new Command
            {
                MenuText = "New Item...",
                ToolTip = "New Item",
                Image = Global.GetEtoIcon("Commands.NewItem.png")
            };

            cmdNewFolder = new Command
            {
                MenuText = "New Folder...",
                ToolTip = "New Folder",
                Image = Global.GetEtoIcon("Commands.NewFolder.png")
            };

            cmdExistingItem = new Command
            {
                MenuText = "Existing Item...",
                ToolTip = "Add Existing Item",
                Image = Global.GetEtoIcon("Commands.ExistingItem.png")
            };

            cmdExistingFolder = new Command
            {
                MenuText = "Existing Folder...",
                ToolTip = "Add Existing Folder",
                Image = Global.GetEtoIcon("Commands.ExistingFolder.png")
            };

            // Build Commands

            cmdBuild = new Command
            {
                MenuText = "Build",
                ToolTip = "Build",
                Image = Global.GetEtoIcon("Commands.Build.png"),
                Shortcut = Keys.F6
            };

            cmdRebuild = new Command
            {
                MenuText = "Rebuild",
                ToolTip = "Rebuild",
                Image = Global.GetEtoIcon("Commands.Rebuild.png")
            };

            cmdClean = new Command
            {
                MenuText = "Clean",
                ToolTip = "Clean",
                Image = Global.GetEtoIcon("Commands.Clean.png")
            };

            cmdCancelBuild = new Command
            {
                MenuText = "Cancel Build",
                ToolTip = "Cancel Build",
                Image = Global.GetEtoIcon("Commands.CancelBuild.png")
            };

            cmdDebugMode = new CheckCommand
            {
                MenuText = "Debug Mode"
            };

            // Help Commands

            cmdHelp = new Command
            {
                MenuText = "View Help",
                Shortcut = Keys.F1,
                Image = Global.GetEtoIcon("Commands.Help.png")
            };

            cmdAbout = new Command
            {
                MenuText = "About"
            };

            // Context Menu

            cmdOpenItem = new Command
            {
                MenuText = "Open",
                Image = Global.GetEtoIcon("Commands.OpenItem.png")
            };

            cmdOpenItemWith = new Command
            {
                MenuText = "Open With"
            };

            cmdOpenItemLocation = new Command
            {
                MenuText = "Open Containing Directory"
            };

            cmdOpenOutputItemLocation = new Command
            {
                MenuText = "Open Output Directory"
            };

            cmdCopyAssetPath = new Command
            {
                MenuText = "Copy Asset Path"
            };

            cmdRebuildItem = new Command
            {
                Image = Global.GetEtoIcon("Commands.Rebuild.png"),
                MenuText = "Rebuild"
            };
        }

        private void InitalizeMenu()
        {
            Menu = new PipelineMenuBar();

            menuFile = new ButtonMenuItem
            {
                Text = "&File"
            };
            menuFile.Items.Add(cmdNew);
            menuFile.Items.Add(cmdOpen);

            menuRecent = new ButtonMenuItem
            {
                Text = "Open Recent"
            };
            menuFile.Items.Add(menuRecent);

            menuFile.Items.Add(cmdClose);
            menuFile.Items.Add(new SeparatorMenuItem());
            menuFile.Items.Add(cmdImport);
            menuFile.Items.Add(new SeparatorMenuItem());
            menuFile.Items.Add(cmdSave);
            menuFile.Items.Add(cmdSaveAs);
            Menu.Items.Add(menuFile);

            menuEdit = new ButtonMenuItem
            {
                Text = "&Edit"
            };
            menuEdit.Items.Add(cmdUndo);
            menuEdit.Items.Add(cmdRedo);
            menuEdit.Items.Add(new SeparatorMenuItem());

            menuAdd = (ButtonMenuItem)cmdAdd.CreateMenuItem();
            menuAdd.Items.Add(cmdNewItem);
            menuAdd.Items.Add(cmdNewFolder);
            menuAdd.Items.Add(new SeparatorMenuItem());
            menuAdd.Items.Add(cmdExistingItem);
            menuAdd.Items.Add(cmdExistingFolder);
            menuEdit.Items.Add(menuAdd);

            menuEdit.Items.Add(new SeparatorMenuItem());
            menuEdit.Items.Add(cmdExclude);
            menuEdit.Items.Add(new SeparatorMenuItem());
            menuEdit.Items.Add(cmdRename);
            //menuEdit.Items.Add(cmdDelete);
            Menu.Items.Add(menuEdit);

            // View Commands

            menuView = new ButtonMenuItem
            {
                Text = "&View"
            };
            Menu.Items.Add(menuView);

            menuBuild = new ButtonMenuItem
            {
                Text = "&Build"
            };
            menuBuild.Items.Add(cmdBuild);
            menuBuild.Items.Add(cmdRebuild);
            menuBuild.Items.Add(cmdClean);
            menuBuild.Items.Add(cmdCancelBuild);
            menuBuild.Items.Add(new SeparatorMenuItem());
            menuBuild.Items.Add(cmdDebugMode);
            Menu.Items.Add(menuBuild);

            menuHelp = new ButtonMenuItem
            {
                Text = "&Help"
            };
            menuHelp.Items.Add(cmdHelp);
            Menu.Items.Add(menuHelp);

            Menu.QuitItem = cmdExit;
            Menu.AboutItem = cmdAbout;
        }

        private void InitalizeContextMenu()
        {
            cmOpenItem = cmdOpenItem.CreateMenuItem();
            cmOpenItemWith = cmdOpenItemWith.CreateMenuItem();

            cmAdd = (ButtonMenuItem)cmdAdd.CreateMenuItem();
            cmAdd.Items.Add(cmdNewItem.CreateMenuItem());
            cmAdd.Items.Add(cmdNewFolder.CreateMenuItem());
            cmAdd.Items.Add(new SeparatorMenuItem());
            cmAdd.Items.Add(cmdExistingItem.CreateMenuItem());
            cmAdd.Items.Add(cmdExistingFolder.CreateMenuItem());

            cmOpenItemLocation = cmdOpenItemLocation.CreateMenuItem();
            cmOpenOutputItemLocation = cmdOpenOutputItemLocation.CreateMenuItem();
            cmCopyAssetPath = cmdCopyAssetPath.CreateMenuItem();
            cmRebuildItem = cmdRebuildItem.CreateMenuItem();
            cmExclude = cmdExclude.CreateMenuItem();
            cmRename = cmdRename.CreateMenuItem();
            cmDelete = cmdDelete.CreateMenuItem();
        }

        private void InitalizeToolbar()
        {
            toolBuild = cmdBuild.CreateToolItem();
            toolRebuild = cmdRebuild.CreateToolItem();
            toolClean = cmdClean.CreateToolItem();
            toolCancelBuild = cmdCancelBuild.CreateToolItem();

            ToolBar = toolbar = new ToolBar();
            ToolBar.Style = "ToolBar";
            ToolBar.Items.Add(cmdNew);
            ToolBar.Items.Add(cmdOpen);
            ToolBar.Items.Add(cmdSave);
            ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
            ToolBar.Items.Add(cmdUndo);
            ToolBar.Items.Add(cmdRedo);
            ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
            ToolBar.Items.Add(cmdNewItem);
            ToolBar.Items.Add(cmdExistingItem);
            ToolBar.Items.Add(cmdNewFolder);
            ToolBar.Items.Add(cmdExistingFolder);
            ToolBar.Items.Add(new SeparatorToolItem { Type = SeparatorToolItemType.Divider });
            ToolBar.Items.Add(toolBuild);
            ToolBar.Items.Add(toolRebuild);
            ToolBar.Items.Add(toolClean);
            toolbar.Items.Add(toolCancelBuild);
        }
    }
}

