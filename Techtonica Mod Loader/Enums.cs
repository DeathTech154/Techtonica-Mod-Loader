using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techtonica_Mod_Loader
{
    #region ModListSource

    public enum ModListSource
    {
        Installed,
        Online,
        Null
    }

    public static partial class StringUtils
    {
        public static ModListSource GetModListSourceFromName(string name) {
            switch (name) {
                case "Installed": return ModListSource.Installed;
                case "Online": return ModListSource.Online;
                case "Null": return ModListSource.Null;
                default: return ModListSource.Null;
            }
        }

        public static string GetModListSourceName(ModListSource modListSource) {
            switch (modListSource) {
                case ModListSource.Installed: return "Installed";
                case ModListSource.Online: return "Online";
                case ModListSource.Null: return "Null";
                default: return Enum.GetName(typeof(ModListSource), modListSource);
            }
        }

        public static List<string> GetAllModListSourceNames() {
            List<string> names = new List<string>();
            foreach (ModListSource modListSource in Enum.GetValues(typeof(ModListSource))) {
                if(modListSource != ModListSource.Null) {
                    names.Add(GetModListSourceName(modListSource));
                }
            }

            return names;
        }
    }

    #endregion

    #region ModListSortOption

    public enum ModListSortOption
    {
        Alphabetical,
        LastUpdated,
        Downloads,
        Popularity,
        Null
    }

    public static partial class StringUtils
    {
        public static ModListSortOption GetModListSortOptionFromName(string name) {
            switch (name) {
                case "Last Updated": return ModListSortOption.LastUpdated;
                case "Alphabetical": return ModListSortOption.Alphabetical;
                case "Downloads": return ModListSortOption.Downloads;
                case "Popularity": return ModListSortOption.Popularity;
                case "Null": return ModListSortOption.Null;
                default: return ModListSortOption.Null;
            }
        }

        public static string GetModListSortOptionName(ModListSortOption modListSortOption) {
            switch (modListSortOption) {
                case ModListSortOption.LastUpdated: return "Last Updated";
                case ModListSortOption.Alphabetical: return "Alphabetical";
                case ModListSortOption.Downloads: return "Downloads";
                case ModListSortOption.Popularity: return "Popularity";
                case ModListSortOption.Null: return "Null";
                default: return Enum.GetName(typeof(ModListSortOption), modListSortOption);
            }
        }

        public static List<string> GetAllModListSortOptionNames() {
            List<string> names = new List<string>();
            foreach (ModListSortOption modListSortOption in Enum.GetValues(typeof(ModListSortOption))) {
                if(modListSortOption != ModListSortOption.Null) {
                    names.Add(GetModListSortOptionName(modListSortOption));
                }
            }

            return names;
        }
    }

    #endregion
}
