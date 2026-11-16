using System;
using System.Windows.Forms;

namespace SpeechApp;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        btnStartListening.Click += btnStartListening_Click;
        btnStopListening.Click += btnStopListening_Click;
        btnSpeak.Click += btnSpeak_Click;
        btnStopSpeaking.Click += btnStopSpeaking_Click;
    }

    private void btnStartListening_Click(object? sender, EventArgs e)
    {
        // Konuşma tanıma işlemi Aşama 2'de eklenecektir
    }

    private void btnStopListening_Click(object? sender, EventArgs e)
    {
        // Konuşmayı durdurma işlemi Aşama 2'de eklenecektir
    }

    private void btnSpeak_Click(object? sender, EventArgs e)
    {
        // Metin seslendirme işlemi Aşama 2'de eklenecektir
    }

    private void btnStopSpeaking_Click(object? sender, EventArgs e)
    {
        // Seslendirmeyi durdurma işlemi Aşama 2'de eklenecektir
    }
}
