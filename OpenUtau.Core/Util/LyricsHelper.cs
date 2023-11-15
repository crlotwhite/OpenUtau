﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenUtau.Api;
using OpenUtau.Core.G2p;

namespace OpenUtau.Core.Util {
    public interface ILyricsHelper {
        string Source { get; }
        string Convert(string lyric);
    }

    public class ActiveLyricsHelper : SingletonBase<ActiveLyricsHelper> {
        public ILyricsHelper? Current { get; private set; }

        public ActiveLyricsHelper() {
            Set(GetPreferred());
        }

        public void Set(Type? t) {
            if (t == null || !Available.Contains(t)) {
                Current = null;
                return;
            }
            Current = Activator.CreateInstance(t) as ILyricsHelper;
        }

        public Type GetPreferred() {
            return Available.FirstOrDefault(
                avail => avail.Name == Preferences.Default.LyricHelper)
                ?? typeof(HiraganaLyricsHelper);
        }

        public readonly List<Type> Available = new List<Type>() {
            typeof(HiraganaLyricsHelper),
            typeof(PinyinLyricsHelper),
            typeof(JyutpingLyricsHelper),
            typeof(ArpabetG2pLyricsHelper),
            typeof(FrenchG2pLyricsHelper),
            typeof(GermanG2pLyricsHelper),
            typeof(ItalianG2pLyricsHelper),
            typeof(PortugueseG2pLyricsHelper),
            typeof(RussianG2pLyricsHelper),
            typeof(SpanishG2pLyricsHelper),
        };
    }

    public class HiraganaLyricsHelper : ILyricsHelper {
        public string Source => "a->あ";
        public string Convert(string text) {
            return WanaKanaNet.WanaKana.ToHiragana(text);
        }
    }

    public class PinyinLyricsHelper : ILyricsHelper {
        public string Source => "汉->han";
        public string Convert(string lyric) {
            var zhG2p = ZhG2p.GetMandarinInstance();
            var pinyinRes = zhG2p.Convert(lyric, false, true);
            return pinyinRes;
        }
    }

    public class JyutpingLyricsHelper : ILyricsHelper {
        public string Source => "粤->jyut";
        public string Convert(string lyric) {
            var zhG2p = ZhG2p.GetCantoneseInstance();
            var jyutpingRes = zhG2p.Convert(lyric, false, true);
            return jyutpingRes;
        }
    }

    public abstract class G2pLyricsHelper : ILyricsHelper {
        readonly G2pPack pack;
        public G2pLyricsHelper(G2pPack pack) {
            this.pack = pack;
        }
        public string Source => pack.GetType().Name;
        public string Convert(string lyric) {
            var result = pack.Query(lyric);
            if (result == null || result.Length == 0) {
                return null;
            }
            return string.Join(" ", pack.Query(lyric));
        }
    }

    public class ArpabetG2pLyricsHelper : G2pLyricsHelper {
        public ArpabetG2pLyricsHelper() : base(new ArpabetG2p()) { }
    }

    public class FrenchG2pLyricsHelper : G2pLyricsHelper {
        public FrenchG2pLyricsHelper() : base(new FrenchG2p()) { }
    }

    public class GermanG2pLyricsHelper : G2pLyricsHelper {
        public GermanG2pLyricsHelper() : base(new GermanG2p()) { }
    }

    public class ItalianG2pLyricsHelper : G2pLyricsHelper {
        public ItalianG2pLyricsHelper() : base(new ItalianG2p()) { }
    }

    public class PortugueseG2pLyricsHelper : G2pLyricsHelper {
        public PortugueseG2pLyricsHelper() : base(new PortugueseG2p()) { }
    }

    public class RussianG2pLyricsHelper : G2pLyricsHelper {
        public RussianG2pLyricsHelper() : base(new RussianG2p()) { }
    }

    public class SpanishG2pLyricsHelper : G2pLyricsHelper {
        public SpanishG2pLyricsHelper() : base(new SpanishG2p()) { }
    }
}

