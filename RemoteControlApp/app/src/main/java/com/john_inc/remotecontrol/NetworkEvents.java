package com.john_inc.remotecontrol;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.ContextWrapper;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiManager;

import com.john_inc.remotecontrol.listeners.EventListener;

import java.util.Objects;

public class NetworkEvents {

    private ContextWrapper contextWrapper;
    private BroadcastReceiver broadcastReceiver = createBroadcastReceiver();
    private EventListener eventListener = null;

    public NetworkEvents(ContextWrapper contextWrapper) {
        this.contextWrapper = contextWrapper;
    }

    public void whenNetworkChanges(EventListener eventListener) {
        IntentFilter intentFilter = new IntentFilter(WifiManager.NETWORK_STATE_CHANGED_ACTION);
        contextWrapper.registerReceiver(broadcastReceiver, intentFilter);
        this.eventListener = eventListener;
    }

    public void unregister() {
        contextWrapper.unregisterReceiver(broadcastReceiver);
    }

    private BroadcastReceiver createBroadcastReceiver() {
        return new BroadcastReceiver() {
            @Override
            public void onReceive(Context context, Intent intent) {
                if (Objects.requireNonNull(intent.getAction()).equals(WifiManager.NETWORK_STATE_CHANGED_ACTION)) {
                    onNetworkChangedAction(intent);
                }
            }
        };
    }


    private void onNetworkChangedAction(Intent intent) {
        NetworkInfo info = intent.getParcelableExtra(WifiManager.EXTRA_NETWORK_INFO);
        assert info != null;
        if (info.getType() == ConnectivityManager.TYPE_WIFI) {
            eventListener.onEvent();
        }
    }
}
