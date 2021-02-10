package com.john_inc.remotecontrol.operations;

import android.widget.SeekBar;
import android.widget.TextView;

import com.john_inc.remotecontrol.MainActivity;
import com.john_inc.remotecontrol.R;
import com.john_inc.remotecontrol.commands.GetVolumeCommand;
import com.john_inc.remotecontrol.commands.SetVolumeCommandDTO;

public class VolumeOperations {
    private final MainActivity activity;

    public VolumeOperations(MainActivity activity) {
        this.activity = activity;
    }

    public void setupVolumeBar() {
        SeekBar seekBar = this.activity.findViewById(R.id.seekBar_volume);
        seekBar.setOnSeekBarChangeListener(
                new SeekBar.OnSeekBarChangeListener() {
                    @Override
                    public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                        activity.sendCommand(new SetVolumeCommandDTO(String.valueOf(seekBar.getProgress())));
                        updateVolumeTextView();
                    }

                    @Override
                    public void onStartTrackingTouch(SeekBar seekBar) {
                    }

                    @Override
                    public void onStopTrackingTouch(final SeekBar seekBar) {
                    }
                }
        );
    }

    public void incrementVolume() {
        addToVolume(1);
    }

    public void decrementVolume() {
        addToVolume(-1);
    }

    public void updateVolume() {
        try {
            int volumeLevel = getVolume();
            SeekBar seekBar = this.activity.findViewById(R.id.seekBar_volume);
            seekBar.setProgress(volumeLevel);
            updateVolumeTextView();
            activity.setErrorMessage("");
        } catch (Exception e) {
            activity.setErrorMessage(e.getMessage());
        }
    }

    private void addToVolume(int volumeToAdd) {
        SeekBar seekBar = this.activity.findViewById(R.id.seekBar_volume);
        int newProgress = Math.max(0, seekBar.getProgress() + volumeToAdd);
        newProgress = Math.min(100, newProgress);
        seekBar.setProgress(newProgress);
    }

    private int getVolume() {
        String response = activity.sendCommandAndGetResponse(new GetVolumeCommand());
        return  Integer.parseInt(response);
    }

    private void updateVolumeTextView() {
        TextView volumeText = activity.findViewById(R.id.textView_volume);
        SeekBar seekBar = this.activity.findViewById(R.id.seekBar_volume);
        volumeText.setText(activity.getString(R.string.volume, seekBar.getProgress()));
    }
}
