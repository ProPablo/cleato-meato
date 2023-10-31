using Cleato;
using Godot;
using KongrooTools;
using System;

public partial class MeatUI : Singleton<MeatUI>
{
    Label _t1MeatLabel;
    Label _t2MeatLabel;
    VBoxContainer _vbox;
    Label _timeLabel;
    public override void _Ready()
    {
        _t1MeatLabel = GetNode<Label>("VBoxContainer/T1Meat");
        _t2MeatLabel = GetNode<Label>("VBoxContainer/T2Meat");
        _vbox = GetNode<VBoxContainer>("VBoxContainer");
        _timeLabel = GetNode<Label>("Time");
    }

    public override void _Process(double delta)
    {
        if (!GameManager._.IsInHub)
        {
            _vbox.Visible = true;
            _timeLabel.Visible = true;
            _t1MeatLabel.Text = $"GreyCon: {GameManager._.CurrentMeat.Tier1} / {GameManager._.CurrentDayRes.MeatRequirements.Tier1}";
            _t2MeatLabel.Text = $"Choice Cut: {GameManager._.CurrentMeat.Tier2} / {GameManager._.CurrentDayRes.MeatRequirements.Tier2}";
			var timeSpan = TimeSpan.FromSeconds(GameManager._.CurrentTime);

            _timeLabel.Text = $"Time: {timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}:{(timeSpan.Milliseconds / 10):D2}";
        }
        else
        {
            _vbox.Visible = false;
            _timeLabel.Visible = false;
        }
    }
}
