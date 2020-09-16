package com.john_inc.remotecontrol;

import com.google.gson.annotations.SerializedName;

/**
 * Created by John on 11/30/2017.
 */

class CommandDTO {
    @SuppressWarnings("unused")
    @SerializedName("name") private String name;

    @SuppressWarnings("unused")
    @SerializedName("command") private String command;

    CommandDTO(String name, String cmd) {
        this.name = name;
        this.command = cmd;
    }
}
