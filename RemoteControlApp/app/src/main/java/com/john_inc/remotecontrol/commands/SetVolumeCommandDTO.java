package com.john_inc.remotecontrol.commands;

import com.google.gson.annotations.SerializedName;

public class SetVolumeCommandDTO implements Command {

    @SerializedName("percent") private final String percent;

    public SetVolumeCommandDTO(String percent) {
        this.percent = percent;
    }

    @Override
    public String getName() {
        return "SetVolumeCommand";
    }
}
