using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Controls;
using static Slide_Kinect.Utilities.Properties;

namespace Slide_Kinect {
    internal static class LoadConfiguration {
        // Read JSON file
        public static void readFile() {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a configuration file"; // Title
            openFileDialog.Filter = "JSON files (*.json)|*.json"; // Filter
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // Path

            if (openFileDialog.ShowDialog() == true) {
                applySettings(JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(openFileDialog.FileName)), (Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Slide Kinect") + @"\configuration.json")); // Apply settings
            }
        }

        // Apply settings
        public static void applySettings(Configuration configuration, string path) {
            createProfile(configuration, path);
        }

        // Create new profile
        public static void createProfile(Configuration configuration, string path) {
            string jsonFile = JsonConvert.SerializeObject(configuration, Formatting.Indented); // Convert JSON to text

            using (var file = new StreamWriter(path)) {
                file.Write(jsonFile); // Create file
            }
        }

        // Default configuration for new users
        public static Configuration defaultConfiguration() {
            var newProfile = new Configuration {
                save_automatically = true,
                view_mode = "color",
                draw_skeleton = true,
                next_slide = true,
                previous_slide = true,
                cursor_mode = true,
                hand= "right",
                position = new Position {
                    next_slide = 0.2,
                    previous_slide = -0.2,
                    activation_angle = 0.05,
                    min_x = -0.3,
                    min_y = -0.2,
                    max_x = 0.4,
                    max_y = 0.4,
                    depth = 0.3
                }
            };

            return newProfile;
        }
    }

    // Configuration structure
    public class Configuration {
        public bool save_automatically { get; set; }
        public string view_mode { get; set; }
        public bool draw_skeleton { get; set; }
        public bool next_slide { get; set; }
        public bool previous_slide { get; set; }
        public bool cursor_mode { get; set; }
        public string hand { get; set; }
        public Position position { get; set; }
    }

    // Position structure
    public class Position {
        public double next_slide { get; set; }
        public double previous_slide { get; set; }
        public double activation_angle { get; set; }
        public double min_x { get; set; }
        public double min_y { get; set; }
        public double max_x { get; set; }
        public double max_y { get; set; }
        public double depth { get; set; }
    }
}