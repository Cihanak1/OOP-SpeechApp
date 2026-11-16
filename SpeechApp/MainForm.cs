using System;
using System.Drawing;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace SpeechApp;

public partial class MainForm : Form
{
    private SpeechSynthesizer? synthesizer;
    private SpeechRecognitionEngine? recognizer;
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
                lblStatus.Text = "Durum: Seslendirme tamamlandı.";
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

    private void btnStartListening_Click(object? sender, EventArgs e)
    {
        if (SpeechRecognitionEngine.InstalledRecognizers().Count == 0)
        {
            MessageBox.Show("Bu bilgisayarda konuşma tanıma desteği bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblStatus.Text = "Durum: Konuşma tanıyıcı bulunamadı.";
            return;
        }

        if (isListening)
        {
            return;
        }

        try
        {
            if (recognizer == null)
            {
                recognizer = new SpeechRecognitionEngine();
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.LoadGrammar(new DictationGrammar());
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }

            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            isListening = true;
            lblStatus.Text = "Durum: Dinleniyor, lütfen konuşun...";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Konuşma tanıma başlatılamadı: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "Durum: Dinleme başlatılamadı.";
        }
    }

    private void btnStopListening_Click(object? sender, EventArgs e)
    {
        if (recognizer != null && isListening)
        {
            try
            {
                recognizer.RecognizeAsyncStop();
            }
            catch
            {
            }
            isListening = false;
            lblStatus.Text = "Durum: Dinleme durduruldu.";
        }
    }

    private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(e.Result.Text))
        {
            txtRecognizedText.AppendText(e.Result.Text + Environment.NewLine);
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            synthesizer?.SpeakAsyncCancelAll();
            synthesizer?.Dispose();

            if (isListening)
            {
                recognizer?.RecognizeAsyncStop();
            }
            recognizer?.Dispose();
        }
        catch
        {
        }
    }
}
