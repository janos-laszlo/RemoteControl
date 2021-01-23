package com.john_inc.remotecontrol.commands;

public class GetNextShutdownCommand implements CommandWithResponse {


    @Override
    public String response() {
        return null;
    }

    @Override
    public String getName() {
        return "GetNextShutdownCommand";
    }
}
