package com.john_inc.remotecontrol;

import android.content.Context;
import android.content.SharedPreferences;
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
import com.john_inc.remotecontrol.commands.HibernateCommandDTO;
import com.john_inc.remotecontrol.commands.SetVolumeCommandDTO;
import com.john_inc.remotecontrol.commands.ShutdownCommandDTO;

import java.util.ArrayList;

public class MainActivity extends AppCompatActivity implements AdapterView.OnItemSelectedListener {

    private static final int SIXTY_SECONDS = 60;
    private ArrayList<Receiver> controllableDevices = new ArrayList<>();
    private ArrayList<String> controllableDeviceNames = new ArrayList<>();
    private Receiver selectedControllableDevice;
    private Transmitter transmitter = new Transmitter();

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        setupVolumeBar();
        setupSpinner();
        read();
    }

    @Override
    protected void onStart() {
        super.onStart();
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    controllableDevices = transmitter.findControllableDevices();
                    loadSpinnerData();
                } catch (Exception e) {
                    setErrorMessage(e.getMessage());
                }
            }
        }).start();
    }

    @Override
    protected void onStop() {
        super.onStop();
        write();
    }

    public void start(View view) {
        final EditText macAddress = findViewById(R.id.editText_mac);

        new Thread(new Runnable() {
            @Override
            public void run() {
                String macStr;
                toggleProgressBar();
                try {
                    macStr = MagicPacket.cleanMac(macAddress.getText().toString());
                    System.out.println("Sending to: " + macStr);
                    String s = MagicPacket.send(macStr, selectedControllableDevice.getIpAddress().getHostAddress());
                    System.out.println(s);
                } catch (IllegalArgumentException e) {
                    System.out.println(e.getMessage());
                } catch (Exception e) {
                    System.out.println("Failed to send Wake-on-LAN packet:" + e.getMessage());
                }
                toggleProgressBar();
            }
        }).start();
    }

    public void shutdown(View view) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                toggleProgressBar();
                try {
                    EditText editTextMinutes = findViewById(R.id.editText_minutes);

                    String minutes = editTextMinutes.getText().toString();
                    int seconds;
                    seconds = minutes.isEmpty() ? 0 : (Integer.parseInt(minutes) * SIXTY_SECONDS);

                    sendCommand(new ShutdownCommandDTO(String.valueOf(seconds)));
                } catch (Exception e) {
                    setErrorMessage(e.getMessage());
                }
                toggleProgressBar();
            }
        }).start();
    }

    public void cancelShutdown(View v) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                toggleProgressBar();
                try {
                    sendCommand(new CancelShutdownDTO());
                } catch (Exception e) {
                    setErrorMessage(e.getMessage());
                }
                toggleProgressBar();
            }
        }).start();
    }

    public void hibernate(View view) {
        new Thread(new Runnable() {
            @Override
            public void run() {
                toggleProgressBar();
                try {
                    sendCommand(new HibernateCommandDTO());
                } catch (Exception e) {
                    setErrorMessage(e.getMessage());
                }
                toggleProgressBar();
            }
        }).start();
    }

    @Override
    public void onItemSelected(AdapterView<?> parent, View view, int position, long id) {
        String name = parent.getItemAtPosition(position).toString();
        updateSelectedControllableDevice(name);
    }

    @Override
    public void onNothingSelected(AdapterView<?> parent) {
        selectedControllableDevice = null;
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
                    }

                    @Override
                    public void onStartTrackingTouch(SeekBar seekBar) {
                    }

                    @Override
                    public void onStopTrackingTouch(final SeekBar seekBar) {
                        new Thread(new Runnable() {
                            @Override
                            public void run() {
                                toggleProgressBar();
                                try {
                                    sendCommand(new SetVolumeCommandDTO(String.valueOf(seekBar.getProgress())));
                                } catch (Exception e) {
                                    setErrorMessage(e.getMessage());
                                }
                                toggleProgressBar();
                            }
                        }).start();
                        System.out.println("stop tracking, progress = " + seekBar.getProgress());
                    }
                }
        );
    }

    private void sendCommand(Command cmd) {
        toggleProgressBar();
        try {
            transmitter.sendCommand(cmd, selectedControllableDevice);
        } catch (Exception e) {
            setErrorMessage(e.getMessage());
        }
        toggleProgressBar();
    }

    private void setupSpinner() {
        Spinner spinner = findViewById(R.id.controllable_devices_spinner);
        spinner.setOnItemSelectedListener(this);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(this, android.R.layout.simple_spinner_item, controllableDeviceNames);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
    }

    private void toggleProgressBar() {
        final ProgressBar progressBar = findViewById(R.id.progressBar);

        progressBar.post(new Runnable() {
            @Override
            public void run() {
                if (progressBar.isShown()) {
                    progressBar.setVisibility(View.INVISIBLE);
                } else {
                    progressBar.setVisibility(View.VISIBLE);
                }
            }
        });
    }

    private synchronized void loadSpinnerData() {
        updateControllableDeviceNames();
        runOnUiThread(new Runnable() {
            @Override
            public synchronized void run() {
                Spinner spinner = findViewById(R.id.controllable_devices_spinner);
                ((BaseAdapter) spinner.getAdapter()).notifyDataSetChanged();
            }
        });
    }

    private void updateControllableDeviceNames() {
        controllableDeviceNames.clear();
        for (Receiver controllableDevice : controllableDevices) {
            controllableDeviceNames.add(controllableDevice.getName());
        }
    }

    private void setErrorMessage(final String message) {
        final TextView textView = findViewById(R.id.textView_errors);
        textView.post(new Runnable() {
            @Override
            public void run() {
                textView.setText(message);
            }
        });
    }

    private void updateSelectedControllableDevice(String name) {
        selectedControllableDevice = null;
        for (Receiver controllableDevice : controllableDevices) {
            if (name.equals(controllableDevice.getName())) {
                selectedControllableDevice = controllableDevice;
            }
        }
    }
}
