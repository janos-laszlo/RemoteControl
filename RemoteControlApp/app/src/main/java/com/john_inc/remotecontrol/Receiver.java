package com.john_inc.remotecontrol;

import java.net.InetAddress;

public class Receiver {
    private String name;
    private InetAddress ipAddress;

    Receiver(String name, InetAddress ipAddress) {
        this.name = name;
        this.ipAddress = ipAddress;
    }

    public String getName() {
        return name;
    }

    public InetAddress getIpAddress() {
        return ipAddress;
    }
}
