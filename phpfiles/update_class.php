<?php

// Perantara (Middleware?) dari UpdateClass.cs ke database


$servername = "localhost";
$username = "kasumbai_tes";
$password = "HM7agEekWKpguppNdzcL";
$dbname = "kasumbai_tes";

//Variable yang diambil dari Unity
$id = $_POST["id"];
$classCode = $_POST["classCode"];

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname); //connects to database

// Check connection
if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Cek apakah classCode ada atau doble. Jika tidak ada atau double, kelas tidak bisa diikuti.

$classheckQuery = "SELECT id FROM classes WHERE class_name = '" . $classCode."';";
$classCheck = mysqli_query($conn, $classheckQuery) or die("Query Error");

if ($classCheck->num_rows < 1) 
{
    echo("Class not found.");
    exit();
}elseif ($classCheck->num_rows > 1) {
	echo("Duplicate class found. Please contact your teacher.");
    exit();
}

// Saat classCode kelas ditemukan, tambahkan user ke joinedclass
if ($classCheck->num_rows == 1) 
{

  // output data of each row
    $row = $classCheck->fetch_assoc();
  
    $sql = "INSERT INTO joinedclass (user_id, class_id, created_at, updated_at) VALUES('" . $id . "', '" . $row["id"] . "', '" . date("Y-m-d H:i:s") . "', '" . date("Y-m-d H:i:s") . "');" ;

    mysqli_query($conn, $sql) or die("Query Error");

    echo("0");  
  
} 

$conn->close();
?>