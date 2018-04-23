using UnityEngine;
using System.IO;
using System.Collections;
using AudioSynthesis;
using AudioSynthesis.Bank;
using AudioSynthesis.Synthesis;
using AudioSynthesis.Sequencer;
using AudioSynthesis.Midi;
using AudioSynthesis.Midi.Event;

namespace UnityMidi
{
    [RequireComponent(typeof(AudioSource))]
    public class MidiPlayer : MonoBehaviour
    {
        [SerializeField] StreamingAssetResouce bankSource;
        [SerializeField] StreamingAssetResouce midiSource;
        [SerializeField] bool loadOnAwake = true;
        [SerializeField] bool playOnAwake = true;
        [SerializeField] int channel = 1;
        [SerializeField] int sampleRate = 44100;
        [SerializeField] int bufferSize = 1024;
        PatchBank bank;
        MidiFile midi;
        Synthesizer synthesizer;
        AudioSource audioSource;
        MidiFileSequencer sequencer;
        int bufferHead;
        float[] currentBuffer;

        public Transform[] instruments;
        public Transform endZone;

        public AudioSource AudioSource { get { return audioSource; } }

        public MidiFileSequencer Sequencer { get { return sequencer; } }

        public PatchBank Bank { get { return bank; } }

        public MidiFile MidiFile { get { return midi; } }

        public void Awake()
        {
            synthesizer = new Synthesizer(sampleRate, channel, bufferSize, 1);
            sequencer = new MidiFileSequencer(synthesizer);
            audioSource = GetComponent<AudioSource>();
            midiSource.streamingAssetPath = CurrentMidi.path + CurrentMidi.currentMidiTitle;
            bankSource.streamingAssetPath = Application.streamingAssetsPath + "/CSharpSynth/" + CurrentMidi.currentSoundbankTitle;

            if (loadOnAwake)
            {
                LoadBank(new PatchBank(bankSource));
                LoadMidi(new MidiFile(midiSource));
            }

            if (playOnAwake)
            {
                Play();
            }

            for (int i = 0; i < instruments.Length; i++) {
                AddNote(i, instruments[i]);
            }

            foreach (Transform i in instruments) {
                i.gameObject.SetActive(false);
            }

            endZone.position += new Vector3(0,0,midi.Tracks[0].MidiEvents.Length + 50);
        }

        void AddNote(int channel, Transform instrument) {
            int mdata = midi.Tracks[0].MidiEvents.Length;

            for (int x = 0; x < mdata; x++) {
                MidiEvent mEvent = midi.Tracks[0].MidiEvents[x];
                if ((byte)mEvent.Channel == channel) {
                    Vector3 pos = transform.position;
                    Transform clone = Instantiate(instrument, pos + new Vector3(((byte)mEvent.Data1 * 2) -45, 0, x), transform.rotation) as Transform;
                    if (instrument.gameObject.tag != "Pickup" && (byte)mEvent.Data2 > 20) {
                        clone.localScale = new Vector3(clone.localScale.x, (byte)mEvent.Data2 * 0.5f, clone.localScale.z);
                    }
                    //Debug.Log((byte)mEvent.Channel + " / " + (byte)mEvent.Command + " / " + (byte)mEvent.Data1 + " / " + (byte)mEvent.Data2);
                }
            }
        }

        public void LoadBank(PatchBank bank)
        {
            this.bank = bank;
            synthesizer.UnloadBank();
            synthesizer.LoadBank(bank);
        }

        public void LoadMidi(MidiFile midi)
        {
            this.midi = midi;
            sequencer.Stop();
            sequencer.UnloadMidi();
            sequencer.LoadMidi(midi);
        }

        public void Play()
        {
            sequencer.Play();
            audioSource.Play();
        }

        void OnAudioFilterRead(float[] data, int channel)
        {
            Debug.Assert(this.channel == channel);
            int count = 0;
            while (count < data.Length)
            {
                if (currentBuffer == null || bufferHead >= currentBuffer.Length)
                {
                    sequencer.FillMidiEventQueue();
                    synthesizer.GetNext();
                    currentBuffer = synthesizer.WorkingBuffer;
                    bufferHead = 0;
                }
                var length = Mathf.Min(currentBuffer.Length - bufferHead, data.Length - count);
                System.Array.Copy(currentBuffer, bufferHead, data, count, length);
                bufferHead += length;
                count += length;
            }
        }
    }
}
