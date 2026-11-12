package com.example.logreg;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SearchView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.firebase.ui.database.FirebaseRecyclerAdapter;
import com.firebase.ui.database.FirebaseRecyclerOptions;
import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.Query;
import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.function.Predicate;

public class SearchActivityPassenger extends Fragment
{
    private RecyclerView recyclerView;
    private DatabaseReference databaseReference;
    private FirebaseRecyclerAdapter<Trip, TripViewHolder> adapterMoscowToBryansk;
    private SearchView searchStartView;
    private SearchView searchEndView;
    private Gson gson;
    private ArrayList<String> cities = new ArrayList<>();
    private ArrayList<String> filteredCities = new ArrayList<>();

    private String startCityQuery = "";
    private String endCityQuery = "";

    public SearchActivityPassenger()
    {
        // пустой конструктор
    }

    public static SearchActivityPassenger newInstance() {
        SearchActivityPassenger fragment = new SearchActivityPassenger();
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        gson = new Gson();
        try {
            InputStream inputStream = getResources().openRawResource(R.raw.tb_city);
            BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));
            Type type = new TypeToken<ArrayList<ArrayList<String>>>() {}.getType();
            ArrayList<ArrayList<String>> data = gson.fromJson(reader, type);
            reader.close();
            inputStream.close();
            for (ArrayList<String> cityData : data)
            {
                cities.add(cityData.get(0));
            }
        }
        catch (IOException e) {
            Toast.makeText(getContext(), "Ошибка получения данных городов", Toast.LENGTH_SHORT).show();
        }
        databaseReference = FirebaseDatabase.getInstance().getReference().child("Trips");

        // set the initial query to retrieve all trips from Moscow to Bryansk
        Query query = databaseReference.orderByChild("startCity_endCity").equalTo("Москва_Брянск");

        FirebaseRecyclerOptions<Trip> options = new FirebaseRecyclerOptions.Builder<Trip>()
                .setQuery(query, Trip.class)
                .build();
        adapterMoscowToBryansk = new FirebaseRecyclerAdapter<Trip, TripViewHolder>(options) {
            @NonNull
            @Override
            public TripViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
                View view = LayoutInflater.from(parent.getContext())
                        .inflate(R.layout.trip_item_search, parent, false);
                return new TripViewHolder(view);
            }
            @Override
            protected void onBindViewHolder(@NonNull TripViewHolder holder, int position, @NonNull Trip model) {
                // filter the trips based on the search query values
                if (!startCityQuery.isEmpty() && !endCityQuery.isEmpty()) {
                    if (model.getStartCity().equalsIgnoreCase(startCityQuery)
                            && model.getEndCity().equalsIgnoreCase(endCityQuery)) {
                        holder.bind(model);
                    }
                    else {
                        holder.itemView.setVisibility(View.GONE);
                        holder.itemView.setLayoutParams(new RecyclerView.LayoutParams(0, 0));
                    }
                } else {
                    holder.bind(model);
                }
            }
        };
        if (recyclerView != null) {
            recyclerView.setAdapter(adapterMoscowToBryansk);
        }
    }
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);
        View view = inflater.inflate(R.layout.activity_search, container, false);
        recyclerView = view.findViewById(R.id.recyclerView);
        searchStartView = view.findViewById(R.id.start_adress);
        searchEndView = view.findViewById(R.id.end_adress);
        ListView citiesListView = view.findViewById(R.id.cities_list_view);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(getActivity(), android.R.layout.simple_list_item_1, filteredCities);
        citiesListView.setAdapter(adapter);
        citiesListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {

            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                String selectedCity = filteredCities.get(position);
                citiesListView.setVisibility(View.VISIBLE);
                // filter the trips based on the search query values and update the adapter
                Predicate<Trip> filterPredicate = trip -> trip.getStartCity().equals(startCityQuery) && trip.getEndCity().equals(endCityQuery);
                //adapterMoscowToBryansk.getFilter().filter(startCityQuery + "_" + endCityQuery);
            }
        });
        // show or hide the cities list view based on the focus of the search views
        View.OnFocusChangeListener focusChangeListener = new View.OnFocusChangeListener() {
            @Override
            public void onFocusChange(View v, boolean hasFocus) {
                if (v == searchStartView || v == searchEndView) {
                    if (hasFocus) {
                        citiesListView.setVisibility(View.VISIBLE);
                    } else {
                        citiesListView.setVisibility(View.GONE);
                    }
                }
            }
        };
        searchStartView.setOnFocusChangeListener(focusChangeListener);
        searchEndView.setOnFocusChangeListener(focusChangeListener);
        return view;
    }
    static class TripViewHolder extends RecyclerView.ViewHolder {
        private TextView startCity;
        private TextView endCity;
        private TextView departureTime;
        private TextView freePlaces;
        private TextView price;

        public TripViewHolder(View itemView) {
            super(itemView);
            startCity = itemView.findViewById(R.id.start_adress);
            endCity = itemView.findViewById(R.id.end_adress);
            departureTime = itemView.findViewById(R.id.durationTextView);
            freePlaces = itemView.findViewById(R.id.passcntBox);
            price = itemView.findViewById(R.id.priceBox);
        }
        public void bind(Trip trip) {
            startCity.setText(trip.getStartCity());
            endCity.setText(trip.getEndCity());
            departureTime.setText(trip.getDuration());
            freePlaces.setText(trip.getPassengers());
            price.setText(trip.getPrice());
        }
    }
    private void filterCities(String text) {
        filteredCities.clear();
        if (text.isEmpty()) {
            filteredCities.addAll(cities);
        } else {
            text = text.toLowerCase();
            for (String city : cities) {
                if (city.toLowerCase().contains(text)) {
                    filteredCities.add(city);
                }
            }
        }
    }
}