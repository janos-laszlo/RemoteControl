package com.john_inc.remotecontrol;

import android.content.Context;
import android.content.SharedPreferences;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.EditText;
import android.widget.ProgressBar;
import android.widget.SeekBar;
import android.widget.Spinner;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.john_inc.remotecontrol.commands.CancelShutdownDTO;
import com.john_inc.remotecontrol.commands.Command;
import com.john_inc.remotecontrol.commands.CommandWithResponse;
import com.john_inc.remotecontrol.commands.GetNextShutdownCommand;
import com.john_inc.remotecontrol.commands.HibernateCommandDTO;
import com.john_inc.remotecontrol.commands.SetVolumeCommandDTO;
import com.john_inc.remotecontrol.commands.ShutdownCommandDTO;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Locale;
import java.util.Objects;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.atomic.AtomicReference;

public class MainActivity extends AppCompatActivity implements AdapterView.OnItemSelectedListener {

    private static final int SIXTY_SECONDS = 60;
    private final ArrayList<Receiver> receivers = new ArrayList<>();
    private final ArrayList<String> receiverNames = new ArrayList<>();
    private final Transmitter transmitter = new Transmitter();
    private final NetworkEvents networkEvents = new NetworkEvents(this);
    private final AtomicBoolean threadLock = new AtomicBoolean(false);
    private Receiver selectedReceiver;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        setupVolumeBar();
        setupSpinner();
        updateVolumeTextView();
        read();
    }

    @Override
    protected void onStart() {
        super.onStart();
        networkEvents.whenNetworkChanges(this::updateReceivers);
    }

    @Override
    protected void onStop() {
        super.onStop();
        networkEvents.unregister();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        write();
    }

    private void updateReceivers() {
        if (threadLock.get()) {
            return;
        }

        threadLock.set(true);
        new Thread(() -> {
            if (connectedToWifiAfterMaxFiveSeconds()) {
                setErrorMessage("");
            } else {
                threadLock.set(false);
                setErrorMessage("No WIFI connection available");
                return;
            }

            try {
                transmitter.findReceivers(receiver -> {
                    if (!receiverAlreadyAdded(receiver))
                        receivers.add(receiver);
                    loadSpinnerData();
                });
            } catch (Exception e) {
                setErrorMessage(e.getMessage());
            } finally {
                threadLock.set(false);
            }
        }).start();
    }

    private boolean connectedToWifiAfterMaxFiveSeconds() {
        int waitingCycles = 0;
        while (!isConnectedViaWifi() && waitingCycles++ < 5) {
            sleepOneSecond();
        }

        return waitingCycles < 5;
    }

    private boolean isConnectedViaWifi() {
        ConnectivityManager connectivityManager = (ConnectivityManager) getApplication().getSystemService(Context.CONNECTIVITY_SERVICE);
        assert connectivityManager != null;
        NetworkInfo mWifi = connectivityManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);
        assert mWifi != null;
        return mWifi.isConnected();
    }

    private void sleepOneSecond() {
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    private boolean receiverAlreadyAdded(Receiver receiver) {
        for (Receiver r : receivers) {
            if (r.getName().equals(receiver.getName())) {
                return true;
            }
        }

        return false;
    }

    public void start(View view) {
        final EditText macAddress = findViewById(R.id.editText_mac);

        new Thread(() -> {
            String macStr;
            toggleProgressBar();
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
            toggleProgressBar();
        }).start();
    }

    public void shutdown(View view) {
        EditText editTextMinutes = findViewById(R.id.editText_minutes);

        String minutes = editTextMinutes.getText().toString();
        int seconds;
        seconds = minutes.isEmpty() ? 0 : (Integer.parseInt(minutes) * SIXTY_SECONDS);

        if (seconds < SIXTY_SECONDS) {
            new ConfirmationDialog(getString(R.string.shutdown_now),
                    (dialog, which) -> MainActivity.this.sendCommand(new ShutdownCommandDTO(String.valueOf(seconds))),
                    (dialog, which) -> {
                    })
                    .show(getSupportFragmentManager(), "MainActivity");
        } else {
            sendCommand(new ShutdownCommandDTO(String.valueOf(seconds)));
        }

        sleepOneSecond();
        setNextShutdownTime();
    }

    public void cancelShutdown(View v) {
        sendCommand(new CancelShutdownDTO());
        sleepOneSecond();
        setNextShutdownTime();
    }

    public void hibernate(View view) {
        sendCommand(new HibernateCommandDTO());
    }

    public void decreaseVolume(View view) {
        AddToVolume(-1);
    }

    public void increaseVolume(View view) {
        AddToVolume(1);
    }

    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
        String name = parent.getItemAtPosition(position).toString();
        setSelectedReceiver(name);
        setNextShutdownTime();
    }

    @Override
    public void onNothingSelected(AdapterView<?> parent) {
        selectedReceiver = null;
    }

    // Read the IP and MAC from the preference file.
    private void read() {
        EditText mac = findViewById(R.id.editText_mac);

        SharedPreferences sharedPreferences = getSharedPreferences(
                getString(R.string.preference_file_key), Context.MODE_PRIVATE
        );

        String defaultMAC = getString(R.string.device_MAC_default);
        mac.setText(sharedPreferences.getString(getString(R.string.device_MAC), defaultMAC));
    }

    // Write the data to the preference file.
    private void write() {
        EditText mac = findViewById(R.id.editText_mac);

        SharedPreferences sharedPref = getSharedPreferences(
                getString(R.string.preference_file_key), Context.MODE_PRIVATE
        );
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putString(getString(R.string.device_MAC), mac.getText().toString());
        editor.apply();
    }

    private void setupVolumeBar() {
        SeekBar seekBar = findViewById(R.id.seekBar_volume);

        seekBar.setOnSeekBarChangeListener(
                new SeekBar.OnSeekBarChangeListener() {
                    @Override
                    public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                        sendCommand(new SetVolumeCommandDTO(String.valueOf(seekBar.getProgress())));
                        updateVolumeTextView();
                    }

                    @Override
                    public void onStartTrackingTouch(SeekBar seekBar) {
                    }

                    @Override
                    public void onStopTrackingTouch(final SeekBar seekBar) {
                    }
                }
        );
    }

    private void updateVolumeTextView() {
        TextView volumeText = findViewById(R.id.textView_volume);
        SeekBar seekBar = findViewById(R.id.seekBar_volume);
        volumeText.setText(getString(R.string.volume, seekBar.getProgress()));
    }

    private void AddToVolume(int volumeToAdd) {
        SeekBar seekBar = findViewById(R.id.seekBar_volume);
        int newProgress = Math.max(0, seekBar.getProgress() + volumeToAdd);
        newProgress = Math.min(100, newProgress);
        seekBar.setProgress(newProgress);
    }

    private void sendCommand(Command cmd) {
        Thread t = new Thread(() -> {
            toggleProgressBar();
            try {
                transmitter.sendCommand(cmd, selectedReceiver);
                setErrorMessage("");
            } catch (Exception e) {
                setErrorMessage(e.getMessage());
            }
            toggleProgressBar();
        });
        t.start();
        try {
            t.join();
        } catch (InterruptedException e) {
            setErrorMessage(e.getMessage());
        }
    }

    private String sendCommandAndGetResponse(CommandWithResponse cmd) {
        AtomicReference<String> result = new AtomicReference<>("");
        Thread t = new Thread(() -> {
            toggleProgressBar();
            try {
                result.set(transmitter.sendCommand(cmd, selectedReceiver));
                setErrorMessage("");
            } catch (Exception e) {
                setErrorMessage(e.getMessage());
            }
            toggleProgressBar();
        });

        t.start();
        try {
            t.join();
        } catch (InterruptedException e) {
            setErrorMessage(e.getMessage());
        }
        return result.get();
    }

    private void setupSpinner() {
        Spinner spinner = findViewById(R.id.controllable_devices_spinner);
        spinner.setOnItemSelectedListener(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, receiverNames);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
    }

    private void toggleProgressBar() {
        final ProgressBar progressBar = findViewById(R.id.progressBar);

        progressBar.post(() -> {
            if (progressBar.isShown()) {
                progressBar.setVisibility(View.INVISIBLE);
            } else {
                progressBar.setVisibility(View.VISIBLE);
            }
        });
    }

    private synchronized void loadSpinnerData() {
        updateReceiverNames();
        runOnUiThread(new Runnable() {
            @Override
            public synchronized void run() {
                Spinner spinner = findViewById(R.id.controllable_devices_spinner);
                ((BaseAdapter) spinner.getAdapter()).notifyDataSetChanged();
            }
        });
    }

    private void updateReceiverNames() {
        receiverNames.clear();
        for (Receiver receiver : receivers) {
            receiverNames.add(receiver.getName());
        }
    }

    private void setErrorMessage(final String message) {
        final TextView textView = findViewById(R.id.textView_errors);
        textView.post(() -> textView.setText(message));
    }

    private void setSelectedReceiver(String name) {
        selectedReceiver = null;
        for (Receiver receiver : receivers) {
            if (name.equals(receiver.getName())) {
                selectedReceiver = receiver;
            }
        }
    }

    private void setNextShutdownTime() {
        try {

            String nextShutdownTime = getNextShutdownTime();
            TextView nextShutdown = findViewById(R.id.textView_nextShutdown);
            nextShutdown.setText(nextShutdownTime);
        } catch (Exception e) {
            setErrorMessage(e.getMessage());
        }
    }

    private String getNextShutdownTime() throws Exception {
        String response = sendCommandAndGetResponse(new GetNextShutdownCommand());
        if (response.equals("--")) {
            return response;
        } else {
            SimpleDateFormat dateParser = new SimpleDateFormat("MM/dd/yyyy hh:mm:ss", Locale.getDefault());
            SimpleDateFormat dateFormatter = new SimpleDateFormat("HH:mm", Locale.getDefault());
            return dateFormatter.format(Objects.requireNonNull(dateParser.parse(response)));
        }
    }
}
