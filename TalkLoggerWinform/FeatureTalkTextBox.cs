﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tono;
using Tono.GuiWinForm;

namespace TalkLoggerWinform
{
    public class FeatureTalkTextBox : FeatureControlBridgeBase, IMouseListener
    {
        private DataHot Hot => (DataHot)Data;
        private PartsCollectionBase SelectedParts;
        private PartsTalkBar SelectedBar = null;
        private RichTextBox TextBox { get; set; }
        private Label LabelTime { get; set; }

        public override void OnInitInstance()
        {
            base.OnInitInstance();
            SelectedParts = (PartsCollectionBase)Share.Get("SelectedParts", typeof(PartsCollection));

            TextBox = GetControl("textBoxTalk") as RichTextBox;
            TextBox?.ResetText();
            TextBox.SelectionChanged += TextBox_SelectionChanged;
            LabelTime = GetControl("labelTalkBarTime") as Label;
        }
        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            Hot.SelectedText = TextBox.SelectedText;
        }
        public void Update()
        {
            if (SelectedBar == null)
            {
                TextBox.ResetText();
                LabelTime?.ResetText();
                return;
            }
            else
            {
                var spos = TextBox.SelectionStart;  // try to keep text selection
                var slen = TextBox.SelectionLength;
                var stxt = TextBox.SelectedText;

                TextBox.Text = SelectedBar.Text;

                if (slen > 0 && TextBox.Text.Substring(spos, slen) == stxt)
                {
                    TextBox.Select(spos, slen);
                }
                else
                {
                    TextBox.SelectAll();
                }
                TextBox.Focus();

                if (LabelTime != null)
                {
                    var dt = Hot.FirstSpeech + TimeSpan.FromSeconds(SelectedBar.Rect.LT.X);
                    LabelTime.Text = dt.ToString(TimeUtil.FormatHM);
                }
            }
        }

        public int CalsLineCount(RichTextBox text)
        {
            int cnt;
            for( cnt = 0; text.GetFirstCharIndexFromLine(cnt) >= 0; cnt++)
            {
            }
            return cnt;
        }

        public void OnMouseDown(MouseState e)
        {
        }

        public void OnMouseMove(MouseState e)
        {
        }

        public void OnMouseUp(MouseState e)
        {
            if (SelectedParts.Count != 1)
            {
                SelectedBar = null;
                Update();
            }
            else
            {
                foreach (PartsCollectionBase.PartsEntry pe in SelectedParts)
                {
                    SelectedBar = pe.Parts as PartsTalkBar;
                }
                if (SelectedBar != null)
                {
                    Update();
                }
            }
        }

        public void OnMouseWheel(MouseState e)
        {
        }
    }
}