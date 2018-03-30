using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.Extensions;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor;
using OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Editor.Serialization;
using NoteType = OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap.NoteType;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector {
    internal sealed class SldprojReader : DisposableBase, IScoreReader {

        public bool IsStreamingSupported => false;

        public SourceScore ReadSourceScore(Stream stream, string fileName, ReadSourceOptions sourceOptions) {
            var projectVersion = ProjectReader.CheckFormatVersion(fileName);

            ProjectReader projectReader;

            switch (projectVersion) {
                case ProjectVersion.V0_2:
                    projectReader = new SldprojV2Reader();
                    break;
                case ProjectVersion.V0_3:
                case ProjectVersion.V0_3_1:
                    projectReader = new SldprojV3Reader();
                    break;
                case ProjectVersion.V0_4:
                    projectReader = new SldprojV4Reader();
                    break;
                default:
                    throw new FormatException("Unsupported sldproj format version.");
            }

            var project = projectReader.ReadProject(fileName, null);
            var difficulty = (Difficulty)(sourceOptions.ScoreIndex + 1);

            var score = project.GetScore(difficulty);

            if (score == null) {
                throw new FormatException($"Invalid project or invalid score index (\"{sourceOptions.ScoreIndex}\").");
            }

            return ToSourceScore(project, score, sourceOptions);
        }

        public RuntimeScore ReadCompiledScore(Stream stream, string fileName, ReadSourceOptions sourceOptions, ScoreCompileOptions compileOptions) {
            var score = ReadSourceScore(stream, fileName, sourceOptions);

            using (var compiler = new SldprojCompiler()) {
                return compiler.Compile(score, compileOptions);
            }
        }

        protected override void Dispose(bool disposing) {
        }

        [NotNull]
        private static SourceScore ToSourceScore([NotNull] Project project, [NotNull] Score score, [NotNull] ReadSourceOptions options) {
            var result = new SourceScore();

            var sourceNotes = new List<SourceNote>();
            var conductors = new List<Conductor>();

            const int beatsPerMeasure = 4;

            var globalConductor = new Conductor();
            globalConductor.Tempo = (float)project.Settings.BeatPerMinute;
            globalConductor.SignatureNumerator = project.Settings.Signature;
            globalConductor.SignatureDenominator = beatsPerMeasure;

            conductors.Add(globalConductor);

            var allNotes = score.GetAllNotes();

            foreach (var note in allNotes) {
                SourceNote sourceNote = null;
                Conductor conductor = null;

                switch (note.Basic.Type) {
                    case NoteType.TapOrFlick:
                        sourceNote = new SourceNote();
                        FillTapOrFlick(note, sourceNote);
                        break;
                    case NoteType.Hold:
                        if (!note.Helper.IsHoldStart) {
                            continue;
                        }

                        sourceNote = new SourceNote();
                        FillHold(note, sourceNote);
                        break;
                    case NoteType.Slide:
                        if (!note.Helper.IsSlideStart) {
                            continue;
                        }

                        sourceNote = new SourceNote();
                        FillSlide(note, sourceNote);
                        break;
                    case NoteType.VariantBpm: {
                            conductor = new Conductor();

                            FillCommonProperties(note, conductor);

                            Debug.Assert(note.Params != null, "note.Params != null");

                            conductor.Tempo = (float)note.Params.NewBpm;
                            conductor.SignatureNumerator = project.Settings.Signature;
                            conductor.SignatureDenominator = beatsPerMeasure;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (sourceNote != null) {
                    sourceNotes.Add(sourceNote);
                }

                if (conductor != null) {
                    conductors.Add(conductor);
                }
            }

            result.MusicOffset = project.Settings.StartTimeOffset;
            result.TrackCount = CgssTrackCount;
            result.ScoreIndex = options.ScoreIndex;

            sourceNotes.Sort((n1, n2) => n1.Ticks.CompareTo(n2.Ticks));
            conductors.Sort((n1, n2) => n1.Ticks.CompareTo(n2.Ticks));

            result.Notes = sourceNotes.ToArray();
            result.Conductors = conductors.ToArray();

            return result;

            void FillCommonProperties(Note note, NoteBase sourceNote) {
                var bar = note.Basic.Bar;

                sourceNote.Measure = bar.Basic.Index;

                var fraction = (float)note.Basic.IndexInGrid / bar.GetGridPerSignature() / beatsPerMeasure;

                sourceNote.Beat = (int)fraction;

                //sourceNote.GroupID = 0;

                sourceNote.Ticks = 60 * (long)(beatsPerMeasure * (sourceNote.Measure + fraction) * NoteBase.TicksPerBeat);
            }

            void FillSourceNoteProperties(Note note, SourceNote sourceNote) {
                FillCommonProperties(note, sourceNote);

                sourceNote.Size = NoteSize.Small;
                sourceNote.Speed = 1.0f;

                sourceNote.StartX = (float)(note.Basic.StartPosition - 1);
                sourceNote.EndX = (float)(note.Basic.FinishPosition - 1);
                sourceNote.TrackIndex = (int)sourceNote.StartX;

                if (sourceNote.TrackIndex < 0 || sourceNote.TrackIndex > (CgssTrackCount - 1)) {
                    Debug.Print("Warning: Invalid track index \"{0}\", changing into range [0, {1}].", sourceNote.TrackIndex, CgssTrackCount - 1);

                    if (sourceNote.TrackIndex < 0) {
                        sourceNote.TrackIndex = 0;
                    } else if (sourceNote.TrackIndex > (CgssTrackCount - 1)) {
                        sourceNote.TrackIndex = (CgssTrackCount - 1);
                    }
                }

                sourceNote.LeadTime = (float)DefaultLeadTime.TotalSeconds;
            }

            void FillTapOrFlick(Note note, SourceNote sourceNote) {
                FillSourceNoteProperties(note, sourceNote);

                switch (note.Basic.FlickType) {
                    case NoteFlickType.None:
                        sourceNote.Type = MilliSim.Contributed.Scores.NoteType.Tap;
                        sourceNote.FlickDirection = FlickDirection.None;
                        break;
                    case NoteFlickType.Left:
                        sourceNote.Type = MilliSim.Contributed.Scores.NoteType.Flick;
                        sourceNote.FlickDirection = FlickDirection.Left;
                        break;
                    case NoteFlickType.Right:
                        sourceNote.Type = MilliSim.Contributed.Scores.NoteType.Flick;
                        sourceNote.FlickDirection = FlickDirection.Right;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            void FillHold(Note note, SourceNote sourceNote) {
                FillSourceNoteProperties(note, sourceNote);

                sourceNote.Type = MilliSim.Contributed.Scores.NoteType.Hold;

                var holdEnd = new SourceNote();

                Debug.Assert(note.Editor.HoldPair != null, "note.Editor.HoldPair != null");

                FillSourceNoteProperties(note.Editor.HoldPair, holdEnd);

                holdEnd.Type = MilliSim.Contributed.Scores.NoteType.Hold;

                switch (note.Editor.HoldPair.Basic.FlickType) {
                    case NoteFlickType.None:
                        holdEnd.FlickDirection = FlickDirection.None;
                        break;
                    case NoteFlickType.Left:
                        holdEnd.FlickDirection = FlickDirection.Left;
                        break;
                    case NoteFlickType.Right:
                        holdEnd.FlickDirection = FlickDirection.Right;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                sourceNote.FollowingNotes = new[] { holdEnd };
            }

            void FillSlide(Note note, SourceNote sourceNote) {
                FillSourceNoteProperties(note, sourceNote);

                sourceNote.Type = MilliSim.Contributed.Scores.NoteType.Slide;

                var following = new List<SourceNote>();

                var nextSlideNode = note.Editor.NextSlide;

                while (nextSlideNode != null) {
                    var node = new SourceNote();

                    FillSourceNoteProperties(nextSlideNode, node);

                    node.Type = MilliSim.Contributed.Scores.NoteType.Slide;

                    if (nextSlideNode.Helper.IsSlideEnd) {
                        switch (nextSlideNode.Basic.FlickType) {
                            case NoteFlickType.None:
                                node.FlickDirection = FlickDirection.None;
                                break;
                            case NoteFlickType.Left:
                                node.FlickDirection = FlickDirection.Left;
                                break;
                            case NoteFlickType.Right:
                                node.FlickDirection = FlickDirection.Right;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    following.Add(node);

                    nextSlideNode = nextSlideNode.Editor.NextSlide;
                }

                sourceNote.FollowingNotes = following.ToArray();
            }
        }

        private const int CgssTrackCount = 5;

        private static readonly TimeSpan DefaultLeadTime = TimeSpan.FromSeconds(1.4);

    }
}
