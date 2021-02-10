package com.john_inc.remotecontrol;

import android.os.Bundle;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import com.john_inc.remotecontrol.commands.Command;
import com.john_inc.remotecontrol.operations.PowerOperations;
import com.john_inc.remotecontrol.operations.ReceiverOperations;
import com.john_inc.remotecontrol.operations.VolumeOperations;

import java.util.concurrent.atomic.AtomicReference;

public class MainActivity extends AppCompatActivity {

    private final Transmitter transmitter = new Transmitter();
    private final NetworkEvents networkEvents = new NetworkEvents(this);

    private final PowerOperations powerOperations =
            new PowerOperations(this);
    private final VolumeOperations volumeOperations =
            new VolumeOperations(this);
    private final ReceiverOperations receiverOperations =
            new ReceiverOperations(this, transmitter);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        volumeOperations.setupVolumeBar();
        receiverOperations.setupSpinner();
        powerOperations.init();
        receiverOperations.onReceiverSelected(() -> {
            powerOperations.selectedReceiver(receiverOperations.selectedReceiver());
            powerOperations.updateNextShutdownTime();
            volumeOperations.updateVolume();
        });
    }

    @Override
    protected void onStart() {
        super.onStart();
        networkEvents.whenNetworkChanges(receiverOperations::updateReceivers);
    }

    @Override
    protected void onStop() {
        super.onStop();
        networkEvents.unregister();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        powerOperations.destroy();
    }

    public void start(View view) {
        powerOperations.start();
    }

    public void shutdown(View view) {
        powerOperations.shutdown();
    }

    public void cancelShutdown(View v) {
        powerOperations.cancelShutdown();
    }

    public void hibernate(View view) {
        powerOperations.hibernate();
    }

    public void decrementVolume(View view) {
        volumeOperations.decrementVolume();
    }

    public void incrementVolume(View view) {
        volumeOperations.incrementVolume();
    }

    public void toggleProgressBar() {
        final ProgressBar progressBar = findViewById(R.id.progressBar);

        progressBar.post(() -> {
            if (progressBar.isShown()) {
                progressBar.setVisibility(View.INVISIBLE);
            } else {
                progressBar.setVisibility(View.VISIBLE);
            }
        });
    }

    public void setErrorMessage(final String message) {
        final TextView textView = findViewById(R.id.textView_errors);
        textView.post(() -> textView.setText(message));
    }

    public String sendCommandAndGetResponse(Command cmd) {
        AtomicReference<String> result = new AtomicReference<>("");
        Thread t = new Thread(() -> {
            toggleProgressBar();
            try {
                result.set(transmitter.sendCommandAndGetResponse(cmd, receiverOperations.selectedReceiver()));
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

    public void sendCommand(Command cmd) {
        Thread t = new Thread(() -> {
            toggleProgressBar();
            try {
                transmitter.sendCommand(cmd, receiverOperations.selectedReceiver());
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
}
