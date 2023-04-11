package com.example.logreg;

import com.google.firebase.database.DatabaseReference;
import com.google.firebase.database.FirebaseDatabase;

public class Trip {
    private String startCity;
    private String endCity;
    private String date;
    private int duration;
    private int price;
    private String driverName;

    public Trip(String startCity, String endCity, String date, int duration, int price, String driverName)
    {
        this.startCity = startCity;
        this.endCity = endCity;
        this.date = date;
        this.duration = duration;
        this.price = price;
        this.driverName = driverName;
    }

    public Trip() {

    }

    public String getStartCity()
    {
        return startCity;
    }

    public void setStartCity(String startCity)
    {
        this.startCity = startCity;
    }

    public String getEndCity()
    {
        return endCity;
    }

    public void setEndCity(String endCity)
    {
        this.endCity = endCity;
    }

    public String getDate()
    {
        return date;
    }

    public void setDate(String date)
    {
        this.date = date;
    }

    public int getDuration()
    {
        return duration;
    }

    public void setDuration(int duration)
    {
        this.duration = duration;
    }

    public int getPrice()
    {
        return price;
    }

    public void setPrice(int price)
    {
        this.price = price;
    }

    public String getDriverName()
    {
        return driverName;
    }

    public void setDriverName(String driverName)
    {
        this.driverName = driverName;
    }

    public void saveToFirebase()
    {
        DatabaseReference database = FirebaseDatabase.getInstance().getReference();
        DatabaseReference tripsRef = database.child("Trips");
        String key = tripsRef.push().getKey();
        tripsRef.child(key).setValue(this);
    }
}