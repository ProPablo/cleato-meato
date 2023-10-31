using Godot;
using KongrooTools;
using System;

public partial class DialogManager : Singleton<DialogManager>
{

    private DialogRes _currentDialog;
    private Timer _dialogTimer;
    private Panel _dialogPanel;
    private RichTextLabel _dialogLabel;
    private uint _currentLine = 0;

    public override void _Ready()
    {
        _dialogTimer = GetNode<Timer>("Timer");
        _dialogPanel = GetNode<Panel>("Panel");
        _dialogLabel = GetNode<RichTextLabel>("Panel/RichTextLabel");

        _dialogTimer.Timeout += OnDialogTimerTimeout;

        _dialogPanel.Visible = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _dialogTimer.Timeout -= OnDialogTimerTimeout;
    }

    public void OnDialogTimerTimeout()
    {
        _dialogLabel.VisibleCharacters++;
        if (_dialogLabel.VisibleCharacters >= _dialogLabel.Text.Length)
        {
            _dialogTimer.Stop();
        }
    }


    public override void _Process(double delta)
    {
        if (_currentDialog != null)
        {
            if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("attack"))
            {
                if (_dialogLabel.VisibleCharacters < _dialogLabel.Text.Length - 1)
                {
                    _dialogLabel.VisibleCharacters = _dialogLabel.Text.Length;
                }
                else
                    ProceedToNext();
            }
        }
    }

    public void PlayDialog(DialogRes dialog)
    {
        _currentDialog = dialog;
        _dialogPanel.Visible = true;
        _currentLine = 0;
        _dialogLabel.VisibleCharacters = 0;
        _dialogLabel.Text = _currentDialog.Lines[(int)_currentLine];
        _dialogTimer.Start();
    }

    private void ProceedToNext()
    {
        if (_currentLine < _currentDialog.Lines.Count - 1)
        {
            _currentLine++;
            _dialogLabel.Text = _currentDialog.Lines[(int)_currentLine];
            _dialogLabel.VisibleCharacters = 0;
            _dialogTimer.Start();
        }
        else
        {
            _currentDialog = null;
            _dialogPanel.Visible = false;
        }
    }
}
