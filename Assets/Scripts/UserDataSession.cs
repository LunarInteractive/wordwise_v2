using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Fajar
/// Class yang digunakan untuk menampung data-data yang digunakan dalam aplikasi.
/// Baik itu class data siap pakai (UserDataSession) maupun data untuk menampung
/// hasil parsing JSON dari database (RegistrationResponse, etc..)
/// </summary>
public static class UserDataSession
{
    public static string id;
    public static string role;
    public static string fullName;
    public static string email;
    public static string schoolName;
    public static string classID;
    public static string className;
    public static string classCode;
    public static string levelID;
    public static string levelName;
    //public static string photoUrl;

    public static bool OnSession { get { return id != null; } }

    public static void CloseSession() 
    {
        if (fullName != null)
        {
            id = null;
            role = null;
            fullName = null;
            email = null;
            schoolName = null;
            className = null;
            classCode = null;
            classID = null;
            //photoUrl = null;


            
        }
        
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class RegistrationResponse
{
    public string message;
    public UserData data;
}

[System.Serializable]
public class UserResponse
{
    public UserData data;
}

[System.Serializable]
public class UserData
{
    public string id;
    public string name;
    public string email;
    public string role;
    public string password;
    public string school;
    public string classCode;
}

[System.Serializable]
public class ClassCodeResponse
{
    public string message;
    public ClassData data;
}

[System.Serializable]
public class ClassData
{
    public string id;
    public string user_id;
    public string class_id;
}

[System.Serializable]
public class UpdateUserPayload
{
    public string user_id;
    public string name;
    public string email;
    public string password;            
    public string password_confirmation;
    public string school;
}

[System.Serializable]
public class SigninResponse
{
    public string message;
    public UserSigninData user;
}

[System.Serializable]
public class UserSigninData
{
    public string id;
    public string role;
    public string name;
    public string email;
    public string school;
    public string email_verified_at;
    public string created_at;
    public string updated_at;
}

[System.Serializable]
public class ClassesResponse
{
    public UserJoinedClassesResponse[] user_joined_classes;
}

[System.Serializable]
public class UserJoinedClassesResponse
{
    public string id;
    public string user_id;
    public string class_id;
    public string created_at;
    public string updated_at;
    public ClassesData classes;
}

[System.Serializable]
public class ClassesData
{
    public string id;
    public string user_id;
    public string class_name;
    public string token;
    public string created_at;
    public string updated_at;
}