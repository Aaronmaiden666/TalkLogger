﻿// (c) 2020 Manabu Tonosaki
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Linq;
using Tono;
using Tono.GuiWinForm;

namespace TalkLoggerWinform
{
    public class FeatureTalkBar : CoreFeatureBase, ITokenListener
    {
        public NamedId TokenTriggerID
        {
            get
            {
                return TokenSpeechEventQueued;
            }
        }

        public override void OnInitInstance()
        {
            base.OnInitInstance();
            TarPane = Pane.GetPane("GuiViewMain");
        }
        public override void Start(NamedId who)
        {
            base.Start(who);
            while (Hot.SpeechEventQueue.Count > 0)
            {
                var se = Hot.SpeechEventQueue.Dequeue();
                ProcSpeechEvent(se);
            }
        }

        public void ProcSpeechEvent(SpeechEvent se)
        {
            try
            {
                var p2 = (int)(DateTime.Now - Hot.FirstSpeech).TotalSeconds + 1;
                var tar = Parts.GetPartsByLocationID(new Id { Value = se.RowID }).Select(a => (PartsTalkBar)a).Where(a => a.SessionID == se.SessionID).FirstOrDefault();
                switch (se.Action)
                {
                    case SpeechEvent.Actions.Start:
                        ProcStart(se, p2);
                        break;
                    case SpeechEvent.Actions.SetColor:
                        ProcSetColor(se, p2, tar);
                        break;
                    default:
                        ProcUpdate(se, p2, tar);
                        break;
                }
            }
            catch (Exception ex)
            {
                LOG.WriteLineException(ex);
            }
            Pane.Invalidate(null);
        }

        private void ProcUpdate(SpeechEvent se, int totime, PartsTalkBar tar)
        {
            if (tar == null)
                return;

            tar.Text = se.Text;
            tar.Rect = CodeRect.FromLTRB(Math.Min(totime - 1, tar.Rect.LT.X), tar.Rect.LT.Y, totime, tar.Rect.RB.Y);
            if (se.Action == SpeechEvent.Actions.Canceled)
            {
                tar.IsCancelled = true;
            }
        }

        private void ProcSetColor(SpeechEvent se, int totime, PartsTalkBar tar)
        {
            if (tar == null)
                return;

            tar.BarColor = Color.FromArgb(int.Parse(se.Text));
        }

        private void ProcStart(SpeechEvent se, int totime)
        {
            var p1 = (int)(se.TimeGenerated - Hot.FirstSpeech).TotalSeconds;
            if (p1 > totime - 1)
            {
                p1 = totime - 1;
            }

            var pt = new PartsTalkBar
            {
                SessionID = se.SessionID,
                TimeTalkStarted = se.TimeGenerated,
                PartsPositioner = base.TalkPositioner,
                PartsPositionCorder = base.TalkPosCoder,
                Rect = CodeRect.FromLTRB(
                                        l: p1,
                                        r: totime,
                                        t: se.RowID,
                                        b: se.RowID),
            };
            Parts.Add(TarPane, pt, LayerTalkBar);
        }
    }
}
