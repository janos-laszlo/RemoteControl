package com.john_inc.remotecontrol.commands;

public class GetNextShutdownCommand implements Command {

    @Override
    public String getName() {
        return "GetNextShutdownCommand";
    }
}
