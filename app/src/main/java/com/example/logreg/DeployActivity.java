package com.example.logreg;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link DeployActivity#newInstance} factory method to
 * create an instance of this fragment.
 */
public class DeployActivity extends Fragment {

    // TODO: Rename parameter arguments, choose names that match
    // the fragment initialization parameters, e.g. ARG_ITEM_NUMBER
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    // TODO: Rename and change types of parameters
    private String mParam1;
    private String mParam2;

    public DeployActivity()
    {
        // Required empty public constructor
    }

    /**
     * Use this factory method to create a new instance of
     * this fragment using the provided parameters.
     *
     * @param param1 Parameter 1.
     * @param param2 Parameter 2.
     * @return A new instance of fragment SearchActivityPassenger.
     */
    // TODO: Rename and change types and number of parameters
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
        depButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v)
            {
                Trip trip = new Trip("Москва", "Санкт-Петербург", "2023-04-09", 5,1200,"Паша");
                trip.saveToFirebase();
            }
        });
        return view;
    }
}