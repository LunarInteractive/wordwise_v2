<?php

// Perantara (Middleware?) dari SignIn.cs ke database

$servername = "localhost";
$username = "kasumbai_tes";
$password = "HM7agEekWKpguppNdzcL";
$dbname = "kasumbai_tes";

//Variable yang diambil dari Unity
$signInEmail = $_POST["signInEmail"];
$signInPass = $_POST["signInPass"];


// Create connection
$conn = new mysqli($servername, $username, $password, $dbname); //connects to database
// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Cek apakah email ada, dan ambil datanya
$sql = "SELECT id, role, name, email, school, password FROM users WHERE email = '" . $signInEmail ."'";
$result = $conn->query($sql);

$algorithm = PASSWORD_BCRYPT;

// bcrypt's cost parameter can change over time as hardware improves
$options = ['cost' => 10];

if ($result->num_rows == 1) 
{

  // output data of each row
  while($row = $result->fetch_assoc()) 
  {
    
    //$loginPass = crypt($signInPass, $row["salt"]);

    //Cek password
    //Password di re-hash tiap kali login
    if (password_verify($signInPass, $row["password"]))
    {
        // Check if either the algorithm or the options have changed
        if (password_needs_rehash($hash, $algorithm, $options)) {
        // If so, create a new hash, and replace the old one
            $newHash = password_hash($signInPass, $algorithm, $options);

            // Update the user record with the $newHash
            $updatePassQuery = "UPDATE users SET password = '". $newHash ."' WHERE users.id = '" . $row["id"] . "';";
            $updatePass = mysqli_query($conn, $updatePassQuery) or die("Query Error");
        }

        // data yang dikirim ke SignIn.cs
        echo $row["id"] . "\n";
        echo $row["role"] . "\n";
        echo $row["name"] . "\n";
        echo $row["email"] . "\n";
        echo $row["school"];    

        // Jika user sudah terdaftar di class, tambahkan data classcode
        $checkClassQuery = "SELECT class_id FROM joinedclass WHERE user_id = '". $row["id"] ."';";
        $resultClass = $conn->query($checkClassQuery);

 
        if ($resultClass->num_rows == 1)
        {
            $rowClass = $resultClass->fetch_assoc();

            $checkClassNameQuery = "SELECT class_name FROM classes WHERE id = '". $rowClass["class_id"] ."';";
            $resultClassName = $conn->query($checkClassNameQuery);

            if ($resultClassName->num_rows == 1)
            {
                $rowClassName = $resultClassName->fetch_assoc();
                echo "\n" . $rowClassName["class_name"];
            }
        } 

    }
    else
    {
        echo "1";

    }  
  }
} 
else 
{
  echo "10";
}

$conn->close();
?>