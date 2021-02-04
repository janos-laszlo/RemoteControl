package com.john_inc.remotecontrol.operations;

import android.content.Context;
import android.content.SharedPreferences;
import android.widget.EditText;
import android.widget.TextView;

import com.john_inc.remotecontrol.ConfirmationDialog;
import com.john_inc.remotecontrol.MagicPacket;
import com.john_inc.remotecontrol.MainActivity;
import com.john_inc.remotecontrol.R;
import com.john_inc.remotecontrol.Receiver;
import com.john_inc.remotecontrol.commands.CancelShutdownDTO;
import com.john_inc.remotecontrol.commands.GetNextShutdownCommand;
import com.john_inc.remotecontrol.commands.HibernateCommandDTO;
import com.john_inc.remotecontrol.commands.ShutdownCommandDTO;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Locale;
import java.util.Objects;

public class PowerOperations {

    private static final int SIXTY_SECONDS = 60;
    private final MainActivity activity;
    private Receiver selectedReceiver;

    public PowerOperations(MainActivity activity) {
        this.activity = activity;
    }

    public void selectedReceiver(Receiver receiver) {
        selectedReceiver = receiver;
    }

    public void initNextShutdownTime() {
        try {
            String nextShutdownTime = getNextShutdownTime();
            TextView nextShutdown = activity.findViewById(R.id.textView_nextShutdown);
            nextShutdown.setText(nextShutdownTime);
            activity.setErrorMessage("");
        } catch (Exception e) {
            activity.setErrorMessage(e.getMessage());
        }
    }

    public void setNextShutdownTime(int secondsFromNow) {
        try {
            Calendar calendar = Calendar.getInstance();
            calendar.add(Calendar.SECOND, secondsFromNow);
            SimpleDateFormat dateFormatter = new SimpleDateFormat("HH:mm", Locale.getDefault());
            setNextShutdownTimeText(dateFormatter.format(calendar.getTime()));
            activity.setErrorMessage("");
        } catch (Exception e) {
            activity.setErrorMessage(e.getMessage());
        }
    }

    public void clearNextShutdownTime() {
        try {
            setNextShutdownTimeText("--");
            activity.setErrorMessage("");
        } catch (Exception e) {
            activity.setErrorMessage(e.getMessage());
        }
    }

    private void setNextShutdownTimeText(String text) {
        TextView nextShutdown = activity.findViewById(R.id.textView_nextShutdown);
        nextShutdown.setText(text);
    }

    private String getNextShutdownTime() throws Exception {
        String response = activity.sendCommandAndGetResponse(new GetNextShutdownCommand());
        if (response.equals("--")) {
            return response;
        } else {
            SimpleDateFormat dateParser = new SimpleDateFormat("MM/dd/yyyy hh:mm:ss a", Locale.getDefault());
            SimpleDateFormat dateFormatter = new SimpleDateFormat("HH:mm", Locale.getDefault());
            return dateFormatter.format(Objects.requireNonNull(dateParser.parse(response)));
        }
    }

    public void shutdown() {
        EditText editTextMinutes = activity.findViewById(R.id.editText_minutes);

        String minutes = editTextMinutes.getText().toString();
        int seconds;
        seconds = minutes.isEmpty() ? 0 : (Integer.parseInt(minutes) * SIXTY_SECONDS);

        if (seconds < SIXTY_SECONDS) {
            new ConfirmationDialog(activity.getString(R.string.shutdown_now),
                    (dialog, which) -> shutdownIn(seconds),
                    (dialog, which) -> {
                    })
                    .show(activity.getSupportFragmentManager(), "MainActivity");
        } else {
            shutdownIn(seconds);
        }
    }

    public void cancelShutdown() {
        activity.sendCommand(new CancelShutdownDTO());
        clearNextShutdownTime();
    }

    public void hibernate() {
        activity.sendCommand(new HibernateCommandDTO());
    }

    public void start() {
        final EditText macAddress = activity.findViewById(R.id.editText_mac);

        new Thread(() -> {
            String macStr;
            activity.toggleProgressBar();
            try {
                macStr = MagicPacket.cleanMac(macAddress.getText().toString());
                System.out.println("Sending to: " + macStr);
                String s = MagicPacket.send(macStr, selectedReceiver.getIpAddress().getHostAddress());
                System.out.println(s);
            } catch (IllegalArgumentException e) {
                System.out.println(e.getMessage());
            } catch (Exception e) {
                System.out.println("Failed to send Wake-on-LAN packet:" + e.getMessage());
            }
            activity.toggleProgressBar();
        }).start();
    }

    public void init() {
        read();
    }

    public void destroy() {
        write();
    }

    private void shutdownIn(int seconds) {
        activity.sendCommand(new ShutdownCommandDTO(String.valueOf(seconds)));
        setNextShutdownTime(seconds);
    }

    // Read the IP and MAC from the preference file.
    private void read() {
        EditText mac = activity.findViewById(R.id.editText_mac);

        SharedPreferences sharedPreferences = activity.getSharedPreferences(
                activity.getString(R.string.preference_file_key), Context.MODE_PRIVATE
        );

        String defaultMAC = activity.getString(R.string.device_MAC_default);
        mac.setText(sharedPreferences.getString(activity.getString(R.string.device_MAC), defaultMAC));
    }

    // Write the data to the preference file.
    private void write() {
        EditText mac = activity.findViewById(R.id.editText_mac);

        SharedPreferences sharedPref = activity.getSharedPreferences(
                activity.getString(R.string.preference_file_key), Context.MODE_PRIVATE
        );
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putString(activity.getString(R.string.device_MAC), mac.getText().toString());
        editor.apply();
    }
}
