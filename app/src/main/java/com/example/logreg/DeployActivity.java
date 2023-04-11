package com.example.logreg;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

public class DeployActivity extends Fragment {
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    private String mParam1;
    private String mParam2;

    public DeployActivity()
    {
        //пустой конструктор
    }
    public static SearchActivityPassenger newInstance(String param1, String param2)
    {
        SearchActivityPassenger fragment = new SearchActivityPassenger();
        Bundle args = new Bundle();
        args.putString(ARG_PARAM1, param1);
        args.putString(ARG_PARAM2, param2);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        if (getArguments() != null)
        {
            mParam1 = getArguments().getString(ARG_PARAM1);
            mParam2 = getArguments().getString(ARG_PARAM2);
        }
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_deploy, container, false);

        Button depButton = view.findViewById(R.id.deployButton);
        depButton.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                Trip trip = new Trip("0001","1000",2, "Гена", "Гоша","Паша","Москва","Санкт-Петербург","2020-20-12",5,1200,"Павел Стрункин",1);
                trip.saveToFirebase();
            }
        });
        return view;
    }
}