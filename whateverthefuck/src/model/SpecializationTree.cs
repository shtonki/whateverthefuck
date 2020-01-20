using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.util;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public class SpecializationTree : IEncodable
    {
        private const float BaseMoveSpeed = Character.SpeedSlow;
        private const float BaseDamageTakenMultiplier = 1;
        private const int BaseGlobalCooldownTicks = 100;
        private const int BaseMaxHealth = 100;
        private const int BaseIntelligence = 10;
        private const int BaseStrength = 10;

        public SpecializationTree()
        {
            var speed2 = new SpecializationNode(Specializations.Speed2);
            var speed1 = new SpecializationNode(Specializations.Speed1, speed2);
            var int1 = new SpecializationNode(Specializations.Intelligence1);
            Root = new SpecializationNode(Specializations.Root, speed1, int1);
        }

        public event Action OnChanged;

        public SpecializationNode Root { get; set; }

        public void SetBaseStats(StatStruct stats)
        {
            stats.DamageTakenMultiplier = BaseDamageTakenMultiplier;
            stats.GlobalCooldown = BaseGlobalCooldownTicks;
            stats.MaxHealth = BaseMaxHealth;
            stats.MoveSpeed = BaseMoveSpeed;

            stats.Intelligence = BaseIntelligence;
            stats.Strength = BaseStrength;

            var specs = GetSpecialization();

            if (specs[(int)Specializations.Speed1])
            {
                stats.MoveSpeed += BaseMoveSpeed * 1f;
            }

            if (specs[(int)Specializations.Speed2])
            {
                stats.MoveSpeed += BaseMoveSpeed * 1f;
            }

            if (specs[(int)Specializations.Intelligence1])
            {
                stats.Intelligence += 100;
            }
        }

        public void Set(SpecializationTree other)
        {
            SetSpecialization(other.GetSpecialization());
            OnChanged?.Invoke();
        }

        private bool[] GetSpecialization()
        {
            bool[] rt = new bool[Enum.GetValues(typeof(Specializations)).Length];

            foreach (var node in GetNodes())
            {
                rt[(int)node.Specialization] = node.Enabled;
            }

            return rt;
        }

        private void SetSpecialization(bool[] specs)
        {
            foreach (var node in GetNodes())
            {
                node.Enabled = specs[(int)node.Specialization];
            }
        }

        private IEnumerable<SpecializationNode> GetNodes()
        {
            List<SpecializationNode> rt = new List<SpecializationNode>();
            AddNode(Root, rt);
            return rt;
        }

        private void AddNode(SpecializationNode node, List<SpecializationNode> list)
        {
            list.Add(node);

            foreach (var child in node.Children)
            {
                AddNode(child, list);
            }
        }

        public void Encode(WhateverEncoder encoder)
        {
            encoder.Encode(GetSpecialization());
        }

        public void Decode(WhateverDecoder decoder)
        {
            SetSpecialization(decoder.DecodeBoolArray());
        }
    }

    public enum Specializations
    {
        Root,

        Speed1,
        Speed2,
        Intelligence1,
    }

    public class SpecializationNode
    {
        public SpecializationNode(Specializations specialization, params SpecializationNode[] children)
        {
            Children = children;
            Specialization = specialization;

            Sprite = new Sprite(util.SpriteID.testSprite1);

        }

        public SpecializationNode[] Children { get; }

        public bool Enabled { get; set; }

        public Specializations Specialization { get; set; }

        public Sprite Sprite { get; set; }
    }
}
