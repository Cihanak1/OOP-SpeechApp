using System;
using System.Drawing;
using System.IO;
using System.Speech.Synthesis;
using System.Text.Json;
using System.Windows.Forms;
using NAudio.Wave;
using Vosk;

namespace SpeechApp;

public partial class MainForm : Form
{
    private SpeechSynthesizer? synthesizer;

    private Model? voskModel;
    private VoskRecognizer? voskRecognizer;
    private WaveInEvent? waveIn;
    private bool isListening = false;

    public MainForm()
    {
        InitializeComponent();

        InitSynthesizer();

        btnStartListening.Click += btnStartListening_Click;
        btnStopListening.Click += btnStopListening_Click;
        btnSpeak.Click += btnSpeak_Click;
        btnStopSpeaking.Click += btnStopSpeaking_Click;
        FormClosing += MainForm_FormClosing;
    }

    private void InitSynthesizer()
    {
        try
        {
            synthesizer = new SpeechSynthesizer();

            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                if (voice.Enabled && voice.VoiceInfo.Culture.Name.StartsWith("tr", StringComparison.OrdinalIgnoreCase))
                {
                    synthesizer.SelectVoice(voice.VoiceInfo.Name);
                    break;
                }
            }

            synthesizer.SpeakCompleted += (s, e) =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lblStatus.Text = "Durum: Seslendirme tamamlandı."));
                }
                else
                {
                    lblStatus.Text = "Durum: Seslendirme tamamlandı.";
                }
            };
        }
        catch (Exception)
        {
            lblStatus.Text = "Durum: Ses motoru başlatılamadı.";
        }
    }

    private void btnSpeak_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTextToSpeak.Text))
        {
            MessageBox.Show("Lütfen seslendirilecek bir metin girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtTextToSpeak.Focus();
            return;
        }

        if (synthesizer == null)
        {
            MessageBox.Show("Seslendirme motoru hazır değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            synthesizer.SpeakAsyncCancelAll();
            synthesizer.SpeakAsync(txtTextToSpeak.Text.Trim());
            lblStatus.Text = "Durum: Metin seslendiriliyor...";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Seslendirme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnStopSpeaking_Click(object? sender, EventArgs e)
    {
        if (synthesizer != null)
        {
            try
            {
                synthesizer.SpeakAsyncCancelAll();
                lblStatus.Text = "Durum: Seslendirme durduruldu.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Durdurma hatası: " + ex.Message);
            }
        }
    }

    private string FindModelPath()
    {
        string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "model");
        if (Directory.Exists(path1))
        {
            return path1;
        }

        string path2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "model");
        if (Directory.Exists(path2))
        {
            return Path.GetFullPath(path2);
        }

        if (Directory.Exists("model"))
        {
            return Path.GetFullPath("model");
        }

        return string.Empty;
    }

    private void btnStartListening_Click(object? sender, EventArgs e)
    {
        if (isListening)
        {
            return;
        }

        string modelPath = FindModelPath();
        if (string.IsNullOrEmpty(modelPath))
        {
            MessageBox.Show("Vosk model klasörü bulunamadı. Lütfen 'model' klasörünün proje içinde veya programın yanında olduğundan emin olun.", "Model Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblStatus.Text = "Durum: Model klasörü bulunamadı.";
            return;
        }

        try
        {
            if (voskModel == null)
            {
                Vosk.Vosk.SetLogLevel(-1);
                voskModel = new Model(modelPath);
            }

            if (voskRecognizer == null)
            {
                voskRecognizer = new VoskRecognizer(voskModel, 16000.0f);
            }

            if (WaveInEvent.DeviceCount == 0)
            {
                MessageBox.Show("Sistemde kullanılabilir bir mikrofon bulunamadı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatus.Text = "Durum: Mikrofon bulunamadı.";
                return;
            }

            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
            waveIn.BufferMilliseconds = 100;
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;

            waveIn.StartRecording();
            isListening = true;
            lblStatus.Text = "Durum: Dinleniyor, lütfen Türkçe konuşun...";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Konuşma tanıma başlatılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Durum: Dinleme başlatılamadı.";
            StopListening();
        }
    }

    private void btnStopListening_Click(object? sender, EventArgs e)
    {
        StopListening();
    }

    private void StopListening()
    {
        if (isListening && waveIn != null)
        {
            try
            {
                waveIn.StopRecording();
            }
            catch
            {
            }
        }
        isListening = false;
        lblStatus.Text = "Durum: Dinleme durduruldu.";
    }

    private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
    {
        if (voskRecognizer == null || !isListening)
        {
            return;
        }

        try
        {
            if (voskRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                string jsonResult = voskRecognizer.Result();
                string recognizedText = ExtractTextFromJson(jsonResult);
                if (!string.IsNullOrWhiteSpace(recognizedText))
                {
                    AppendRecognizedText(recognizedText);
                }
            }
        }
        catch
        {
        }
    }

    private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
    {
        if (voskRecognizer != null)
        {
            try
            {
                string finalJson = voskRecognizer.FinalResult();
                string finalWord = ExtractTextFromJson(finalJson);
                if (!string.IsNullOrWhiteSpace(finalWord))
                {
                    AppendRecognizedText(finalWord);
                }
            }
            catch
            {
            }
        }

        if (waveIn != null)
        {
            waveIn.DataAvailable -= WaveIn_DataAvailable;
            waveIn.RecordingStopped -= WaveIn_RecordingStopped;
            waveIn.Dispose();
            waveIn = null;
        }
    }

    private string ExtractTextFromJson(string json)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("text", out JsonElement textElement))
            {
                return textElement.GetString() ?? string.Empty;
            }
        }
        catch
        {
        }
        return string.Empty;
    }

    private void AppendRecognizedText(string text)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => AppendRecognizedText(text)));
            return;
        }

        txtRecognizedText.AppendText(text + Environment.NewLine);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            synthesizer?.SpeakAsyncCancelAll();
            synthesizer?.Dispose();

            StopListening();

            voskRecognizer?.Dispose();
            voskModel?.Dispose();
        }
        catch
        {
        }
    }
}
