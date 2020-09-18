package com.john_inc.remotecontrol.commands;

import com.google.gson.annotations.SerializedName;

public class ShutdownCommandDTO implements Command {

    @SerializedName("seconds") private final String seconds;

    public ShutdownCommandDTO(String seconds) {
        this.seconds = seconds;
    }

    @Override
    public String getName() {
        return "ShutdownCommand";
    }
}
