package com.john_inc.remotecontrol.commands;

import com.google.gson.annotations.SerializedName;

public class CommandDTO {
    @SerializedName("name") private String name;

    @SerializedName("command") private String command;

    public CommandDTO(String name, String cmd) {
        this.name = name;
        this.command = cmd;
    }
}
