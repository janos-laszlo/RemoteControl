package com.john_inc.remotecontrol;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

class MagicPacket
{
    private static final int WAKE_ON_LAN_PORT = 9;
    private static final char SEPARATOR = ':';

    static String send(String mac, String ip) throws IOException, IllegalArgumentException
    {
        // validate MAC and chop into array
        final String[] hex = validateMac(mac);

        // convert to base16 bytes
        final byte[] macBytes = new byte[6];
        for(int i=0; i<6; i++) {
            macBytes[i] = (byte) Integer.parseInt(hex[i], 16);
        }

        final byte[] bytes = new byte[102];

        // fill first 6 bytes
        for(int i=0; i<6; i++) {
            bytes[i] = (byte) 0xff;
        }
        // fill remaining bytes with target MAC
        for(int i=6; i<bytes.length; i+=macBytes.length) {
            System.arraycopy(macBytes, 0, bytes, i, macBytes.length);
        }

        // create socket to IP
        final InetAddress address = InetAddress.getByName(ip);
        final DatagramPacket packet = new DatagramPacket(bytes, bytes.length, address, WAKE_ON_LAN_PORT);
        final DatagramSocket socket = new DatagramSocket();
        socket.send(packet);
        socket.close();

        return hex[0]+SEPARATOR+hex[1]+SEPARATOR+hex[2]+SEPARATOR+hex[3]+SEPARATOR+hex[4]+SEPARATOR+hex[5];
    }

    static String cleanMac(String mac) throws IllegalArgumentException
    {
        final String[] hex = validateMac(mac);

        StringBuffer sb = new StringBuffer();
        boolean isMixedCase = false;

        // check for mixed case
        for(int i=0; i<6; i++) {
            sb.append(hex[i]);
        }
        String testMac = sb.toString();
        if((!testMac.toLowerCase().equals(testMac)) && (!testMac.toUpperCase().equals(testMac))) {
            isMixedCase = true;
        }

        sb = new StringBuffer();
        for(int i=0; i<6; i++) {
            // convert mixed case to lower
            if(isMixedCase) {
                sb.append(hex[i].toLowerCase());
            }else{
                sb.append(hex[i]);
            }
            if(i < 5) {
                sb.append(SEPARATOR);
            }
        }
        return sb.toString();
    }

    private static String[] validateMac(String mac) throws IllegalArgumentException
    {
        // error handle semi colons
        mac = mac.replace(";", ":");

        // attempt to assist the user a little
        StringBuilder newMac = new StringBuilder();

        if(mac.matches("([a-zA-Z0-9]){12}")) {
            // expand 12 chars into a valid mac address
            for(int i=0; i<mac.length(); i++){
                if((i > 1) && (i % 2 == 0)) {
                    newMac.append(":");
                }
                newMac.append(mac.charAt(i));
            }
        }else{
            newMac = new StringBuilder(mac);
        }

        // regexp pattern match a valid MAC address
        final Pattern pat = Pattern.compile("((([0-9a-fA-F]){2}[-:]){5}([0-9a-fA-F]){2})");
        final Matcher m = pat.matcher(newMac.toString());

        if(m.find()) {
            String result = m.group();
            return result.split("([:\\-])");
        }else{
            throw new IllegalArgumentException("Invalid MAC address");
        }
    }
}
