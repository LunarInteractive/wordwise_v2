<?php

// Perantara (Middleware?) dari UserUpdate.cs ke database

$servername = "localhost";
$username = "kasumbai_tes";
$password = "HM7agEekWKpguppNdzcL";
$dbname = "kasumbai_tes";

//Variable yang diambil dari Unity
$id = $_POST["id"];
$newName = $_POST["signUpName"];
$newEmail = $_POST["signUpEmail"];
$newSchool = $_POST["signUpSchool"];
//$signUpClass = $_POST["signUpClass"];
//$photoUrl = $_POST["photoUrl"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname); //connects to database
// Check connection

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// cek jika email yang diganti sudah dipakai user lain
$emailCheckQuery = "SELECT email FROM users WHERE email = '" . $newEmail . "' AND id != '".$id."';";
$emailCheck = mysqli_query($conn, $emailCheckQuery) or die("Query Error");

if ($emailCheck->num_rows > 0) 
{
    echo("1");
    exit();
}

$sql = "UPDATE users SET name = '" . $newName . "', email = '".$newEmail."', school = '".$newSchool."', updated_at = '" . date("Y-m-d H:i:s") . "' WHERE id = '" . $id . "';" ;

mysqli_query($conn, $sql) or die("Query Error");

echo("0");

$conn->close();
?>