package com.john_inc.remotecontrol;

import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.fragment.app.DialogFragment;

public class ConfirmationDialog extends DialogFragment {

    private final String question;
    private final DialogInterface.OnClickListener yes;
    private final DialogInterface.OnClickListener no;

    public ConfirmationDialog(String question, DialogInterface.OnClickListener yes, DialogInterface.OnClickListener no) {
        this.question = question;
        this.yes = yes;
        this.no = no;
    }

    @NonNull
    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setMessage(question)
                .setPositiveButton(R.string.yes, yes)
                .setNegativeButton(R.string.cancel, no);
        return builder.create();
    }
}
