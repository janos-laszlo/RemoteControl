package com.john_inc.remotecontrol.operations;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.Spinner;

import com.john_inc.remotecontrol.MainActivity;
import com.john_inc.remotecontrol.R;
import com.john_inc.remotecontrol.Receiver;
import com.john_inc.remotecontrol.Transmitter;
import com.john_inc.remotecontrol.listeners.EventListener;

import java.util.ArrayList;
import java.util.concurrent.atomic.AtomicBoolean;

public class ReceiverOperations implements AdapterView.OnItemSelectedListener {

    private final MainActivity activity;
    private final Transmitter transmitter;

    private final ArrayList<Receiver> receivers = new ArrayList<>();
    private final ArrayList<String> receiverNames = new ArrayList<>();
    private final AtomicBoolean threadLock = new AtomicBoolean(false);
    private Receiver selectedReceiver;
    private EventListener receiverSelectedListener;

    public ReceiverOperations(MainActivity activity, Transmitter transmitter) {
        this.activity = activity;
        this.transmitter = transmitter;
    }

    public void updateReceivers() {
        if (threadLock.get()) {
            return;
        }

        threadLock.set(true);
        new Thread(() -> {
            if (connectedToWifiAfterMaxTenSeconds()) {
                activity.setErrorMessage("");
            } else {
                threadLock.set(false);
                activity.setErrorMessage("No WIFI connection available");
                return;
            }

            try {
                transmitter.findReceivers(receiver -> {
                    if (!receiverAlreadyAdded(receiver))
                        receivers.add(receiver);
                    loadSpinnerData();
                });
            } catch (Exception e) {
                activity.setErrorMessage(e.getMessage());
            } finally {
                threadLock.set(false);
            }
        }).start();
    }

    public void setupSpinner() {
        Spinner spinner = activity.findViewById(R.id.controllable_devices_spinner);
        spinner.setOnItemSelectedListener(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(activity, android.R.layout.simple_spinner_item, receiverNames);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
    }

    public void setSelectedReceiver(String name) {
        selectedReceiver(null);
        for (Receiver receiver : receivers) {
            if (name.equals(receiver.getName())) {
                selectedReceiver(receiver);
            }
        }
    }

    public Receiver selectedReceiver() {
        return selectedReceiver;
    }

    public void selectedReceiver(Receiver receiver) {
        selectedReceiver = receiver;
    }

    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
        String name = parent.getItemAtPosition(position).toString();
        setSelectedReceiver(name);
        receiverSelectedListener.onEvent();
    }

    @Override
    public void onNothingSelected(AdapterView<?> parent) {
        selectedReceiver(null);
    }

    private synchronized void loadSpinnerData() {
        updateReceiverNames();
        activity.runOnUiThread(new Runnable() {
            @Override
            public synchronized void run() {
                Spinner spinner = activity.findViewById(R.id.controllable_devices_spinner);
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

    private boolean connectedToWifiAfterMaxTenSeconds() {
        int waitingCycles = 0;
        while (!isConnectedViaWifi() && waitingCycles++ < 10) {
            sleepOneSecond();
        }

        return waitingCycles < 10;
    }

    private boolean isConnectedViaWifi() {
        ConnectivityManager connectivityManager = (ConnectivityManager) activity.getApplication().getSystemService(Context.CONNECTIVITY_SERVICE);
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

    public void onReceiverSelected(EventListener listener) {
        this.receiverSelectedListener = listener;
    }
}
