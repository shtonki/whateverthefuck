﻿using System;
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

            LayoutNode(panel, 0, 0, tree.Root);

            AddChild(panel);
        }

        private void LayoutNode(Panel panel, int x, int y, SpecializationNode node)
        {
            SpecializationNodePanel b = new SpecializationNodePanel(node);
            b.Size = new GLCoordinate(0.1f, 0.1f);
            b.Location = new GLCoordinate(0.2f * x, 0.2f * y);
            b.BackColor = Color.Orange;

            panel.AddChild(b);

            for (int i = 0; i < node.Children.Length; i++)
            {
                LayoutNode(panel, x + i, y + 1, node.Children[i]);
            }
        }
    }

    class SpecializationNodePanel : Panel
    {
        private SpecializationNode node;

        public SpecializationNodePanel(SpecializationNode node)
        {
            this.node = node;

            this.Sprite = node.Sprite;

            this.OnMouseButtonDown += (c, i) =>
            {
                node.Toggle();

                if (node.Enabled)
                {
                    this.AddBorder(Color.AntiqueWhite);
                }
                else
                {
                    this.AddBorder(Color.Transparent);
                }
            };
        }
    }
}
