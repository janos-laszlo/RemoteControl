package com.john_inc.remotecontrol.commands;

public interface CommandWithResponse extends Command {
    String response();
}
