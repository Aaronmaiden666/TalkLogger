﻿using System.Drawing;
using Tono.GuiWinForm;

namespace TalkLoggerWinform
{
    public class PartsTalkBar : PartsBase, IPartsSelectable
    {
        private static Font FontTalk = new Font("Yu Gothic UI", 8.0f, FontStyle.Regular);
        public string SessionID { get; set; }
        public bool IsSelected { get; set; }
        public override bool Draw(IRichPane rp)
        {
            var sr = GetScRect(rp);
            rp.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(160, Color.DarkGreen)), sr);
            rp.Graphics.DrawString(Text, FontTalk, Brushes.White, new RectangleF(sr.LT.X, sr.LT.Y, sr.Width, sr.Height), new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
            });
            if (IsSelected)
            {
                foreach (var col in new[] {
                    Color.Yellow,
                    Color.FromArgb(160, Color.LightGreen),
                    Color.FromArgb(120, Color.Blue),
                })
                {
                    rp.Graphics.DrawRectangle(new Pen(col), sr);
                    sr.Inflate(1);
                }
            }
            return true;
        }
    }
}