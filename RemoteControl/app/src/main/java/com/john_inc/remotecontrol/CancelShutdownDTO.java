package com.john_inc.remotecontrol;

class CancelShutdownDTO implements Command {
    @Override
    public String getName() {
        return "CancelShutdownCommand";
    }
}
