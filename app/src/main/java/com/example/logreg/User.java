package com.example.logreg;

import java.lang.reflect.Array;

public class User {
    public String id;
    public String name;
    public String surname;
    public String patronum;
    public String email;
    public String password;
    public String phone_number;
    public String[] array;

    public User(String id, String name, String surname, String patronum, String email, String password, String phone_number,String[] array) {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.patronum = patronum;
        this.email = email;
        this.password = password;
        this.phone_number = phone_number;
    }
    public User()
    {

    }
    public String getPassword()
    {
        return password;
    }
}
