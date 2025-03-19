using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary> Fajar
/// Fitur pengecekan isi form. Bisa dipakai untuk sign in, sign up, change pass, edit profile
/// Juga dipakai untuk mengirim isi form ke middleware.
/// Khusus untuk password dan email ada pengecekan khusus agar data yang diinput lebih rapih
/// dan aman.
/// </summary>
public class FormChecker : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] GameObject warningPanel;
    [SerializeField] GameObject processPanel;
    [SerializeField] GameObject submitPanel;
    [SerializeField] TMP_InputField fullName;
    [SerializeField] public TMP_InputField email;
    [SerializeField] public TMP_InputField oldPassword;
    [SerializeField] public TMP_InputField password;
    [SerializeField] TMP_InputField repeatPassword;
    [SerializeField] TMP_InputField schoolName;
    [SerializeField] TMP_InputField className;
    [SerializeField] FileUploader profilePhoto;
    [SerializeField] GameObject transitionToScene; //Diperlukan jika ingin pindah scene setelah submit sukses

    // Menentukan perilaku form saat submit dipanggil
    public enum FormType { signup, signin, edit, changePass };
    [Header("Form Type")]
    [SerializeField] public FormType formType = FormType.signup;
    [SerializeField] GameObject connectionType;

    // Setting warna untuk penanda form saat salah dan form saat reset
    [Header("Color Setting")]
    [SerializeField] Color normalPlaceholderColor = new Color32(0X71, 0X71, 0X71, 0XFF);
    [SerializeField] Color normalImageColor = Color.white;
    [SerializeField] Color emptyPlaceholderColor = Color.white;
    [SerializeField] Color emptyImageColor = new Color32(0XFD, 0XA4, 0X1C, 0XFF);

    // Untuk menyimpan data inputField yang masih tidak diisi atau diisi dengan salah
    List<TMP_InputField> errorInputs = new List<TMP_InputField>();

    // Pemeriksaan akhir dan pengiriman data
    public void CheckSubmit()
    {
        // Pengecekan form kosong
        CheckAvailability(fullName);
        CheckAvailability(email);
        CheckAvailability(oldPassword);
        CheckAvailability(password);
        CheckAvailability(repeatPassword);
        CheckAvailability(schoolName);
        CheckAvailability(className);

        // Jika isi errorInputs sudah kosong, maka baru bisa submit
        //Debug.Log(errorInputs.Count);
        if (errorInputs.Count > 0)
        {
            // Jika masih ada error terkait password/email,
            // error terkait form kosong ga akan ditampilin dulu
            if (!warningPanel.activeInHierarchy)
            {
                ShowWarning("Please fill out all required fields correctly.");
            }
            
        }
        else
        {
            // Perilaku submit dan jenis koneksi ke database sesuai jenis form
            switch(formType){
                case FormType.signup:
                    ShowProcess("Registering your data...");
                    connectionType.GetComponent<UserRegistration>().StartConnecting(fullName.text, email.text, password.text, repeatPassword.text, schoolName.text);
                    break;
                case FormType.signin:
                    ShowProcess("Signing in...");
                    connectionType.GetComponent<SignIn>().StartConnecting(email.text, password.text);
                    break;
                case FormType.edit:
                    ShowProcess("Updating your data...");
                    //connectionType.GetComponent<UserUpdate>().StartConnecting(fullName.text, email.text, schoolName.text);
                    connectionType.GetComponent<UserUpdate>().StartConnecting(fullName.text, schoolName.text);
                    break;
                case FormType.changePass:
                    ShowProcess("Changing your password...");
                    //connectionType.GetComponent<ChangePass>().StartConnecting(oldPassword.text, password.text, repeatPassword.text);
                    connectionType.GetComponent<ChangePass>().StartConnecting(password.text, repeatPassword.text);
                    break;
            }

        }

    }

    // Pengecekan apakah elemen UI tersedia atau tidak
    // dan jika ada, apakah sudah terisi atau belum
    private bool CheckAvailability(TMP_InputField field)
    {
        if (field != null) 
        {

            if (string.IsNullOrEmpty(field.text))
            {
                
                MarkField(field);
                
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    // Reset warna inputfield saat ditekan.
    public void ResetColor(TMP_InputField field)
    {
        field.GetComponent<Image>().color = normalImageColor;
        field.transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().color = normalPlaceholderColor;
    }

    // Mengecek persyaratan password setelah user selesai mengisi inputfield. Persyaratan mungkin terlalu ketat. Lihat keterangan di bawah.
    public void CheckPassword(TMP_InputField passwordInput) 
    {
        // catatan: sebelumnya fungsi ini memeriksa apakah inputField ini contentType nya password atau bukan
        // Namun, pengecekan dihapus agar password bisa disubmit saat user menekan tombol show di samping password

        // Warning label agar peringatan lebih fleksibel dan informatif
        string warningLabel = "";
        if (!string.IsNullOrEmpty(passwordInput.text))
        {

            if  (passwordInput.text.Length < 6) // minimal 6 karakter
            {
                warningLabel = warningLabel + "6 characters min., ";
            }

            if (!passwordInput.text.Any(char.IsUpper)) // minimal ada 1 kapital
            {

                warningLabel = warningLabel + "a capital, ";
            }

            if (!passwordInput.text.Any(char.IsLower)) // minimal ada 1 huruf kecil
            {

                warningLabel = warningLabel + "a lower case, ";
            }

            if (!passwordInput.text.Any(char.IsNumber)) // minimal ada 1 angka
            {
                warningLabel = warningLabel + "a number, ";
            }
            
            // Untuk penjelasan mengenai Symbol dan Punctuation, cari UnicodeCategory
            // ^ dan $ adalah symbol. * dan @ termasuk punctuation
            // Tapi kedua kategori diinformasikan ke user sebagai symbol saja
            if ((!passwordInput.text.Any(char.IsSymbol))&&(!passwordInput.text.Any(char.IsPunctuation))) // Symbol dan punctuation
            {
                
                warningLabel = warningLabel + "a symbol, ";
            }

            if (passwordInput.text.Any(char.IsSeparator)) // Tidak boleh ada spasi atau whitespace
            {
                warningLabel = warningLabel + "no space(s), ";
            }

            // Cek lagi apakah password sama dengan repeat password
            // Dicek di awal biar panelnya ketimpa dengan pengecekan 
            if (repeatPassword != null)
            {
                CheckRepeatPassword();
            }

            if (!string.IsNullOrEmpty(warningLabel))
            {
                // warning yang ditampilkan akan menampilkan kekurangan password yang diisi user
                // karena tiap peringatan diakhiri koma, maka akhir warningLabel dipotong 2 karakter
                // untuk memotong koma di akhir. Setelah koma terakhir, ditambahi penghubung 'and'
                warningLabel = "Make sure your password has: " + warningLabel.Substring(0, warningLabel.Length-2) + ".";
                int lastComma = warningLabel.LastIndexOf(',');
                warningLabel = warningLabel.Insert(lastComma+1, " and");
                ShowWarning(warningLabel);
                MarkField(passwordInput);
            }


        }

    }

    // cek jika repeat password dan password tidak sama setelah diisi
    public void CheckRepeatPassword()
    {
        if ((!string.IsNullOrEmpty(repeatPassword.text)) && (repeatPassword.text != password.text))
        {
            ShowWarning("Please repeat your password in this field.");
            MarkField(repeatPassword);
        }
        else
        {
            // Ini agar jika pengecekan dilakukan dari CheckPassword dan hasilnya oke
            // pastikan repeatPassword warnanya direset dan dikeluarkan dari ErrorList
            ResetColor(repeatPassword);
            DecreaseErrorField(repeatPassword);
            
        }
    }

    // Cek format email setelah input field diisi. Tidak terlalu strict.
    // Hanya memastikan ada @ dan minimal ada titik setelah @
    public void CheckEmail()
    {
        if ((email.contentType == TMP_InputField.ContentType.EmailAddress) && (!string.IsNullOrEmpty(email.text)))
        {

            bool part0 = email.text.Split("@").Length > 1;
            string part1 = "";
            int part2 = 0;

            if (part0)
            {
                part1 = email.text.Split("@")[1];
                part2 = part1.Split(".").Length;
            }
            
            if (part2 > 1)
            {
                try
                {
                    MailAddress testMail = new MailAddress(email.text);
                }
                catch (FormatException)
                {
                    ShowWarning("Please use proper email format, e.g. student@gmail.com");
                    MarkField(email);
                }
            }
            else
            {
                ShowWarning("Please use proper email format, e.g. student@gmail.com");
                MarkField(email);
            }
                
        }
    }

    // Tandai form yang kosong atau diisi dengan keliru
    // dan masukkan dalam List errorInputs jika belum ada
    public void MarkField(TMP_InputField field)
    {
        field.GetComponent<Image>().color = emptyImageColor;
        field.transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TMP_Text>().color = emptyPlaceholderColor;        

        if (!errorInputs.Exists(x => x == field))
        {
            //Debug.Log("yeah this " + field.name + " is problematic");
            errorInputs.Add(field);
            //Debug.Log(errorInputs.Count);
        }

    }

    // Menampilkan panel warning dan mengatur isi keterangannya
    public void ShowWarning(string message)
    {
        if (processPanel)
        {
            processPanel.SetActive(false);
        }
        warningPanel.SetActive(true);
        warningPanel.GetComponent<WarningPanel>().SetContent(message);
    }

    // Menampilkan panel hasil proses dan mengatur isi keterangannya
    // Jika panel hasil tidak memiliki button, maka akan langsung
    // pindah scene (sesuai transitionToScene).
    public void ShowSubmit(string message)
    {
        if (processPanel)
        {
            processPanel.SetActive(false);
        }
        submitPanel.SetActive(true);
        submitPanel.GetComponent<WarningPanel>().SetContent(message);

        if (submitPanel.GetComponentInChildren<Button>() == null)
        {
            transitionToScene.gameObject.SetActive(true);
        }
    }

    // Menampilkan panel keterangan proses dan mengatur isi keterangannya
    public void ShowProcess(string message)
    {
        processPanel.SetActive(true);
        processPanel.GetComponent<WarningPanel>().SetContent(message);
    }

    // Dipanggil setelah user mengisi inputField.
    // Digunakan untuk mengeluarkan inputField dari errorInputs jika ada.
    // Pada form email, password, dan repeat password, inputField
    // akan langsung dimasukkan lagi ke list errorInputs jika tidak
    // lolos pengecekan
    public void DecreaseErrorField(TMP_InputField field)
    {
        if(errorInputs.Exists(x => x == field)){
            //Debug.Log("yeah this " + field.name + " was problematic");
            
            errorInputs.Remove(field);
            //Debug.Log(errorInputs.Count);
        }
    }

}
