package com.john_inc.remotecontrol;

import com.google.gson.Gson;
import com.john_inc.remotecontrol.commands.Command;
import com.john_inc.remotecontrol.commands.CommandDTO;

import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.util.concurrent.atomic.AtomicBoolean;

public class Transmitter {
    private static final int SERVER_PORT = 11000;
    private static final int TIMEOUT = 5000;
    private static final String CONNECTION_ERROR = "Connection error!";

    public void sendCommand(Command command, Receiver receiver) throws Exception {
        String s = serializeCommand(command);
        sendMessage(s, receiver);
    }

    private String serializeCommand(Command cmd) {
        Gson serializer = new Gson();
        CommandDTO commandDTO = new CommandDTO(cmd.getName(), serializer.toJson(cmd));
        return serializer.toJson(commandDTO);
    }

    private void sendMessage(final String message, Receiver receiver) throws Exception {
        Socket socket = new Socket();
        try {
            socket.connect(new InetSocketAddress(receiver.getIpAddress(), SERVER_PORT), TIMEOUT);

            if (socket.isConnected()) {
                PrintWriter out = new PrintWriter(new BufferedWriter(
                        new OutputStreamWriter(socket.getOutputStream())
                ), true);
                out.print(message + "\\"); // append the message terminator
                out.flush();
                out.close();
            } else
                throw new Exception(CONNECTION_ERROR);
        } finally {
            socket.close();
        }
    }

    public void findReceivers(OnReceiverFoundListener listener) throws Exception {
        DatagramSocket broadcastSocket = new DatagramSocket();
        broadcastSocket.setBroadcast(true);
        broadcastSocket.setSoTimeout(5000);
        byte[] message = "WhoAreYou".getBytes();
        try {
            DatagramPacket packet = new DatagramPacket(message, message.length, InetAddress.getByName("255.255.255.255"), SERVER_PORT);
            broadcastSocket.send(packet);
        } catch (IOException ex) {
            // Ignore this exception.
        }

        DatagramPacket receivedPacket;
        byte[] receivingBuffer = new byte[256];
        AtomicBoolean receivingResponses = new AtomicBoolean(true);
        boolean anyReceiverFound = false;
        while (receivingResponses.get()) {
            try {
                receivedPacket = new DatagramPacket(receivingBuffer, receivingBuffer.length);
                broadcastSocket.receive(receivedPacket);
                String serverResponse = new String(receivedPacket.getData()).trim();
                if (serverResponse.length() == 0) {
                    receivingResponses.set(false);
                } else {
                    listener.onReceiverFound(new Receiver(serverResponse, receivedPacket.getAddress()));
                    anyReceiverFound = true;
                }
            } catch (SocketTimeoutException e) {
                broadcastSocket.close();
                if (!anyReceiverFound) {
                    throw new Exception(e.getMessage());
                } else receivingResponses.set(false);
            }
        }

        broadcastSocket.close();
    }
}
