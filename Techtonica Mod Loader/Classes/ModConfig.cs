using MyLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Techtonica_Mod_Loader;

namespace Techtonica_Mod_Loader.Classes
{
    public class ModConfig
    {
        // Objects & Variables
        public static ModConfig activeConfig;

        public string filePath;
        public List<ConfigOption> options = new List<ConfigOption>();

        // Public Functions

        public List<string> GetCategories() {
            List<string> categories = new List<string>();
            foreach (ConfigOption option in options) {
                if (!categories.Contains(option.category)) {
                    categories.Add(option.category);
                }
            }

            categories.Sort();
            return categories;
        }

        public List<ConfigOption> GetOptionsInCategory(string category) {
            return options.Where(option => option.category == category).ToList();
        }

        public void UpdateSetting(string name, string value) {
            foreach(ConfigOption option in options) {
                if(option.name == name) {
                    StringConfigOption stringOption = option as StringConfigOption;
                    stringOption.value = value;
                    SaveToFile();
                    return;
                }
            }
        }

        public void UpdateSetting(string name, int value) {
            foreach(ConfigOption option in options) {
                if(option.name == name) {
                    IntConfigOption intOption = option as IntConfigOption;
                    intOption.value = value;
                    SaveToFile();
                    return;
                }
            }
        }

        public void UpdateSetting(string name, float value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    FloatConfigOption floatOption = option as FloatConfigOption;
                    floatOption.value = value;
                    SaveToFile();
                    return;
                }
            }
        }

        public void UpdateSetting(string name, double value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    DoubleConfigOption doubleOption = option as DoubleConfigOption;
                    doubleOption.value = value;
                    SaveToFile();
                    return;
                }
            }
        }

        public void UpdateSetting(string name, bool value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    BooleanConfigOption boolOption = option as BooleanConfigOption;
                    boolOption.value = value;
                    SaveToFile();
                    return;
                }
            }
        }

        // Save And Load Functions

        public static ModConfig FromFile(string filename) {
            ModConfig config = new ModConfig() { filePath = filename };
            string[] lines = File.ReadAllLines(filename);


            bool startedConfigFile = false;
            string latestCategory = "";
            for (int i = 0; i < lines.Length; i++) {
                string thisLine = lines[i];
                if (thisLine.StartsWith("[")) {
                    startedConfigFile = true;
                    latestCategory = thisLine.Replace("[", "").Replace("]", "");
                }

                if(thisLine.StartsWith("##") && startedConfigFile) {
                    string optionType = lines[i + 1].Split(new string[] { ": " }, StringSplitOptions.None).Last();
                    List<string> optionLines = new List<string>(){};
                    for(int j = i; j < lines.Count(); j++) {
                        if (string.IsNullOrEmpty(lines[j]) || string.IsNullOrWhiteSpace(lines[j]) || j == lines.Count() - 1) {
                            for(int k = i; k < j; k++) {
                                optionLines.Add(lines[k]);
                            }

                            break;
                        }
                    }

                    switch (optionType) {
                        case "String": config.options.Add(new StringConfigOption(optionLines) { category = latestCategory }); break;
                        case "Int32": config.options.Add(new IntConfigOption(optionLines) { category = latestCategory }); break;
                        case "Single": config.options.Add(new FloatConfigOption(optionLines) { category = latestCategory }); break;
                        case "Double": config.options.Add(new DoubleConfigOption(optionLines) { category = latestCategory }); break;
                        case "Boolean": config.options.Add(new BooleanConfigOption(optionLines) { category = latestCategory }); break;
                    }

                    i = (i + 4 > lines.Count()) ? lines.Count() - 1 : i + 4;
                }
            }

            return config;
        }

        public void SaveToFile() {
            List<string> fileLines = new List<string>() {
                "## Settings file was created by Techtonica Mod Loader",
                ""
            };

            foreach(string category in GetCategories()) {
                fileLines.Add($"[{category}]");
                foreach(ConfigOption option in GetOptionsInCategory(category)) {
                    fileLines.Add("");
                    fileLines.Add($"## {option.description}");
                    fileLines.AddRange(option.ToLines());
                }

                fileLines.Add("");
            }

            File.WriteAllLines(filePath, fileLines);
        }        
    }

    public class ConfigOption 
    {
        public string name;
        public string description;
        public string category;
        public string optionType;

        public virtual List<string> ToLines(){
            string error = "ConfigOption.ToLine() has not been overridden";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return new List<string>();
        }
    }

    public class StringConfigOption : ConfigOption 
    {
        public string value;
        public string defaultValue;

        public StringConfigOption() { optionType = "String"; }
        public StringConfigOption(List<string> fileLines) {
            optionType = "String";
            foreach(string line in fileLines) {
                if(line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last();
                }
                else if (!line.StartsWith("# Setting type")){
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last();
                }
            }
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
                $"{name} = {value}"
            };
        }
    }

    public class IntConfigOption : ConfigOption
    {
        public int value;
        public int defaultValue;
        public int min = int.MinValue;
        public int max = int.MaxValue;

        public IntConfigOption(List<string> fileLines) {
            optionType = "Int32";
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = int.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = int.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };
            
            if(min != int.MinValue && max != int.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out int minValue, out int maxValue) {
            string pattern = @"From (-?\d+) to (-?\d+)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (int.TryParse(match.Groups[1].Value, out int min) && int.TryParse(match.Groups[2].Value, out int max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = int.MinValue;
            maxValue = int.MaxValue;
            return false;
        }
    }

    public class FloatConfigOption : ConfigOption 
    {
        public float value;
        public float defaultValue;
        public float min = float.MinValue;
        public float max = float.MaxValue;

        public FloatConfigOption(List<string> fileLines) {
            optionType = "Single";
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = float.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = float.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };

            if (min != float.MinValue && max != float.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out float minValue, out float maxValue) {
            string pattern = @"From (-?\d+(\.\d+)?) to (-?\d+(\.\d+)?)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (float.TryParse(match.Groups[1].Value, out float min) && float.TryParse(match.Groups[2].Value, out float max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = float.MinValue;
            maxValue = float.MaxValue;
            return false;
        }
    }

    public class DoubleConfigOption : ConfigOption
    {
        public double value;
        public double defaultValue;
        public double min = double.MinValue;
        public double max = double.MaxValue;

        public DoubleConfigOption(List<string> fileLines) {
            optionType = "Double";
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = double.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = double.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };

            if (min != double.MinValue && max != double.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out double minValue, out double maxValue) {
            string pattern = @"From (-?\d+(\.\d+)?) to (-?\d+(\.\d+)?)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (double.TryParse(match.Groups[1].Value, out double min) && double.TryParse(match.Groups[2].Value, out double max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = double.MinValue;
            maxValue = double.MaxValue;
            return false;
        }
    }

    public class BooleanConfigOption : ConfigOption 
    {
        public bool value;
        public bool defaultValue;

        public BooleanConfigOption(){}
        public BooleanConfigOption(List<string> fileLines) {
            optionType = "Boolean";
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last() == "true";
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last() == "true";
                }
            }
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue.ToString().ToLower()}",
                $"{name} = {value.ToString().ToLower()}"
            };
        }
    }
}
