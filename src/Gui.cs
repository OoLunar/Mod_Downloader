using System;
using Gtk;

namespace Mod_Downloader {
    public class Gui {
        public static void Load() {
            Application.Init();
            Window window = new Window("Mod Installer - ForSaken Borders");
            Box windowBox = new Box(Orientation.Vertical, 4);
            windowBox.Margin = 12;
            Label entryText = new Label($"Thank you for using the installer! What would you like to do? None of your mods will be deleted, only moved.");
            RadioButton radioDefault = new RadioButton(null, "Default - Install Mods and Forge");
            RadioButton radioMods = new RadioButton(radioDefault, "Mods - Install Mods");
            RadioButton radioForge = new RadioButton(radioDefault, "Forge - Install Forge");
            RadioButton radioUpdate = new RadioButton(radioDefault, "Update - Update the modpack");
            RadioButton radioRemove = new RadioButton(radioDefault, "Remove - Remove the modpack");
            HBox boxButton = new HBox(true, 8);
            Button buttonCancel = new Button("process-stop", IconSize.Button);
            buttonCancel.Label = "Cancel";
            buttonCancel.Clicked += closeApp;
            Button buttonStart = new Button("emblem-downloads", IconSize.Button);
            buttonStart.Label = "Start!";
            boxButton.Add(buttonCancel);
            boxButton.Add(buttonStart);
            windowBox.Add(entryText);
            windowBox.Add(radioDefault);
            windowBox.Add(radioMods);
            windowBox.Add(radioForge);
            windowBox.Add(radioUpdate);
            windowBox.Add(radioRemove);
            windowBox.Add(boxButton);
            window.Add(windowBox);
            window.SetIconFromFile("res/icon.png");
            window.ShowAll();
            Application.Run();
        }

        private static void closeApp(object obj, EventArgs args) => Environment.Exit(0);
    }
}