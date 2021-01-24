package com.john_inc.remotecontrol.operations;

import android.widget.TextView;

import com.john_inc.remotecontrol.MainActivity;
import com.john_inc.remotecontrol.R;
import com.john_inc.remotecontrol.commands.GetNextShutdownCommand;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Locale;
import java.util.Objects;

public class NextShutdownOperations {
    private final MainActivity activity;

    public NextShutdownOperations(MainActivity activity) {
        this.activity = activity;
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
}
