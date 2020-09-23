﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tono;
using Tono.GuiWinForm;

namespace TalkLoggerWinform
{
    public class FeatureTalkBar : CoreFeatureBase, ITokenListener
    {
        public NamedId TokenTriggerID => TokenSpeechEventQueued;

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
            if (se.Action == SpeechEvent.Actions.Start)
            {
                var p1 = (int)(se.TimeGenerated - Hot.FirstSpeech).TotalSeconds;
                var p2 = (int)(DateTime.Now - Hot.FirstSpeech).TotalSeconds;
                if (p2 < p1 + 3) p2 = p1 + 3;
                var pt = new PartsTalkBar
                {
                    SessionID = se.SessionID,
                    PartsPositioner = base.TalkPositioner,
                    PartsPositionCorder = base.TalkPosCoder,
                    Rect = CodeRect.FromLTRB(
                                            l: p1,
                                            r: p2,
                                            t: se.RowID,
                                            b: se.RowID),
                };
                Parts.Add(TarPane, pt, LayerTalkBar);
                return;
            }
            var tar = Parts.GetPartsByLocationID(new Id { Value = se.RowID }).Select(a => (PartsTalkBar)a).Where(a => a.SessionID == se.SessionID).FirstOrDefault();
            if (tar != null)
            {
                tar.Text = se.Text;
                tar.Rect = CodeRect.FromLTRB(tar.Rect.LT.X, tar.Rect.LT.Y, (int)(DateTime.Now - Hot.FirstSpeech).TotalSeconds, tar.Rect.RB.Y);
                Pane.Invalidate(null);
            }
        }
    }
}