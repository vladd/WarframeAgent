﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Core.Converters;
using Core.ViewModel;
using Newtonsoft.Json;

namespace Core.Model
{
    #region Main

    public class GameSnapshotModel
    {
        public int Version { get; set; }
        public string MobileVersion { get; set; }
        public string BuildLabel { get; set; }
        public int Time { get; set; }
        public int Date { get; set; }
        public Alert[] Alerts { get; set; }
        public Invasion[] Invasions { get; set; }
        [JsonConverter(typeof(ProjectConverter))]
        public ProjectsModel[] ProjectPct { get; set; }
        public string WorldSeed { get; set; }
    }

    #endregion

    #region Alert

    public class Alert
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public Activation Activation { get; set; }
        public Expiry Expiry { get; set; }
        public MissionInfo MissionInfo { get; set; }
    }

    public class MissionInfo
    {
        public string MissionType { get; set; }
        public string Faction { get; set; }
        public string[] Planet { get; set; }
        public string Location { get; set; }
        public string LevelOverride { get; set; }
        public string EnemySpec { get; set; }
        public int MinEnemyLevel { get; set; }
        public int MaxEnemyLevel { get; set; }
        public double Difficulty { get; set; }
        public int Seed { get; set; }
        public int MaxWaveNum { get; set; }
        public MissionReward MissionReward { get; set; }
        public string ExtraEnemySpec { get; set; }
        public List<string> CustomAdvancedSpawners { get; set; }
        public bool? ArchwingRequired { get; set; }
        public bool? IsSharkwingMission { get; set; }
    }

    public class MissionReward
    {
        public int Credits { get; set; }
        public List<CountedItem> CountedItems { get; set; }
        public List<string> Items { get; set; }
    }

    #endregion

    #region Invasions

    public class Invasion
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        public string Faction { get; set; }
        public string Node { get; set; }

        public double Count { get; set; }
        public double Goal { get; set; }

        public string LocTag { get; set; }
        public bool Completed { get; set; }
        public object AttackerReward { get; set; }
        public InvasionMissionInfo AttackerMissionInfo { get; set; }
        public InvasionReward DefenderReward { get; set; }
        public InvasionMissionInfo DefenderMissionInfo { get; set; }
        public Activation Activation { get; set; }

        internal bool Update(Invasion ntf)
        {
            bool hasChanges = false;
            if (Count != ntf.Count)
            {
                hasChanges = true;
                System.Diagnostics.Debug.WriteLine($"Changed Count {Count} to {ntf.Count}");
                Count = ntf.Count;
            }
            if (Goal != ntf.Goal)
            {
                hasChanges = true;
                Goal = ntf.Goal;
            }
            if (Completed != ntf.Completed)
            {
                hasChanges = true;
                Completed = ntf.Completed;
            }
            // is this all that can be changed?
            return hasChanges;
        }
    }

    public class InvasionReward
    {
        public List<CountedItem> CountedItems { get; set; }
    }

    public class InvasionMissionInfo
    {
        public int Seed { get; set; }
        public string Faction { get; set; }
        public List<object> MissionReward { get; set; }
    }

    #endregion

    #region Project

    public class ProjectsModel
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public SolidColorBrush Color { get; set; }
    }

    #endregion

    #region Global

    public class Id : IEquatable<Id>
    {
        [JsonProperty("$oid")]
        public string Oid { get; set; }

        public bool Equals(Id other) => ((object)other != null) && Oid == other.Oid;
        public static bool operator == (Id l, Id r) => l?.Equals(r) ?? ((object)r == null);
        public static bool operator != (Id l, Id r) => !(l == r);
        public override bool Equals(object obj) => Equals(obj as Id);
        public override int GetHashCode() => Oid?.GetHashCode() ?? 0;
    }

    public class Date
    {
        // TODO: simplify this
        [JsonProperty("$numberLong")]
        public string NumberLongStr { get; set; }

        public long NumberLong
        {
            get => long.Parse(NumberLongStr);
            set => NumberLongStr = value.ToString();
        }
    }

    public class Activation
    {
        [JsonProperty("$date")]
        public Date Date { get; set; }
    }

    public class Expiry
    {
        [JsonProperty("$date")]
        public Date Date { get; set; }
    }

    public class CountedItem
    {
        public string ItemType { get; set; }
        public int ItemCount { get; set; }
    }

    #endregion
}