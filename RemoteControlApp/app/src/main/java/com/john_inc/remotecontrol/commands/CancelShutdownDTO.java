package com.john_inc.remotecontrol.commands;

public class CancelShutdownDTO implements Command {
    @Override
    public String getName() {
        return "CancelShutdownCommand";
    }
}
