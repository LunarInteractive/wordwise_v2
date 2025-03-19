<?php

// Perantara (Middleware?) dari UserRegistration.cs ke database

$servername = "localhost";
$username = "kasumbai_tes";
$password = "HM7agEekWKpguppNdzcL";
$dbname = "kasumbai_tes";


//Variable yang diambil dari Unity
$signUpName = $_POST["signUpName"];
$signUpEmail = $_POST["signUpEmail"];
$signUpPass = $_POST["signUpPass"];
$signUpSchool = $_POST["signUpSchool"];
//$signUpClass = $_POST["signUpClass"];
//$photoUrl = $_POST["photoUrl"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname); //connects to database
// Check connection

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Cek jika email sudah ada
$emailCheckQuery = "SELECT email FROM users WHERE email = '" . $signUpEmail . "';";
$emailCheck = mysqli_query($conn, $emailCheckQuery) or die("Query Error");

if ($emailCheck->num_rows > 0) 
{
    echo("1");
    exit();
}

// hash password
$algorithm = PASSWORD_BCRYPT;
// bcrypt's cost parameter can change over time as hardware improves
$options = ['cost' => 10];

$hash = password_hash($signUpPass, $algorithm, $options);

$sql = "INSERT INTO users (role, name, email, school, password, created_at, updated_at) VALUES ('student', '" . $signUpName . "', '" . $signUpEmail . "', '" . $signUpSchool. "', '" . $hash . "', '" . date("Y-m-d H:i:s") . "', '" . date("Y-m-d H:i:s") . "');";


mysqli_query($conn, $sql) or die("Query Error");

echo("0");

$conn->close();
?>