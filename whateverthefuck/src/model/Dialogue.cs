using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace whateverthefuck.src.model
{
    public class Dialogue
    {
        public DialogueNode Root { get; }

        public Dialogue(DialogueNode root)
        {
            Root = root;
        }
    }

    public class DialogueNode
    {
        public string MainText { get; }

        public DialogueChoice[] Choices { get; }

        public DialogueNode(string mainText, params DialogueChoice[] choices)
        {
            MainText = mainText;
            Choices = choices;
        }
    }

    public class DialogueChoice
    {
        public string Text { get; }

        public Action Action { get; }

        public DialogueNode Next { get; }

        public DialogueChoice(string text, Action action, DialogueNode next)
        {
            Text = text;
            Action = action;
            Next = next;
        }
    }
}
