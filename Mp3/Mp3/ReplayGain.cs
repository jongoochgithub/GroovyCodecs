//========================================================================
// This conversion was produced by the Free Edition of
// Java to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

namespace GroovyCodecs.Mp3.Mp3
{
    internal sealed class ReplayGain
    {

        internal int[] A = new int[(int)(GainAnalysis.STEPS_per_dB * GainAnalysis.MAX_dB)];

        internal int[] B = new int[(int)(GainAnalysis.STEPS_per_dB * GainAnalysis.MAX_dB)];

        internal int first;

        internal int freqindex;

        /// <summary>
        ///     left input samples, with pre-buffer
        /// </summary>
        internal int linpre;

        internal float[] linprebuf = new float[GainAnalysis.MAX_ORDER * 2];

        /// <summary>
        ///     left "out" (i.e. post second filter) samples
        /// </summary>
        internal int lout;

        internal float[] loutbuf = new float[GainAnalysis.MAX_SAMPLES_PER_WINDOW + GainAnalysis.MAX_ORDER];

        /// <summary>
        ///     left "first step" (i.e. post first filter) samples
        /// </summary>
        internal int lstep;

        internal float[] lstepbuf = new float[GainAnalysis.MAX_SAMPLES_PER_WINDOW + GainAnalysis.MAX_ORDER];

        internal double lsum;

        /// <summary>
        ///     right input samples ...
        /// </summary>
        internal int rinpre;

        internal float[] rinprebuf = new float[GainAnalysis.MAX_ORDER * 2];

        internal int rout;

        internal float[] routbuf = new float[GainAnalysis.MAX_SAMPLES_PER_WINDOW + GainAnalysis.MAX_ORDER];

        internal int rstep;

        internal float[] rstepbuf = new float[GainAnalysis.MAX_SAMPLES_PER_WINDOW + GainAnalysis.MAX_ORDER];

        internal double rsum;

        /// <summary>
        ///     number of samples required to reach number of milliseconds required
        ///     for RMS window
        /// </summary>
        internal int sampleWindow;

        internal int totsamp;
    }
}