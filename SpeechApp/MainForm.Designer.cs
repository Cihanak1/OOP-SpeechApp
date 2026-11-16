namespace SpeechApp;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    private GroupBox grpSpeechToText;
    private Button btnStartListening;
    private Button btnStopListening;
    private TextBox txtRecognizedText;
    private GroupBox grpTextToSpeech;
    private TextBox txtTextToSpeak;
    private Button btnSpeak;
    private Button btnStopSpeaking;
    private Label lblStatus;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        grpSpeechToText = new GroupBox();
        txtRecognizedText = new TextBox();
        btnStopListening = new Button();
        btnStartListening = new Button();
        grpTextToSpeech = new GroupBox();
        btnStopSpeaking = new Button();
        btnSpeak = new Button();
        txtTextToSpeak = new TextBox();
        lblStatus = new Label();
        grpSpeechToText.SuspendLayout();
        grpTextToSpeech.SuspendLayout();
        SuspendLayout();

        // grpSpeechToText
        grpSpeechToText.Location = new Point(16, 14);
        grpSpeechToText.Size = new Size(608, 195);
        grpSpeechToText.Text = "Konuşmayı Metne Dönüştürme (Speech-to-Text)";
        grpSpeechToText.Controls.Add(txtRecognizedText);
        grpSpeechToText.Controls.Add(btnStopListening);
        grpSpeechToText.Controls.Add(btnStartListening);

        // btnStartListening
        btnStartListening.Location = new Point(14, 25);
        btnStartListening.Size = new Size(150, 32);
        btnStartListening.Text = "Konuşmayı Başlat";
        btnStartListening.UseVisualStyleBackColor = true;

        // btnStopListening
        btnStopListening.Location = new Point(174, 25);
        btnStopListening.Size = new Size(150, 32);
        btnStopListening.Text = "Konuşmayı Durdur";
        btnStopListening.UseVisualStyleBackColor = true;

        // txtRecognizedText
        txtRecognizedText.Location = new Point(14, 68);
        txtRecognizedText.Size = new Size(578, 112);
        txtRecognizedText.Multiline = true;
        txtRecognizedText.ReadOnly = true;
        txtRecognizedText.ScrollBars = ScrollBars.Vertical;

        // grpTextToSpeech
        grpTextToSpeech.Location = new Point(16, 218);
        grpTextToSpeech.Size = new Size(608, 195);
        grpTextToSpeech.Text = "Metni Seslendirme (Text-to-Speech)";
        grpTextToSpeech.Controls.Add(btnStopSpeaking);
        grpTextToSpeech.Controls.Add(btnSpeak);
        grpTextToSpeech.Controls.Add(txtTextToSpeak);

        // txtTextToSpeak
        txtTextToSpeak.Location = new Point(14, 25);
        txtTextToSpeak.Size = new Size(578, 112);
        txtTextToSpeak.Multiline = true;
        txtTextToSpeak.ScrollBars = ScrollBars.Vertical;
        txtTextToSpeak.Text = "Merhaba, bu bir seslendirme testidir.";

        // btnSpeak
        btnSpeak.Location = new Point(14, 148);
        btnSpeak.Size = new Size(130, 32);
        btnSpeak.Text = "Metni Oku";
        btnSpeak.UseVisualStyleBackColor = true;

        // btnStopSpeaking
        btnStopSpeaking.Location = new Point(154, 148);
        btnStopSpeaking.Size = new Size(150, 32);
        btnStopSpeaking.Text = "Seslendirmeyi Durdur";
        btnStopSpeaking.UseVisualStyleBackColor = true;

        // lblStatus
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(16, 426);
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblStatus.ForeColor = Color.DarkSlateGray;
        lblStatus.Text = "Durum: Hazır";

        // MainForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(642, 458);
        Controls.Add(lblStatus);
        Controls.Add(grpTextToSpeech);
        Controls.Add(grpSpeechToText);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Konuşma Uygulaması (Speech App)";

        grpSpeechToText.ResumeLayout(false);
        grpSpeechToText.PerformLayout();
        grpTextToSpeech.ResumeLayout(false);
        grpTextToSpeech.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}
