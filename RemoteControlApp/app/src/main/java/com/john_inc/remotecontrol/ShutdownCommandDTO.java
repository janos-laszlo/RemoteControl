package com.john_inc.remotecontrol;

import com.google.gson.annotations.SerializedName;

public class ShutdownCommandDTO implements Command{

    @SuppressWarnings("unused")
    @SerializedName("seconds") private final String seconds;

    ShutdownCommandDTO(String seconds) {
        this.seconds = seconds;
    }

    @Override
    public String getName() {
        return "ShutdownCommand";
    }
}
