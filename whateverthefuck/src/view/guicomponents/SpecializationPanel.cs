using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuck.src.view.guicomponents
{
    class SpecializationPanel : Panel
    {
        private Panel panel;
        private SpecializationTree tree;

        public SpecializationPanel(SpecializationTree tree, GLCoordinate size)
        {
            this.BackColor = Color.Green;

            this.Size = size;

            this.tree = tree;
            LayoutTree(this.tree);

            var saveButton = new TextPanel("Save", Color.Black);
            saveButton.Location = new GLCoordinate(0, size.Y - 0.2f);
            saveButton.BackColor = Color.White;
            saveButton.OnMouseButtonDown += (c, i) => Save();
            AddChild(saveButton);
        }

        private void Save()
        {
            Program.GameStateManager.UpdateSpecialization(tree);
        }

        private void LayoutTree(SpecializationTree tree)
        {
            if (panel != null)
            {
                RemoveChild(panel);
                panel = null;
            }

            panel = new DraggablePanel();
            panel.Size = new GLCoordinate(Size.X, Size.Y - 0.2f);
            panel.BackColor = Color.Black;

            LayoutNode(panel, 0, 0, tree.Root).available = true;

            AddChild(panel);
        }

        private SpecializationNodePanel LayoutNode(Panel panel, int x, int y, SpecializationNode node)
        {
            SpecializationNodePanel nodePanel = new SpecializationNodePanel(node);
            nodePanel.Size = new GLCoordinate(0.1f, 0.1f);
            nodePanel.Location = new GLCoordinate(0.2f * x, 0.2f * y);
            nodePanel.BackColor = Color.Orange;

            panel.AddChild(nodePanel);

            for (int i = 0; i < node.Children.Length; i++)
            {
                var child = LayoutNode(panel, x + i, y + 1, node.Children[i]);
                nodePanel.OnToggled += snp => child.ParentToggled(snp);
            }

            return nodePanel;
        }
    }

    class SpecializationNodePanel : Panel
    {
        private SpecializationNode node;

        public bool available;

        public SpecializationNodePanel(SpecializationNode node)
        {
            this.node = node;

            this.Sprite = node.Sprite;

            this.OnMouseButtonDown += (c, i) =>
            {
                if (available)
                {
                    Toggle();
                }
            };
        }

        public event Action<SpecializationNodePanel> OnToggled;

        public void Toggle()
        {
            SetState(!node.Enabled, available);
        }

        public void ParentToggled(SpecializationNodePanel parent)
        {
            SetState(node.Enabled && parent.node.Enabled, parent.node.Enabled);
        }

        private void SetState(bool specced, bool speccable)
        {
            node.Enabled = specced;
            available = speccable;

            if (specced)
            {
                this.AddBorder(Color.Green);
            }
            else
            {
                if (speccable)
                {
                    this.AddBorder(Color.Orange);
                }
                else
                {

                    this.AddBorder(Color.Red);
                }
            }

            OnToggled?.Invoke(this);
        }
    }
}
