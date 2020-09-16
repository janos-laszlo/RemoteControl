package com.john_inc.remotecontrol;

import com.google.gson.annotations.SerializedName;

class SetVolumeCommandDTO implements Command {

    @SuppressWarnings("unused")
    @SerializedName("percent") private final String percent;

    SetVolumeCommandDTO(String percent) {
        this.percent = percent;
    }

    @Override
    public String getName() {
        return "SetVolumeCommand";
    }
}
