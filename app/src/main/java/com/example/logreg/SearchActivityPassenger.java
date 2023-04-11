package com.example.logreg;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;
import com.yandex.mapkit.Animation;
import com.yandex.mapkit.MapKitFactory;
import com.yandex.mapkit.geometry.Point;
import com.yandex.mapkit.map.CameraPosition;
import com.yandex.mapkit.map.Map;
import com.yandex.mapkit.map.MapObjectCollection;
import com.yandex.mapkit.map.PlacemarkMapObject;
import com.yandex.mapkit.mapview.MapView;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;

import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.SearchView;
import android.widget.Toast;

public class SearchActivityPassenger extends Fragment
{
    private final String MAPKIT_API_KEY = "c2be26a0-5509-40cf-8294-4a6cbf8ca714";
    public MapView mapview;
    private MapObjectCollection mapObjects;
    private SearchView searchStartView;
    private SearchView searchEndView;
    private Gson gson;
    private ArrayList<String> cities = new ArrayList<>();
    private ArrayList<String> filteredCities = new ArrayList<>();
    private EditText editText1, editText2;
    private static final String ARG_PARAM1 = "param1";
    private static final String ARG_PARAM2 = "param2";

    private String mParam1;
    private String mParam2;

    public SearchActivityPassenger()
    {
        // пустой конструктор
    }
    public static SearchActivityPassenger newInstance(String param1, String param2) {
        SearchActivityPassenger fragment = new SearchActivityPassenger();
        return fragment;
    }
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        MapKitFactory.setApiKey(MAPKIT_API_KEY);
        MapKitFactory.initialize(getActivity());
        gson = new Gson();
        try {
            InputStream inputStream = getResources().openRawResource(R.raw.tb_city);
            BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));
            Type type = new TypeToken<ArrayList<ArrayList<String>>>() {}.getType();
            ArrayList<ArrayList<String>> data = gson.fromJson(reader, type);
            reader.close();
            inputStream.close();
            for (ArrayList<String> cityData : data) {
                cities.add(cityData.get(0));
            }
        } catch (IOException e) {
            Toast.makeText(getContext(), "Ошибка получения данных городов", Toast.LENGTH_SHORT).show();
        }
    }
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.activity_search, container,false);
        searchStartView = view.findViewById(R.id.start_adress);
        searchEndView = view.findViewById(R.id.end_adress);
        ListView citiesListView = view.findViewById(R.id.cities_list_view);
        ArrayAdapter<String> adapter = new ArrayAdapter<>(getActivity(),
                android.R.layout.simple_list_item_1, filteredCities);
        citiesListView.setAdapter(adapter);
        citiesListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> adapterView, View view, int i, long l) {
                // получаем текст выбранного элемента
                String selectedCity = filteredCities.get(i);
                // устанавливаем текст в SearchView
                if (searchStartView.hasFocus()) {
                    searchStartView.setQuery(selectedCity, true);
                } else if (searchEndView.hasFocus()) {
                    searchEndView.setQuery(selectedCity, true);
                }
            }
        });
        // слушатель изменений текста для searchStartView
        searchStartView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                filterCities(newText);
                adapter.notifyDataSetChanged();
                return false;
            }
        });
        // слушатель изменений текста для searchEndView
        searchEndView.setOnQueryTextListener(new SearchView.OnQueryTextListener() {
            @Override
            public boolean onQueryTextSubmit(String query) {
                return false;
            }

            @Override
            public boolean onQueryTextChange(String newText) {
                filterCities(newText);
                adapter.notifyDataSetChanged();
                return false;
            }
        });
        return view;
    }
    @Override
    public  void onViewCreated(View view,  Bundle savedInstanceState){
        mapview = (MapView) view.findViewById(R.id.map_yandex);
    }
    @Override
    public void onStop() {
        mapview.onStop();
        MapKitFactory.getInstance().onStop();
        super.onStop();
    }

    @Override
    public void onStart() {
        super.onStart();
        MapKitFactory.getInstance().onStart();
        mapview.onStart();
        mapview.getMap().move(
                new CameraPosition(new Point(55.751574, 37.573856), 11.0f, 0.0f, 0.0f),
                new Animation(Animation.Type.SMOOTH, 0),
                null);
        mapview.getMap().setRotateGesturesEnabled(true);
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