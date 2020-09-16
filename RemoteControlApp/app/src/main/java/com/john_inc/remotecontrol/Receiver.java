package com.john_inc.remotecontrol;

import java.net.InetAddress;

class Receiver {
    private String name;
    private InetAddress ipAddress;

    Receiver(String name, InetAddress ipAddress) {
        this.name = name;
        this.ipAddress = ipAddress;
    }

    String getName() {
        return name;
    }

    InetAddress getIpAddress() {
        return ipAddress;
    }
}
