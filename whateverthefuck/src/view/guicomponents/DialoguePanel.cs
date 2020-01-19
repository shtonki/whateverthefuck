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
    class DialoguePanel : Panel
    {
        private const float Padding = 0.05f;

        private Dialogue Dialogue;

        public DialoguePanel(Dialogue dialogue, GLCoordinate size)
        {
            this.Size = size;
            Dialogue = dialogue;

            ShowDialogueNode(Dialogue.Root);
        }

        private void ShowDialogueNode(DialogueNode node)
        {
            if (node == null)
            {
                Destroy();
                return;
            }

            ClearChildren();

            var mainText = new TextPanel(node.MainText, Color.Black);
            mainText.Location = new GLCoordinate(0, this.Size.Y - mainText.Size.Y);
            var y = this.Size.Y - mainText.Size.Y;
            mainText.BackColor = Color.HotPink;
            AddChild(mainText);

            foreach (var choice in node.Choices)
            {
                var choiceText = new TextPanel(choice.Text, Color.Black);
                choiceText.OnMouseButtonDown += (a, b) =>
                {
                    choice.Action();
                    ShowDialogueNode(choice.Next);
                };
                y -= choiceText.Size.Y + Padding;
                choiceText.Location = new GLCoordinate(0, y);
                choiceText.BackColor = Color.Green;
                AddChild(choiceText);
            }
        }
    }
}
