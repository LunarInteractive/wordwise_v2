<?php

// Perantara (Middleware?) dari ChangePass.cs ke database

$servername = "localhost";
$username = "kasumbai_tes";
$password = "HM7agEekWKpguppNdzcL";
$dbname = "kasumbai_tes";

//Variable yang diambil dari Unity
$id = $_POST["id"];
$newPass = $_POST["newPass"];
$oldPass = $_POST["oldPass"];


// Create connection
$conn = new mysqli($servername, $username, $password, $dbname); //connects to database
// Check connection

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Cek id
$sql = "SELECT password FROM users WHERE id = '" . $id ."'";
$result = $conn->query($sql);

$algorithm = PASSWORD_BCRYPT;
// bcrypt's cost parameter can change over time as hardware improves
$options = ['cost' => 10];

if ($result->num_rows == 1) 
{

  // output data of each row
  while($row = $result->fetch_assoc()) 
  {
    //check password lama
    if (password_verify($oldPass, $row["password"]))
    {
     
        // hash password baru
        $newHash = password_hash($newPass, $algorithm, $options);

        // Update the user record with the $newHash
        $updatePassQuery = "UPDATE users SET password = '". $newHash ."' WHERE users.id = '" . $id . "';";
        $updatePass = mysqli_query($conn, $updatePassQuery) or die("Query Error");
        
        echo "0";

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