package com.john_inc.remotecontrol;

public class HibernateCommandDTO implements Command {
    @Override
    public String getName() {
        return "HibernateCommand";
    }
}
