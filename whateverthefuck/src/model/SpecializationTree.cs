using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model.entities;
using whateverthefuck.src.view;

namespace whateverthefuck.src.model
{
    public class SpecializationTree
    {
        public SpecializationTree()
        {
            var t1 = new SpecializationNode(Specializations.Test1);
            var t2 = new SpecializationNode(Specializations.Test2);
            Root = new SpecializationNode(Specializations.Root, t1, t2);

        }

        public event Action OnChanged;

        public SpecializationNode Root { get; set; }

        public void SetBaseStats(StatStruct stats)
        {
            stats.DamageTakenMultiplier = 1;
            stats.GlobalCooldown = 1;
            stats.MaxHealth = 100;
            stats.MoveSpeed = Character.SpeedSlow;

            stats.Intelligence = 10;
            stats.Strength = 10;

            var specs = GetSpecialization();

            if (specs[(int)Specializations.Test1])
            {
                stats.MoveSpeed *= 2.0f;
            }
        }

        public void Set()
        {
            OnChanged?.Invoke();
        }

        private bool HasSpecialization(Specializations spec, bool[] bools)
        {
            return bools[(int)spec];
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
    }

    public enum Specializations
    {
        Root,

        Test1,
        Test2,
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

        public void Toggle()
        {
            Enabled = !Enabled;
        }
    }
}
